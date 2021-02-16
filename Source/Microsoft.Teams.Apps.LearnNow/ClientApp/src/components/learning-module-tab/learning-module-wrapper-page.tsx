// <copyright file="learning-module-wrapper-page.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from 'react';
import * as microsoftTeams from "@microsoft/teams-js";
import { Loader, Grid, gridBehavior } from '@fluentui/react-northstar'
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import { ILearningModuleDetail, ISubject, IGrade, ICreatedBy, ITag, IFilterModel, IFilterRequestModel, NotificationType, IUserRole, IModuleResourceViewModel, RequestMode } from "../../model/type";
import { deleteLearningModule, getAuthors, userDownVoteLearningModule, userUpVoteLearningModule, getLearningModules, getLearningModule } from '../../api/learning-module-api';
import InfiniteScroll from 'react-infinite-scroller';
import LearningModuleTile from "./learning-module-tile"
import FilterNoPostContentPage from "../discover-tab/filter-no-post-content-page";
import NoPostAddedPage from "../discover-tab/no-post-added-page"
import NotificationMessage from '../notification-message/notification-message';
import TitleBar from '../resource-filter-bar/title-bar';
import { getSelectedFilters, createUserSettingsAsync } from '../../api/user-settings-api';
import { ICheckBoxItem } from '../resource-filter-bar/filter-bar';
import { getUserRole } from '../../api/member-validation-api';
import { getAllGrades } from "../../api/grade-api";
import { getAllSubjects } from "../../api/subject-api"
import { getAllTags } from "../../api/tag-api"
import Resources from '../../constants/resources';
import { createUserLearningModule } from '../../api/user-learning-module-api';

import "../../styles/site.css";
import "../../styles/tile.css";

interface ILearningModulePageState {
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
    allLearningModules: ILearningModuleDetail[];
    loading: boolean,
    selectedTags?: Array<string>
    selectedGrades?: Array<string>;
    selectedSubjects?: Array<string>;
    selectedCreatedBy?: Array<string>;
    userRole: IUserRole;
    allSubjects: Array<ISubject>;
    allGrades: Array<IGrade>;
    allTags: Array<ITag>;
    allAddedBy: Array<ICreatedBy>;
    isTagsFilterCountValid: boolean;
    isGradeFilterCountValid: boolean;
    isSubjectFilterCountValid: boolean;
    isCreatedByFilterCountValid: boolean;
    clickedModuleId: string;
}

/**
* Component for rendering learning module wrapper page.
*/
class LearningModulePage extends React.Component<WithTranslation, ILearningModulePageState> {

    localize: TFunction;
    userAADObjectId?: string | null = null;
    botId: string = "";
    allPosts: ILearningModuleDetail[]
    userSetting: IFilterModel;
    filterSearchText: string;
    history: any;
    timeout: number | null;

    constructor(props: any) {
        super(props);
        this.timeout = null;
        this.localize = this.props.t;
        this.allPosts = [];
        this.filterSearchText = "";
        this.userSetting = {
            tagIds: [],
            subjectIds: [],
            gradeIds: [],
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
            allLearningModules: [],
            loading: true,
            selectedGrades: [],
            selectedSubjects: [],
            selectedCreatedBy: [],
            selectedTags: [],
            userRole: {
                isAdmin: false,
                isTeacher: false
            },
            allSubjects: [],
            allGrades: [],
            allTags: [],
            allAddedBy: [],
            isTagsFilterCountValid: true,
            isGradeFilterCountValid: true,
            isSubjectFilterCountValid: true,
            isCreatedByFilterCountValid: true,
            clickedModuleId: "",
        }
    }

    /**
    * Get initial set of data when component is mounted.
    */
    public async componentDidMount() {

        microsoftTeams.initialize();
        microsoftTeams.getContext((context: microsoftTeams.Context) => {
            this.userAADObjectId = context.userObjectId!
        });

        // Get user role details.
        this.getUserRoles();

        // Get all filters
        await this.getFilters();

        // Get saved filters by user for learning module
        this.getSelectedFilter();

        // Fetch initial learning modules
        this.initLearningModules();

        window.addEventListener("resize", this.setWindowWidth);
    }

    public componentWillUnmount() {
        window.removeEventListener('resize', this.setWindowWidth);
    }

