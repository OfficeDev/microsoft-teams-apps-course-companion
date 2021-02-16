// <copyright file="discover-wrapper-page.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from 'react';
import * as microsoftTeams from "@microsoft/teams-js";
import { Loader, Grid, gridBehavior } from '@fluentui/react-northstar'
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import { deleteResource, userDownVoteResource, userUpVoteResource, getResources, getResource } from '../../api/resource-api'
import { IResourceDetail, ISubject, IGrade, ICreatedBy, ITag, IFilterModel, IFilterRequestModel, NotificationType, IUserRole, RequestMode } from "../../model/type";
import InfiniteScroll from 'react-infinite-scroller';
import Tile from "../discover-tab/tile"
import NoPostAddedPage from "../discover-tab/no-post-added-page"
import FilterNoPostContentPage from "./filter-no-post-content-page";
import NotificationMessage from '../notification-message/notification-message';
import Resources from '../../constants/resources';
import { getUserRole } from '../../api/member-validation-api';
import TitleBar from '../resource-filter-bar/title-bar';
import { getSelectedFilters, createUserSettingsAsync } from '../../api/user-settings-api';
import { ICheckBoxItem } from '../resource-filter-bar/filter-bar';
import { getAllGrades } from "../../api/grade-api";
import { getAllSubjects } from "../../api/subject-api"
import { getAuthors } from "../../api/resource-api";
import { getAllTags } from "../../api/tag-api"
import { createUserResource } from '../../api/user-resource-api'

import "../../styles/site.css";
import "../../styles/tile.css";

interface IDiscoverPageState {
    windowWidth: number;
    searchText: string;
    alertMessage: string;
    alertType: NotificationType;
    showAlert: boolean;
    showNoPostPage: boolean;
    infiniteScrollParentKey: number;
    isFilterApplied: boolean;
    isPageInitialLoad: boolean;
    pageLoadStart: number;
    hasMorePosts: boolean;
    allResources: IResourceDetail[];
    loading: boolean;
    userRole: IUserRole;
    selectedTags?: Array<string>
    selectedGrades?: Array<string>;
    selectedSubjects?: Array<string>;
    selectedCreatedBy?: Array<string>;
    allSubjects: Array<ISubject>;
    allGrades: Array<IGrade>;
    allTags: Array<ITag>;
    allCreatedBy: Array<ICreatedBy>;
    isTagsFilterCountValid: boolean;
    isGradeFilterCountValid: boolean;
    isSubjectFilterCountValid: boolean;
    isCreatedByFilterCountValid: boolean;
    clickedResourceId: string;
}

/**
* Component for discover resource wrapper page.
*/
class DiscoverPage extends React.Component<WithTranslation, IDiscoverPageState> {

    localize: TFunction;
    userAADObjectId?: string | null = null;
    botId: string = "";
    filterSearchText: string;
    userSetting: IFilterModel;
    history: any;
    allPosts: IResourceDetail[];
    timeout: number | null;

    constructor(props: any) {
        super(props);
        this.timeout = null;
        this.localize = this.props.t;
        this.filterSearchText = "";
        this.allPosts = [];
        this.userSetting = {
            gradeIds: [],
            subjectIds: [],
            tagIds: [],
            createdByObjectIds: [],
        }
        this.state = {
            windowWidth: window.innerWidth,
            searchText: "",
            alertMessage: "",
            alertType: 0,
            showAlert: false,
            showNoPostPage: false,
            infiniteScrollParentKey: 0,
            isFilterApplied: false,
            isPageInitialLoad: true,
            pageLoadStart: -1,
            hasMorePosts: true,
            allResources: [],
            loading: true,
            userRole: {
                isAdmin: false,
                isTeacher: false
            },
            selectedGrades: [],
            selectedSubjects: [],
            selectedCreatedBy: [],
            selectedTags: [],
            allSubjects: [],
            allGrades: [],
            allTags: [],
            allCreatedBy: [],
            isTagsFilterCountValid: true,
            isGradeFilterCountValid: true,
            isSubjectFilterCountValid: true,
            isCreatedByFilterCountValid: true,
            clickedResourceId: "",
        }

        this.history = props.history;
    }

