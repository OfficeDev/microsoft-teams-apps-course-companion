// <copyright file="your-learning-resource-page.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from 'react';
import * as microsoftTeams from "@microsoft/teams-js";
import { Loader, EyeIcon, Text, Grid, gridBehavior } from '@fluentui/react-northstar'
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import CommandBar from "./tab-command-bar";
import { NotificationType, IResourceDetail, IUserRole } from "../../model/type";
import InfiniteScroll from 'react-infinite-scroller';
import Tile from "../discover-tab/tile"
import NotificationMessage from '../notification-message/notification-message';
import Resources from '../../constants/resources';
import { searchUserResources, createUserResource, deleteUserResource } from '../../api/user-resource-api';
import { getUserRole } from '../../api/member-validation-api';
import { deleteResource, getResource, userDownVoteResource, userUpVoteResource } from '../../api/resource-api';
import "../../styles/site.css";
import "../../styles/tile.css";

interface IYourLearningResourcePageState {
    windowWidth: number;
    alertMessage: string;
    alertType: NotificationType;
    showAlert: boolean;
    showNoPostPage: boolean;
    infiniteScrollParentKey: number;
    isPageInitialLoad: boolean;
    hasMorePosts: boolean;
    allResources: IResourceDetail[];
    loading: boolean,
    isCreatedByFilter: boolean;
    searchText: string;
    showPostLoader: boolean;
    userRoleDetails: IUserRole;
    clickedResourceId: string;
}

/**
* Resource sub tab component for your learning tab.
*/
class YourLearningResourcePage extends React.Component<WithTranslation, IYourLearningResourcePageState> {

    localize: TFunction;
    userAADObjectId?: string;
    botId: string = "";
    history: any

    constructor(props: any) {
        super(props);
        this.localize = this.props.t;
        this.state = {
            windowWidth: window.innerWidth,
            alertMessage: "",
            alertType: 0,
            showAlert: false,
            showNoPostPage: false,
            infiniteScrollParentKey: 0,
            isPageInitialLoad: true,
            hasMorePosts: true,
            allResources: [],
            loading: false,
            isCreatedByFilter: false,
            searchText: "",
            showPostLoader: false,
            userRoleDetails: {
                isAdmin: false,
                isTeacher: false
            },
            clickedResourceId: "",
        }

        this.history = props.history;
    }

    public async componentDidMount() {
        // Get user role details.
        this.getUserRoles();
        microsoftTeams.initialize();
        microsoftTeams.getContext((context: microsoftTeams.Context) => {
            this.userAADObjectId = context.userObjectId!

            // Get resources to be loaded for tab.
            this.getAllResource(0);
        });

        window.addEventListener("resize", this.setWindowWidth);
    }

    public componentWillUnmount() {
        window.removeEventListener('resize', this.setWindowWidth);
    }

    /**
    * Sets state for showing alert notification.
    * @param {string} content Notification message
    * @param {number} type Boolean value indicating 1- Success 2- Error
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
    * get window width real time
    */
    private setWindowWidth = () => {
        if (window.innerWidth !== this.state.windowWidth) {
            this.setState({ windowWidth: window.innerWidth });
        }
    };