    /**
    * Fetch all filters
    */
    getFilters = () => {

        // Get subjects
        this.getSubjects();

        //Get grades
        this.getGrades();

        //Get tags
        this.getTags();

        //Get authors
        this.getAddedBy();

    }

    /**
    * Fetch initial learning modules
    */
    private initLearningModules = async () => {

        const allLearningModuleResponse = await getLearningModules(0, this.userSetting);
        if (allLearningModuleResponse.status === 200 && allLearningModuleResponse.data) {
            this.allPosts = allLearningModuleResponse.data;
            if (allLearningModuleResponse.data.length === 0) {
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
            this.setState({ allGrades: gradeResponse.data })
        }
    }

    /**
    * Fetch list of tags from API
    */
    private getTags = async () => {
        const tagsResponse = await getAllTags(this.handleAuthenticationFailure);
        if (tagsResponse.status === 200 && tagsResponse.data) {
            this.setState({ allTags: tagsResponse.data })
        }
    }

    /**
    * Fetch list of authors from API
    */
    private getAddedBy = async () => {
        const authorResponse = await getAuthors(this.handleAuthenticationFailure);
        if (authorResponse.status === 200 && authorResponse.data) {
            this.setState({ allAddedBy: authorResponse.data })
        }
    }

    /**
    * Fetch list of saved filters by user for learning module.
    */
    private getSelectedFilter = async () => {
        const selectedFilterResponse = await getSelectedFilters(Resources.learningModuleEntityType);
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
    * Sets state for showing alert notification.
    * @param {String} content Notification message
    * @param {Number} type Boolean value indicating 1- Success 2- Error
    */
    private showAlert = (content: string, type: number) => {
        this.setState({ alertMessage: content, alertType: type, showAlert: true }, () => {
            setTimeout(() => {
                this.hideAlert();
            }, Resources.alertTimeOut);
        });
    }

    /**
    * Sets state for hiding alert notification.
    */
    private hideAlert = () => {
        this.setState({ showAlert: false })
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
    * Navigate to create learningModule task module.
    */
    private handleAddNewLearningModuleClick = () => {
        let appBaseUrl = window.location.origin;
        microsoftTeams.tasks.startTask({
            completionBotId: this.botId,
            title: this.localize('createLMContentTaskModuleHeaderText'),
            height: Resources.taskModuleHeight,
            width: Resources.taskModuleWidth,
            url: `${appBaseUrl}/createmodule?viewMode=${RequestMode.create}`,
            fallbackUrl: `${appBaseUrl}/createmodule?viewMode=${RequestMode.create}`,
        }, this.addModuleSubmitHandler);
    }

    /**
    * Navigate to edit module task module.
    * @param {String} learningModuleId learning module identifier
    */
    private handleEditClick = (learningModuleId: string) => {
        let appBaseUrl = window.location.origin;
        microsoftTeams.tasks.startTask({
            completionBotId: this.botId,
            title: this.localize('editLMContentTaskModuleHeaderText'),
            height: Resources.taskModuleHeight,
            width: Resources.taskModuleWidth,
            url: `${appBaseUrl}/createmodule?viewMode=${RequestMode.edit}&resourceId=${learningModuleId}`,
            fallbackUrl: `${appBaseUrl}/createmodule?viewMode=${RequestMode.edit}&resourceId=${learningModuleId}`,
        }, this.updateResourceSubmitHandler);
    }

    /**
    * Create resource content task module handler.
    * @param {Any} err learning module task module error
    * @param {Any} module learning module response data
    */
    private updateResourceSubmitHandler = async (err: any, module: any) => {
        if (module) {
            let title = module["title"];
            let isSuccess = module["isSuccess"] === Resources.successFlag;

            if (isSuccess) {
                this.showAlert(this.localize("postUpdateSuccess", { "resourceName": title }), NotificationType.Success);
                let saveResponse = module["saveResponse"];
                let allFilteredModules = this.state.allLearningModules.filter((learningModule: ILearningModuleDetail) => {
                    return learningModule.id !== saveResponse.id;
                });
                allFilteredModules.unshift(saveResponse);
                this.setState({
                    allLearningModules: allFilteredModules
                });
            } else {
                this.showAlert(this.localize("postErrorMessage", { "resourceName": title }), NotificationType.Failure);
            }
        }
    };

    /**
    * Create resource content task module handler.
    * @param {Any} err learning module task module error
    * @param {Any} module learning module response data
    */
    private addModuleSubmitHandler = async (err: any, module: any) => {
        if (module) {
            let title = module["title"];
            let isSuccess = module["isSuccess"] === Resources.successFlag;
            if (isSuccess) {
                this.showAlert(this.localize("postUpdateSuccess", { "resourceName": title }), NotificationType.Success);
                let allModules = this.state.allLearningModules;
                let saveResponse = module["saveResponse"]
                allModules.unshift(saveResponse);
                this.setState({
                    allLearningModules: allModules
                });

            } else {
                this.showAlert(this.localize("postErrorMessage", { "resourceName": title }), NotificationType.Failure);
            }
        }
    };

    /**
    * Navigate to preview learningModule content task module.
    * @param {String} learningModuleId learning module identifier
    */
    private handlePreviewClick = (learningModuleId: string) => {
        let appBaseUrl = window.location.origin;
        this.setState({ clickedModuleId: learningModuleId });
        microsoftTeams.tasks.startTask({
            completionBotId: this.botId,
            title: this.localize('previewContentLMTaskModuleHeaderText'),
            height: Resources.taskModuleHeight,
            width: Resources.taskModuleWidth,
            url: `${appBaseUrl}/learningmodulepreview?viewMode=1&learningModuleId=${learningModuleId}`,
            fallbackUrl: `${appBaseUrl}/learningmodulepreview?viewMode=1&learningModuleId=${learningModuleId}`,
        }, this.previewClickSubmitHandler);
    }

    /**
    * Preview module content task module handler.
    */
    private previewClickSubmitHandler = async () => {
        let moduleId = this.state.clickedModuleId;
        const moduleDetailsResponse = await getLearningModule(moduleId);
        if (moduleDetailsResponse !== null && moduleDetailsResponse.data) {

            let moduleDetailsResponseData: IModuleResourceViewModel = moduleDetailsResponse.data;
            let moduleDetail = moduleDetailsResponseData.learningModule;

            let allLearningModules = this.state.allLearningModules.map((module: ILearningModuleDetail) => module.id === moduleId ? moduleDetail : module);
            this.setState({ allLearningModules: allLearningModules });
        }
    };

    /**
    * Add learningModule to private list.
    * @param learningModuleId {String} learningModule unique identifier.
    */
    private handleAddToUserModuleClick = async (learningModuleId: string) => {
        let userLearningModule = {
            learningmoduleid: learningModuleId,
        };
        const addLearningModuleResponse = await createUserLearningModule(userLearningModule);
        if (addLearningModuleResponse.status === 200 && addLearningModuleResponse.data !== null) {
            this.showAlert(this.localize("addToUserListSuccess"), NotificationType.Success)
        } else if (addLearningModuleResponse.status === 409) {
            this.showAlert(this.localize("moduleAlreadyExistsInUserList"), NotificationType.Success)
        }
        else {
            this.showAlert(this.localize("addToUserListModuleError"), NotificationType.Failure)
        }
    }

    /**
    * Save user learning module filter settings.
    */
    private onFilterSaved = async (resourceSettings: IFilterModel) => {
        const saveFilterResponse = await createUserSettingsAsync(resourceSettings, Resources.learningModuleEntityType);
        if (saveFilterResponse.status === 200) {
            this.showAlert(this.localize("filterSaveSuccessText"), NotificationType.Success);
            this.setState({ isFilterApplied: false });
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
    * Set state of search text as per user input change
    * @param searchText {String} Search text entered by user
    */
    private handleSearchInputChange = (searchText: string) => {
        this.filterSearchText = searchText;

        if (searchText.length === 0) {
            this.setState({
                searchText: searchText,
                isPageInitialLoad: true,
                pageLoadStart: -1,
                infiniteScrollParentKey: this.state.infiniteScrollParentKey + 1,
                allLearningModules: [],
                hasMorePosts: true,
                isFilterApplied: false
            });
        }
        else {
            this.setState({ isFilterApplied: true, searchText: searchText })
        }
    }

    /**
    * Update selected grades state and call API based on tags check-box selection.
    * @param {Array<ICheckBoxItem>} selectedCheckboxes User selected check-box array
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
    * Update selected subject state and call API based on tags check-box selection.
    * @param {Array<ICheckBoxItem>} selectedCheckboxes User selected check-box array
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
    * Update selected tags state and call API based on tags check-box selection.
    * @param {Array<ICheckBoxItem>} selectedCheckboxes User selected check-box array
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
    * Update selected authors state and call API based on tags check-box selection.
    * @param {Array<ICheckBoxItem>} selectedCheckboxes User selected check-box array
    */
    private onAddedByCheckboxStateChange = (selectedCheckboxes: Array<ICheckBoxItem>) => {
        this.userSetting.createdByObjectIds = [];
        let selectedAuthors: Array<ICheckBoxItem> = selectedCheckboxes.filter((checkboxItem: ICheckBoxItem) => {
            return checkboxItem.isChecked;
        });

        this.userSetting.createdByObjectIds = selectedAuthors.map((createdByCheckboxItem: ICheckBoxItem) => createdByCheckboxItem.id);
        this.onFilterInputChange();
    }

    /**
    * Reset filter settings when reset icon is clicked.
    */
    private onResetFilters = () => {
        this.userSetting.tagIds = [];
        this.userSetting.subjectIds = [];
        this.userSetting.gradeIds = [];
        this.userSetting.createdByObjectIds = [];
        this.onFilterInputChange();
    }

    /**
    * Update filter state when filter value changes.
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

        this.setState({
            isFilterApplied: isFilterOn,
            allLearningModules: [],
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
    * Get learning modules based on the filters applied and the page count..
    * @param {Number} page Page number based on next paged data is fetched from API
    */
    private getFilteredLearningModules = async (page: number) => {
        const moduleDetails = await getLearningModules(page, this.userSetting);
        if (moduleDetails.status === 200 && moduleDetails.data) {
            let existingLearningModules = [...this.state.allLearningModules];
            if (page === 0) {
                existingLearningModules = moduleDetails.data;
            }
            else {
                moduleDetails.data.forEach((learningModule: ILearningModuleDetail) => {
                    existingLearningModules.push(learningModule);
                });
            }

            this.setState({ allLearningModules: existingLearningModules, isPageInitialLoad: false, hasMorePosts: moduleDetails.data.length == Resources.recordsToLoad });
            this.allPosts = this.state.allLearningModules;
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
            isFilterApplied: false,
            hasMorePosts: true,
            allLearningModules: []
        });
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
    * Get user role details.
    */
    private getUserRoles = async () => {
        const userRole = await getUserRole(this.handleAuthenticationFailure);
        if (userRole.status === 200 && userRole.data) {
            this.setState({ userRole: userRole.data });
        }
    }

    /**
    * Get all learningModule based on current page count. 
    * @param {Number} page Page number based on next paged data is fetched from API
    */
    private getAllLearningModule = async (page: number) => {

        const allLearningModulesResponse = await getLearningModules(page, this.userSetting);
        if (allLearningModulesResponse.status === 200 && allLearningModulesResponse.data) {

            let existingLearningModules = [...this.state.allLearningModules];
            allLearningModulesResponse.data.map((learningModule: ILearningModuleDetail) => {
                existingLearningModules.push(learningModule);
            });

            this.setState({ allLearningModules: existingLearningModules, isPageInitialLoad: false, hasMorePosts: allLearningModulesResponse.data.length > Resources.recordsToLoad, loading: false });
            this.allPosts = this.state.allLearningModules;
        }
    }

    /**
    * Method gets invoked when user clicks on like.
    * @param {String} moduleId learning module identifier.
    */
    private handleVoteClick = async (moduleId: string) => {
        let allLearningModules = this.state.allLearningModules.map((module: ILearningModuleDetail) => ({ ...module }));
        let moduleIndex = allLearningModules.findIndex((module: ILearningModuleDetail) => module.id === moduleId);
        let learningModuleDetail = allLearningModules[moduleIndex];
        let moduleLikedByUser = learningModuleDetail.isLikedByUser;

        if (moduleLikedByUser) {
            let userDownVoteLearningModuleResponse = await userDownVoteLearningModule(learningModuleDetail.id);
            if (userDownVoteLearningModuleResponse.status === 200 && userDownVoteLearningModuleResponse.data) {
                learningModuleDetail.isLikedByUser = false;
                learningModuleDetail.voteCount = learningModuleDetail.voteCount ? learningModuleDetail.voteCount - 1 : learningModuleDetail.voteCount;
            }
        } else {
            let userUpVoteLearningModuleResponse = await userUpVoteLearningModule(learningModuleDetail.id);
            if (userUpVoteLearningModuleResponse.status === 200 && userUpVoteLearningModuleResponse.data) {
                learningModuleDetail.isLikedByUser = true;
                learningModuleDetail.voteCount = learningModuleDetail.voteCount! + 1;
            }
        }
        allLearningModules[moduleIndex] = learningModuleDetail;
        this.setState({ allLearningModules: allLearningModules })
    }


    /**
    * Invoked by Infinite scroll component when user scrolls down to fetch next set of projects.
    * @param {Number} page Page count for which next set of modules needs to be fetched.
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
                    this.getFilteredLearningModules(page);
                }, Resources.randomClicksTimeOut);
            }

            else {
                this.getAllLearningModule(page);
            }
        }
    }

    /**
    * Get learning modules data from API using search text entered by user.
    * @param {Number} page Page count for which next set of modules needs to be fetched.
    */
    private searchFilterPostUsingAPI = async (page: number) => {
        let isFilterOn: boolean = false;
        const searchText = this.state.searchText.trim();

        if (searchText.length) {
            isFilterOn = true;
            const searchRequestDetails: IFilterRequestModel = {
                searchText: searchText
            };
            let response = await getLearningModules(page, searchRequestDetails);

            if (response.status === 200 && response.data) {
                this.setState({
                    allLearningModules: response.data,
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
    * Delete learning module.
    * @param {String} learningModuleId learningModule unique identifier.
    */
    private handleDeleteClick = async (learningModuleId: string) => {
        let deleteLearningModuleResponse = await deleteLearningModule(learningModuleId);
        if (deleteLearningModuleResponse.status === 200) {

            let allFilteredModules = this.state.allLearningModules.filter((learningModule: ILearningModuleDetail) => {
                return learningModule.id !== learningModuleId;
            });

            this.setState({
                allLearningModules: allFilteredModules
            });
            this.showAlert(this.localize("postDeleteSuccess"), NotificationType.Success)
        } else {
            this.showAlert(this.localize("postDeleteError"), NotificationType.Failure)
        }
    }

    /**
    * Renders the component.
    */
    private renderLearningModuleTabContent() {

        // Cards component array to be rendered on grid.
        const cards = new Array<JSX.Element>();
        const tiles = this.state.allLearningModules.map((learningModule: ILearningModuleDetail) => (<LearningModuleTile index={learningModule.id}
            learningModuleDetails={learningModule}
            handleEditClick={this.handleEditClick}
            handlePreviewClick={this.handlePreviewClick}
            handleVoteClick={this.handleVoteClick}
            currentUserId={this.userAADObjectId!}
            userRole={this.state.userRole}
            handleDeleteClick={this.handleDeleteClick}
            handleAddToUserModuleClick={this.handleAddToUserModuleClick}
            isPrivateListTab={false}
        />));

        if (tiles.length > 0) {
            cards.push(
                <Grid columns={this.state.windowWidth > Resources.maxWidthForMobileView ? 3 : 1}
                    accessibility={gridBehavior}
                    className="tile-render"
                    content={tiles}>
                </Grid>);
        }

        let scrollViewStyle = { height: this.state.isFilterApplied === true ? "75vh" : "80vh" };
        return (
            <>
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
                                handleAddClick={this.handleAddNewLearningModuleClick}
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
                                allCreatedBy={this.state.allAddedBy}
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
                                        tiles.length ? cards : (this.state.hasMorePosts ?
                                            <></> : (this.state.isFilterApplied ? < FilterNoPostContentPage /> : <NoPostAddedPage handleAddNewResource={this.handleAddNewLearningModuleClick} isValidUser={(this.state.userRole.isAdmin || this.state.userRole.isTeacher)} />))
                                    }
                                </InfiniteScroll>
                            </div>
                        </div>
                    </div>
                </div>
            </>
        )
    }

    /**
    * Renders the component.
    */
    public render() {
        let contents = this.state.loading
            ? <p><Loader /></p>
            : this.renderLearningModuleTabContent();
        return (
            <div>
                {contents}
            </div>
        );
    }
}

export default withTranslation()(LearningModulePage)