    public async componentDidMount() {
        microsoftTeams.initialize();
        microsoftTeams.getContext((context) => {
            this.userAADObjectId = context.userObjectId!
        });

        window.addEventListener("resize", this.setWindowWidth);

        // Get user role details.
        this.getUserRoles();

        // Get all filters
        this.getFilters();

        // Get saved filters by user for resource
        this.getSelectedFilter();

        // Get initial resources
        this.initResources();
    }

    public componentWillUnmount() {
        window.removeEventListener('resize', this.setWindowWidth);
    }

    /**
    * Fetch all filters
    */
    private getFilters = () => {

        // Get subjects
        this.getSubjects();

        //Get grades
        this.getGrades();

        //Get tags
        this.getTags();

        //Get createdBy
        this.getCreatedBy();
    }

    /**
    * Fetch initial resources
    */
    private initResources = async () => {

        const allResourcesResponse = await getResources(0, this.userSetting);
        if (allResourcesResponse.status === 200 && allResourcesResponse.data) {
            this.allPosts = allResourcesResponse.data;
            if (allResourcesResponse.data.length === 0) {
                this.setState({ loading: false, hasMorePosts: false })
                return;
            }
            this.setState({ loading: false })
        }
    }


    /**
    * Fetch list of subjects from API
    */
    private getSubjects = async () => {
        const subjectResponse = await getAllSubjects(this.handleAuthenticationFailure);
        if (subjectResponse.status === 200 && subjectResponse.data) {
            this.setState({ allSubjects: subjectResponse.data })
        }
    }

    /**
    * Fetch list of grades from API
    */
    private getGrades = async () => {
        const gradeResponse = await getAllGrades(this.handleAuthenticationFailure);
        if (gradeResponse.status === 200 && gradeResponse.data) {
            this.setState({ allGrades: gradeResponse.data });
        }
    }

    /**
    * Fetch list of tags from API
    */
    private getTags = async () => {
        const tagsResponse = await getAllTags(this.handleAuthenticationFailure);
        if (tagsResponse.status === 200 && tagsResponse.data) {
            this.setState({ allTags: tagsResponse.data });
        }
    }

    /**
    * Fetch list of createdBy from API
    */
    private getCreatedBy = async () => {
        const createdByResponse = await getAuthors(this.handleAuthenticationFailure);
        if (createdByResponse.status === 200 && createdByResponse.data) {
            this.setState({ allCreatedBy: createdByResponse.data });
        }
    }

    /**
    * handle error occurred during authentication
    */
    private handleAuthenticationFailure = (error: string) => {
        // When the getAuthToken function returns a "resourceRequiresConsent" error, 
        // it means Azure AD needs the user's consent before issuing a token to the app. 
        // The following code redirects the user to the "Sign in" page where the user can grant the consent. 
        // Right now, the app redirects to the consent page for any error.
        console.error("Error from getAuthToken: ", error);
        this.history.push('/signin');
    }

    /**
    * Sets state for showing alert notification.
    * @param {String} content Notification message
    * @param {Number} type Value indicating 1- Success 2- Error
    */
    private showAlert = (content: string, type: number) => {
        this.setState({ alertMessage: content, alertType: type, showAlert: true }, () => {
            setTimeout(() => {
                this.setState({ showAlert: false })
            }, Resources.alertTimeOut);
        });
    }

    /**
    * Sets state for hiding alert notification.
    */
    private hideAlert = () => {
        this.setState({ showAlert: false });
    }

    /**
    * Get window width real time
    */
    private setWindowWidth = () => {
        if (window.innerWidth !== this.state.windowWidth) {
            this.setState({ windowWidth: window.innerWidth });
        }
    };