    /**
    * Navigate to edit resource task module.
    * @param {String} resourceId resource identifier.
    */
    private handleEditClick = (resourceId: string) => {
        let appBaseUrl = window.location.origin;
        microsoftTeams.tasks.startTask({
            completionBotId: this.botId,
            title: this.localize('editContentTaskModuleHeaderText'),
            height: Resources.taskModuleHeight,
            width: Resources.taskModuleWidth,
            url: `${appBaseUrl}/resourcecontent?viewMode=1&resourceId=${resourceId}`,
            fallbackUrl: `${appBaseUrl}/resourcecontent?viewMode=1&resourceId=${resourceId}`,
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
    * handle error occurred during authentication
    * @param {string} error error message.
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
    * Navigate to preview resource content task module.
    * @param {string} resourceId resource unique identifier.
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
    * Get user role details.
    */
    private getUserRoles = async () => {
        const userRoleDetails = await getUserRole(this.handleAuthenticationFailure);
        if (userRoleDetails.status === 200 && userRoleDetails.data) {
            this.setState({ userRoleDetails: userRoleDetails.data });
        }
    }

    /**
    * Get all resources based on current page number. 
    * @param {number} page Current page number for getting data.
    */
    private getAllResource = async (page: number) => {
        const allResourcesResponse = await searchUserResources(page, { userObjectId: this.userAADObjectId, isSaved: this.state.isCreatedByFilter ? false : true, searchText: this.state.searchText })
        if (allResourcesResponse.status === 200 && allResourcesResponse.data) {
            let existingResources = [...this.state.allResources];
            allResourcesResponse.data.forEach((resource: IResourceDetail) => {
                existingResources.push(resource);
            });
            this.setState({
                allResources: existingResources,
                isPageInitialLoad: false,
                hasMorePosts: allResourcesResponse.data.length === Resources.recordsToLoad,
                showNoPostPage: existingResources.length === 0,
                showPostLoader: false
            });
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
    * Invoked by Infinite scroll component when user scrolls down to fetch next set of resources.
    * @param {number} page  Page number for which next set of resource needs to be fetched.
    */
    private loadMoreProjects = (page: number) => {
        this.getAllResource(page);
    }

    /**
    * Invoked by your learning created by toggle button change.
    */
    private handleCreatedByToggleButtonChange = () => {
        let isCreatedByFilter = this.state.isCreatedByFilter;
        this.setState({
            isCreatedByFilter: !isCreatedByFilter,
            allResources: [],
            showPostLoader: true
        },
            () => this.getAllResource(0));
    }

    /**
    * Add resource to user saved resource list.
    * @param {string} resourceId resource unique identifier.
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
    * Remove resource from user resource list.
    * @param {string} resourceId resource unique identifier.
    */
    private handleRemoveFromUserResourcesClick = async (resourceId: string) => {
        const deleteUserResourceResponse = await deleteUserResource(resourceId);
        if (deleteUserResourceResponse.status === 200 && deleteUserResourceResponse.data !== null) {
            this.showAlert(this.localize("resourceRemovedFromUserListSuccess"), NotificationType.Success);
            this.state.allResources!.map((resource: IResourceDetail, index: number) => {
                if (resource.id === resourceId) {
                    this.handleRemoveDeletedResourceFromList(index);
                }
            });
        }
        else {
            this.showAlert(this.localize("removeFromUserListResourceError"), NotificationType.Failure)
        }
    }

    /**
    * Remove resource tile from tab.
    * @param {number} index Resource tile index.
    */
    private handleRemoveDeletedResourceFromList = (index: number) => {
        let allResource = [...this.state.allResources];
        allResource.splice(index, 1);
        this.setState({ allResources: allResource });
    }

    /**
    * Delete resource.
    * @param {string} resourceId Rsource unique identifier.
    */
    private handleDeleteClick = async (resourceId: string) => {
        let deleteResourceResponse = await deleteResource(resourceId);
        if (deleteResourceResponse.status === 200) {
            this.showAlert(this.localize("resourceDeleteSuccess"), NotificationType.Success)
            this.state.allResources!.map((resource: IResourceDetail, index: number) => {
                if (resource.id === resourceId) {
                    this.handleRemoveDeletedResourceFromList(index);
                }
            });
        } else {
            this.showAlert(this.localize("unableToDeleteResource"), NotificationType.Failure)
        }
    }

    /**
    * Navigate to add to learning module task module.
    * @param {string} gradeId Grade unique identifier.
    * @param {string} subjectId Subject unique identifier.
    * @param {string} resourceId Resource unique identifier.
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
        if (err) {
            return "";
        }
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
    * Set state of search text as per user input change
    * @param  {string} searchText Search text entered by user.
    */
    private handleSearchInputChange = (searchText: string) => {
        this.setState({
            searchText: searchText
        });
    }

    /**
    * Handle search.
    */
    private handleSearchIconClick = async () => {
        var searchText = this.state.searchText;
        this.setState({
            searchText: searchText,
            allResources: [],
            showPostLoader: true,
        }, () => this.getAllResource(0));
    }

    /**
    * render your learning tab content.
    */
    private renderYourLearningTabContent() {

        // Cards component array to be rendered on grid.
        const cards = new Array<JSX.Element>();
        let tiles = new Array<JSX.Element>();
        let allResources = this.state.allResources.map((resource: IResourceDetail) => ({ ...resource }));

        allResources.forEach((resource: IResourceDetail) => {
            tiles.push(
                <Tile index={resource.id}
                    resourceDetails={resource}
                    handleEditClick={this.handleEditClick}
                    handlePreviewClick={this.handlePreviewClick}
                    handleVoteClick={this.handleVoteClick}
                    currentUserId={this.userAADObjectId!}
                    userRole={this.state.userRoleDetails}
                    handleAddToUserResourcesClick={this.handleAddToUserResourcesClick}
                    handleRemoveFromUserResourcesClick={this.handleRemoveFromUserResourcesClick}
                    handleDeleteClick={this.handleDeleteClick}
                    isCreatedByFilter={!this.state.isCreatedByFilter}
                    handleAddToLearningModuleClick={this.handleAddToLearningModuleClick}
                />
            )
        });

        if (tiles.length > 0) {
            cards.push(
                <Grid columns={this.state.windowWidth > Resources.maxWidthForMobileView ? Resources.threeColumnGrid : Resources.oneColumnGrid}
                    accessibility={gridBehavior}
                    className="tile-render"
                    content={tiles}>
                </Grid>);
        }

        let scrollViewStyle = { height: Resources.yourLearningHeight };
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
                            <CommandBar
                                handleSearchInputChange={this.handleSearchInputChange}
                                isValidUser={(this.state.userRoleDetails.isAdmin || this.state.userRoleDetails.isTeacher)}
                                handleCreatedByToggleButtonChange={this.handleCreatedByToggleButtonChange}
                                handleSearchIconClick={this.handleSearchIconClick}
                                windowWidth={this.state.windowWidth}
                            />
                            {
                                this.state.showPostLoader ?
                                    <div className="loader"><Loader /></div> :
                                    <div key={this.state.infiniteScrollParentKey} className="scroll-view scroll-view-mobile" style={scrollViewStyle}>
                                        <InfiniteScroll
                                            pageStart={0}
                                            loadMore={this.loadMoreProjects}
                                            hasMore={this.state.hasMorePosts}
                                            initialLoad={false}
                                            useWindow={false}
                                            loader={<div className="loader"><Loader /></div>}>
                                            {
                                                this.state.showNoPostPage ?
                                                    <div className="no-post-added-container">
                                                        <div className="app-logo">
                                                            <EyeIcon size="largest" />
                                                        </div>
                                                        <div className="no-data-preview">
                                                            <Text content={this.localize("noDataFoundNote")} />
                                                        </div>
                                                    </div>
                                                    :
                                                    cards
                                            }
                                        </InfiniteScroll>
                                    </div>}
                        </div>
                    </div>
                </div>
            </>
        )
    }

    /**
    *    Renders the component.
    */
    public render() {
        let contents = this.state.loading
            ? <p><Loader /></p>
            : this.renderYourLearningTabContent();
        return (
            <div>
                {contents}
            </div>
        );
    }
}

export default withTranslation()(YourLearningResourcePage)