    /**
    * Fetch list of saved filters by user for resource.
    */
    private getSelectedFilter = async () => {
        const selectedFilterResponse = await getSelectedFilters(Resources.resourceEntityType);
        if (selectedFilterResponse.status === 200 && selectedFilterResponse.data) {
            let selectedFilters = selectedFilterResponse.data as IFilterModel;
            this.userSetting = selectedFilters;
            this.setState({
                selectedTags: selectedFilters.tagIds,
                selectedGrades: selectedFilters.gradeIds,
                selectedSubjects: selectedFilters.subjectIds,
                selectedCreatedBy: selectedFilters.createdByObjectIds
            });
        }
    }

    /**
    * Navigate to create resource content task module.
    */
    private handleAddNewResourceClick = () => {
        let appBaseUrl = window.location.origin;
        microsoftTeams.tasks.startTask({
            completionBotId: this.botId,
            title: this.localize('createContentTaskModuleHeaderText'),
            height: Resources.taskModuleHeight,
            width: Resources.taskModuleWidth,
            url: `${appBaseUrl}/resourcecontent`,
            fallbackUrl: `${appBaseUrl}/resourcecontent`,
        }, this.addResourceSubmitHandler);
    }

    /**
    * Create resource content task module handler.
    * @param {Any} err resource task module error
    * @param {Any} module resource response data
    */
    private addResourceSubmitHandler = async (err: any, module: any) => {
        if (module) {
            let title = module["title"];
            let isSuccess = module["isSuccess"] === Resources.successFlag

            if (isSuccess) {
                this.showAlert(this.localize("postUpdateSuccess", { "resourceName": title }), NotificationType.Success)
                let allResources = this.state.allResources;
                let saveResponse = module["saveResponse"]
                allResources.unshift(saveResponse);
                this.setState({
                    allResources: allResources
                });
            }
            else {
                this.showAlert(this.localize("postErrorMessage", { "resourceName": title }), NotificationType.Failure)
            }
        }
    };

    /**
    * Navigate to edit resource content task module.
    * @param {String} resourceId resource unique identifier.
    */
    private handleEditClick = (resourceId: string) => {
        let appBaseUrl = window.location.origin;
        microsoftTeams.tasks.startTask({
            completionBotId: this.botId,
            title: this.localize('editContentTaskModuleHeaderText'),
            height: Resources.taskModuleHeight,
            width: Resources.taskModuleWidth,
            url: `${appBaseUrl}/resourcecontent?viewMode=${RequestMode.edit}&resourceId=${resourceId}`,
            fallbackUrl: `${appBaseUrl}/resourcecontent?viewMode=${RequestMode.edit}&resourceId=${resourceId}`,
        }, this.editResourceSubmitHandler);
    }

    /**
    * Edit resource content task module handler.
    * @param {Any} err resource task module error
    * @param {Any} module resource response data
    */
    private editResourceSubmitHandler = async (err: any, module: any) => {
        if (module) {
            let title = module["title"];
            let isSuccess = module["isSuccess"] === Resources.successFlag;

            if (isSuccess) {
                this.showAlert(this.localize("postUpdateSuccess", { "resourceName": title }), NotificationType.Success);
                let saveResponse = module["saveResponse"];
                let allFilteredResource = this.state.allResources.filter((resource: IResourceDetail) => {
                    return resource.id !== saveResponse.id;
                });

                allFilteredResource.unshift(saveResponse);
                this.setState({
                    allResources: allFilteredResource
                });
            }
            else {
                this.showAlert(this.localize("postErrorMessage", { "resourceName": title }), NotificationType.Failure);
            }
        }
    };

    /**
    * Delete resource.
    * @param resourceId {String} resource unique identifier.
    */
    private handleDeleteClick = async (resourceId: string) => {
        let deleteResourceResponse = await deleteResource(resourceId);
        if (deleteResourceResponse.status === 200) {
            this.handleRemoveDeletedResourceFromList(resourceId);
            this.showAlert(this.localize("resourceDeleteSuccess"), NotificationType.Success);
        } else {
            this.showAlert(this.localize("unableToDeleteResource"), NotificationType.Failure);
        }
    }

    /**
    * Remove resource from list.
    * Invoked when user clicks on remove module from saved list
    */
    private handleRemoveDeletedResourceFromList = (resourceId: string) => {
        let allResources = [...this.state.allResources];
        let resourceIndex = allResources.findIndex((resource: IResourceDetail) => resource.id === resourceId)!;
        allResources.splice(resourceIndex, 1);
        this.setState({ allResources: allResources });
    }

    /**
    * Navigate to preview resource content task module.
    * @param {String} resourceId resource unique identifier.
    */
    private handlePreviewClick = (resourceId: string) => {
        let appBaseUrl = window.location.origin;
        this.setState({ clickedResourceId: resourceId });
        microsoftTeams.tasks.startTask({
            completionBotId: this.botId,
            title: this.localize('previewContentTaskModuleHeaderText'),
            height: Resources.taskModuleHeight,
            width: Resources.taskModuleWidth,
            url: `${appBaseUrl}/previewcontent?viewMode=1&resourceId=${resourceId}`,
            fallbackUrl: `${appBaseUrl}/previewcontent?viewMode=1&resourceId=${resourceId}`,
        }, this.previewClickSubmitHandler);
    }

    /**
    * Preview resource content task module handler.
    */
    private previewClickSubmitHandler = async () => {
        let resourceId = this.state.clickedResourceId;
        const resourceDetailsResponse = await getResource(resourceId);
        if (resourceDetailsResponse !== null && resourceDetailsResponse.data) {
            let resourceDetail: IResourceDetail = resourceDetailsResponse.data;
            let allResources = this.state.allResources.map((resource: IResourceDetail) => resource.id === resourceId ? resourceDetail : resource);
            this.setState({ allResources: allResources });
        }
    };

    /**
    * Navigate to add resource to learning module task module.
    * @param {String} gradeId grade unique identifier.
    * @param {String} subjectId subject unique identifier.
    * @param {String} resourceId resource unique identifier.
    */
    private handleAddToLearningModuleClick = async (gradeId?: string, subjectId?: string, resourceId?: string) => {
        let appBaseUrl = window.location.origin;
        microsoftTeams.tasks.startTask({
            completionBotId: this.botId,
            title: this.localize('addLearningModuleText'),
            height: Resources.taskModuleHeight,
            width: Resources.taskModuleWidth,
            url: `${appBaseUrl}/addlearningitems?gradeId=${gradeId}&subjectId=${subjectId}&resourceId=${resourceId}`,
            fallbackUrl: `${appBaseUrl}/addlearningitems?gradeId=${gradeId}subjectId=${subjectId}&resourceId=${resourceId}`,
        }, this.addToLearningModuleSubmitHandler);
    }

    /**
    * handleAddToLearningModuleClick content task module handler.
    * @param {Any} err learning module task module error
    * @param {Any} module learning module response data
    */
    private addToLearningModuleSubmitHandler = async (err: any, result: any) => {
        if (result != null) {
            if (result["isSuccess"]) {
                this.showAlert(this.localize("resourceAddedToLearningModule"), NotificationType.Success);
            } else if (result["isDuplicate"]) {
                this.showAlert(this.localize("resourceAddedToLearningModuleDuplicate"), NotificationType.Failure);
            }
            else {
                this.showAlert(this.localize("postErrorMessage"), NotificationType.Failure);
            }
        }
    };

    /**
    * Add resource to private list.
    * @param {String} resourceId resource unique identifier.
    */
    private handleAddToUserResourcesClick = async (resourceId: string) => {
        let userResource = {
            resourceid: resourceId,
        };
        const addResourceResponse = await createUserResource(userResource);
        if (addResourceResponse.status === 200 && addResourceResponse.data !== null) {
            this.showAlert(this.localize("addToUserListSuccess"), NotificationType.Success)
        } else if (addResourceResponse.status === 409) {
            this.showAlert(this.localize("resourceAlreadyExistsInUserList"), NotificationType.Failure)
        }
        else {
            this.showAlert(this.localize("addToUserListResourceError"), NotificationType.Failure)
        }
    }

    /**
    * Get user role details.
    */
    private getUserRoles = async () => {
        const userRole = await getUserRole(this.handleAuthenticationFailure);
        if (userRole.status === 200 && userRole.data) {
            this.setState({ userRole: userRole.data });
        }
    }

    /**
    * Get all resource based on current page count. 
    * @param {Number} page Page count to get resource.
    */
    private getAllResource = async (page: number) => {

        const allResourcesResponse = await getResources(page, this.userSetting);
        if (allResourcesResponse.status === 200 && allResourcesResponse.data) {
            let existingResources = [...this.state.allResources];
            allResourcesResponse.data.forEach((resource: IResourceDetail) => {
                existingResources.push(resource);
            });
            this.allPosts = this.state.allResources;
            this.setState({ allResources: existingResources, isPageInitialLoad: false, hasMorePosts: allResourcesResponse.data.length > 9 });
        }
    }

    /**
    * Invoked when either filter bar is displayed or closed
    * @param isOpen Boolean indicating whether filter bar is displayed or closed.
    */
    handleFilterClear = (isOpen: boolean) => {
        this.setState({ isFilterApplied: isOpen });
    }

    /**
    * Reset filter settings when reset icon is clicked.
    */
    private onResetFilters = () => {
        this.userSetting.gradeIds = [];
        this.userSetting.subjectIds = [];
        this.userSetting.tagIds = [];
        this.userSetting.createdByObjectIds = [];
        this.onFilterInputChange();
    }

    /**
     * Set state of search text as per user input change
     * @param {String} searchText Search text entered by user
     */
    private handleSearchInputChange = (searchText: string) => {

        if (searchText.length === 0) {
            this.setState({
                searchText: searchText,
                isPageInitialLoad: true,
                pageLoadStart: -1,
                infiniteScrollParentKey: this.state.infiniteScrollParentKey + 1,
                allResources: [],
                hasMorePosts: true,
                isFilterApplied: false
            });
        }
        else {
            this.setState({ isFilterApplied: true, searchText: searchText })
        }

    }

    /**
    * Filter tiles based on 'grade' checkbox selection.
    * @param {Array<ICheckBoxItem>} selectedCheckboxes User selected checkbox array
    */
    private onGradeCheckboxStateChange = (selectedCheckboxes: Array<ICheckBoxItem>) => {
        this.userSetting.gradeIds = [];
        let selectedGrades: Array<ICheckBoxItem> = selectedCheckboxes.filter((checkboxItem: ICheckBoxItem) => {
            return checkboxItem.isChecked;
        });

        this.userSetting.gradeIds = selectedGrades.map((gradeCheckboxItem: ICheckBoxItem) => gradeCheckboxItem.id);
        this.onFilterInputChange();
    }

    /**
    * Filter tiles based on 'subject' checkbox selection.
    * @param {Array<ICheckBoxItem>} selectedCheckboxes User selected checkbox array
    */
    private onSubjectCheckboxStateChange = (selectedCheckboxes: Array<ICheckBoxItem>) => {
        this.userSetting.subjectIds = [];
        let selectedSubjects: Array<ICheckBoxItem> = selectedCheckboxes.filter((checkboxItem: ICheckBoxItem) => {
            return checkboxItem.isChecked;
        });

        this.userSetting.subjectIds = selectedSubjects.map((subjectCheckboxItem: ICheckBoxItem) => subjectCheckboxItem.id);
        this.onFilterInputChange();
    }

    /**
    * Filter tiles based on 'tags' check box selection.
    * @param {Array<ICheckBoxItem>} selectedCheckboxes User selected check box array
    */
    private onTagsCheckboxStateChange = (selectedCheckboxes: Array<ICheckBoxItem>) => {
        this.userSetting.tagIds = [];
        let selectedTags: Array<ICheckBoxItem> = selectedCheckboxes.filter((checkboxItem: ICheckBoxItem) => {
            return checkboxItem.isChecked;
        });

        this.userSetting.tagIds = selectedTags.map((tagCheckboxItem: ICheckBoxItem) => tagCheckboxItem.id);
        this.onFilterInputChange();
    }

    /**
    * Filter tiles based on 'createdBy' check box selection.
    * @param {Array<ICheckBoxItem>} selectedCheckboxes User selected check box array
    */
    private onAddedByCheckboxStateChange = (selectedCheckboxes: Array<ICheckBoxItem>) => {
        this.userSetting.createdByObjectIds = [];
        let selectedCreatedBy: Array<ICheckBoxItem> = selectedCheckboxes.filter((checkboxItem: ICheckBoxItem) => {
            return checkboxItem.isChecked;
        });

        this.userSetting.createdByObjectIds = selectedCreatedBy.map((createdByCheckboxItem: ICheckBoxItem) => createdByCheckboxItem.id);
        this.onFilterInputChange();
    }

    /**
    * Update filter state when filter value changes
    */
    private onFilterInputChange = async () => {
        let isFilterOn: boolean = false;
        let isTagsFilterCountValid: boolean = true;
        let isSubjectFilterCountValid: boolean = true;
        let isGradeFilterCountValid: boolean = true;
        let isCreatedByFilterCountValid: boolean = true;

        if (this.userSetting.createdByObjectIds.length ||
            this.userSetting.gradeIds.length ||
            this.userSetting.subjectIds.length ||
            this.userSetting.tagIds.length) {
            isFilterOn = true;
        }

        if (this.userSetting.createdByObjectIds.length > Resources.maxSelectedFilters) {
            isCreatedByFilterCountValid = false;
        }
        if (this.userSetting.subjectIds.length > Resources.maxSelectedFilters) {
            isSubjectFilterCountValid = false;
        }
        if (this.userSetting.tagIds.length > Resources.maxSelectedFilters) {
            isTagsFilterCountValid = false;
        }
        if (this.userSetting.gradeIds.length > Resources.maxSelectedFilters) {
            isGradeFilterCountValid = false;
        }

        this.setState({
            isFilterApplied: isFilterOn,
            allResources: [],
            hasMorePosts: true,
            isPageInitialLoad: true,
            pageLoadStart: -1,
            infiniteScrollParentKey: this.state.infiniteScrollParentKey + 1,
            searchText: "",
            isTagsFilterCountValid: isTagsFilterCountValid,
            isGradeFilterCountValid: isGradeFilterCountValid,
            isSubjectFilterCountValid: isSubjectFilterCountValid,
            isCreatedByFilterCountValid: isCreatedByFilterCountValid
        });
    }

    /**
    * Save user resource filter settings.
    * @param {IFilterModel} resourceSettings filter settings done by user for resource.
    */
    private onFilterSaved = async (resourceSettings: IFilterModel) => {
        const saveFilterResponse = await createUserSettingsAsync(resourceSettings, Resources.resourceEntityType);
        if (saveFilterResponse.status === 200) {
            this.showAlert(this.localize("filterSaveSuccessText"), NotificationType.Success);
            this.setState({ isFilterApplied: false });
        }
    }

    /**
    * Fetch resources based on user filter settings
    * @param {Number} page Page count for which next set of resources needs to be fetched.
    */
    private getFilteredResources = async (page: number) => {
        const resourceDetails = await getResources(page, this.userSetting);
        if (resourceDetails.status === 200 && resourceDetails.data) {
            let existingResources = [...this.state.allResources];
            if (page === 0) {
                existingResources = resourceDetails.data;
            }
            else {
                resourceDetails.data.forEach((resource: IResourceDetail) => {
                    existingResources.push(resource);
                });
            }

            this.setState({ allResources: existingResources, isPageInitialLoad: false, hasMorePosts: resourceDetails.data.length == Resources.recordsToLoad });
            this.allPosts = this.state.allResources;
        }
    }

    /**
    * Method gets invoked when user clicks on like.
    * @param {String} resourceId resource unique identifier.
    */
    private handleVoteClick = async (resourceId: string) => {
        let allResources = this.state.allResources.map((resource: IResourceDetail) => ({ ...resource }));
        let resourceIndex = allResources.findIndex((resource: IResourceDetail) => resource.id === resourceId);
        let resourceDetail = allResources[resourceIndex];
        let resourceLikedByUser = resourceDetail.isLikedByUser;

        if (resourceLikedByUser) {
            let userDownVoteResourceResponse = await userDownVoteResource(resourceDetail.id);
            if (userDownVoteResourceResponse.status === 200 && userDownVoteResourceResponse.data) {
                resourceDetail.isLikedByUser = false;
                resourceDetail.voteCount = resourceDetail.voteCount ? resourceDetail.voteCount - 1 : resourceDetail.voteCount;
            }
        } else {
            let userUpVoteResourceResponse = await userUpVoteResource(resourceDetail.id);
            if (userUpVoteResourceResponse.status === 200 && userUpVoteResourceResponse.data) {
                resourceDetail.isLikedByUser = true;
                resourceDetail.voteCount = resourceDetail.voteCount! + 1;
            }
        }
        allResources[resourceIndex] = resourceDetail;
        this.setState({ allResources: allResources });
    }

    /**
    * Invoked by Infinite scroll component when user scrolls down to fetch next set of projects.
    * @param {Number} page Page count for which next set of resources needs to be fetched.
    */
    private loadMoreProjects = (page: number) => {
        if (this.state.hasMorePosts) {
            if (this.state.searchText.trim().length) {
                this.searchFilterPostUsingAPI(page);
            }
            else if (this.state.isFilterApplied) {
                if (this.timeout) {
                    window.clearTimeout(this.timeout);
                }

                this.timeout = window.setTimeout(async () => {
                    this.getFilteredResources(page);
                }, Resources.randomClicksTimeOut);
            }
            else {
                this.getAllResource(page);
            }
        }
    }

    /**
    * Get resources data from API using search text entered by user.
    * @param {Number} page Page count for which next set of resources needs to be fetched.
    */
    private searchFilterPostUsingAPI = async (page: number) => {
        let isFilterOn: boolean = false;
        const searchText = this.state.searchText.trim();
        if (searchText.length) {
            isFilterOn = true;
            const searchRequestDetails: IFilterRequestModel = {
                searchText: searchText
            };

            let response = await getResources(page, searchRequestDetails);

            if (response.status === 200 && response.data) {
                this.setState({
                    allResources: response.data,
                    hasMorePosts: response.data.length > Resources.recordsToLoad,
                    loading: false,
                    isFilterApplied: isFilterOn
                });
            }
            else {
                this.setState({ loading: false, isFilterApplied: isFilterOn })
            }
        }
    }

    /**
    * Invoked when user hits enter or clicks on search icon for searching post through command bar
    */
    private invokeApiSearch = () => {
        this.setState({
            isPageInitialLoad: true,
            pageLoadStart: -1,
            infiniteScrollParentKey: this.state.infiniteScrollParentKey + 1,
            allResources: [],
            isFilterApplied: false,
            hasMorePosts: true
        });
    }

    private renderDiscoverTabContent() {

        // Cards component array to be rendered on grid.
        const cards = new Array<JSX.Element>();
        const tiles = this.state.allResources.map((resource: IResourceDetail) => (<Tile index={resource.id}
            resourceDetails={resource}
            handleEditClick={this.handleEditClick}
            handlePreviewClick={this.handlePreviewClick}
            handleDeleteClick={this.handleDeleteClick}
            handleVoteClick={this.handleVoteClick}
            currentUserId={this.userAADObjectId!}
            userRole={this.state.userRole}
            handleAddToLearningModuleClick={this.handleAddToLearningModuleClick}
            handleAddToUserResourcesClick={this.handleAddToUserResourcesClick}
        />));

        if (tiles.length > 0) {
            cards.push(
                <Grid columns={this.state.windowWidth > Resources.maxWidthForMobileView ? Resources.threeColumnGrid : Resources.oneColumnGrid}
                    accessibility={gridBehavior}
                    className="tile-render"
                    content={tiles}>
                </Grid>);
        }

        let scrollViewStyle = { height: this.state.isFilterApplied === true ? "75vh" : "80vh" };
        return (
            <div className="container-div">
                <div className="container-subdiv-cardview">
                    <div className="container-fluid-overriden">
                        <NotificationMessage
                            onClose={this.hideAlert}
                            showAlert={this.state.showAlert}
                            content={this.state.alertMessage}
                            notificationType={this.state.alertType}
                        />
                        <TitleBar
                            commandBarSearchText={this.state.searchText}
                            onFilterClear={this.handleFilterClear}
                            hideFilterbar={!this.state.isFilterApplied}
                            onSearchInputChange={this.handleSearchInputChange}
                            onGradeCheckboxStateChange={this.onGradeCheckboxStateChange}
                            onSubjectCheckboxStateChange={this.onSubjectCheckboxStateChange}
                            onTagsCheckboxStateChange={this.onTagsCheckboxStateChange}
                            onAddedByCheckboxStateChange={this.onAddedByCheckboxStateChange}
                            handleAddClick={this.handleAddNewResourceClick}
                            userRole={this.state.userRole}
                            searchFilterPostsUsingAPI={this.invokeApiSearch}
                            onFilterChangesSaved={this.onFilterSaved}
                            selectedGrades={this.state.selectedGrades}
                            selectedSubjects={this.state.selectedSubjects}
                            selectedTags={this.state.selectedTags}
                            selectedCreatedBy={this.state.selectedCreatedBy}
                            onResetFilterClick={this.onResetFilters}
                            allGrades={this.state.allGrades}
                            allSubjects={this.state.allSubjects}
                            allTags={this.state.allTags}
                            allCreatedBy={this.state.allCreatedBy}
                            isTagsFilterCountValid={this.state.isTagsFilterCountValid}
                            isGradeFilterCountValid={this.state.isGradeFilterCountValid}
                            isSubjectFilterCountValid={this.state.isSubjectFilterCountValid}
                            isCreatedByFilterCountValid={this.state.isCreatedByFilterCountValid}

                        />
                        <div key={this.state.infiniteScrollParentKey} className="scroll-view scroll-view-mobile" style={scrollViewStyle}>
                            <InfiniteScroll
                                pageStart={this.state.pageLoadStart}
                                loadMore={this.loadMoreProjects}
                                hasMore={this.state.hasMorePosts}
                                initialLoad={this.state.isPageInitialLoad}
                                useWindow={false}
                                loader={<div className="loader"><Loader /></div>}>

                                {
                                    tiles.length ?
                                        cards : (this.state.hasMorePosts ?
                                            <></> : (this.state.isFilterApplied ? < FilterNoPostContentPage /> : <NoPostAddedPage handleAddNewResource={this.handleAddNewResourceClick} isValidUser={(this.state.userRole.isAdmin || this.state.userRole.isTeacher)} />))
                                }

                            </InfiniteScroll>
                        </div>
                    </div>
                </div>
            </div>
        )
    }

    /**
    * Renders the component.
    */
    public render() {
        let contents = this.state.loading
            ? <p><Loader /></p>
            : this.renderDiscoverTabContent();
        return (
            <div>
                {contents}
            </div>
        );
    }
}

export default withTranslation()(DiscoverPage)