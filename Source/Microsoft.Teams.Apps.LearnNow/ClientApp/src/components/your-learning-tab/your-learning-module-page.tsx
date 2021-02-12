// <copyright file="your-learning-module-page.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from 'react';
import * as microsoftTeams from "@microsoft/teams-js";
import { Loader, EyeIcon, Text, Grid, gridBehavior } from '@fluentui/react-northstar'
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from 'i18next';
import { ILearningModuleDetail, NotificationType, IUserRole, IModuleResourceViewModel } from "../../model/type";
import { deleteLearningModule, getLearningModule, userDownVoteLearningModule, userUpVoteLearningModule } from '../../api/learning-module-api';
import InfiniteScroll from 'react-infinite-scroller';
import LearningModuleTile from "../learning-module-tab/learning-module-tile"
import NotificationMessage from '../notification-message/notification-message';
import LearningModules from '../../constants/resources';
import { getUserRole } from '../../api/member-validation-api';
import CommandBar from "./tab-command-bar";
import { createUserLearningModule, searchUserLearningModules, deleteUserLearningModule } from '../../api/user-learning-module-api';
import Resources from '../../constants/resources';

import "../../styles/site.css";
import "../../styles/tile.css";

interface IYourLearningModulePageState {
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
    isCreatedByFilter: boolean;
    showPostLoader: boolean;
    userRole: IUserRole;
    clickedModuleId: string,
}

/**
* Learning module sub tab component for your learning tab.
*/
class YourLearningModulePage extends React.Component<WithTranslation, IYourLearningModulePageState> {

    localize: TFunction;
    userAADObjectId?: string;
    botId: string = "";
    history: any

    constructor(props: any) {
        super(props);
        this.localize = this.props.t;
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
            loading: false,
            isCreatedByFilter: false,
            showPostLoader: false,
            userRole: {
                isAdmin: false,
                isTeacher: false
            },
            clickedModuleId: "",
        }
    }

    public async componentDidMount() {

        // Get user role details.
        this.getUserRoles();

        microsoftTeams.initialize();
        microsoftTeams.getContext((context: microsoftTeams.Context) => {
            this.userAADObjectId = context.userObjectId!

            // Get learning modules to be loaded for tab.
            this.getAllLearningModule(0);
        });
        window.addEventListener("resize", this.setWindowWidth);
    }

    public componentWillUnmount() {
        window.removeEventListener('resize', this.setWindowWidth);
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
    * Navigate to edit module task module.
    * @param {String} learningModuleId learning module identifier
    */
    private handleEditClick = (learningModuleId: string) => {
        let appBaseUrl = window.location.origin;
        microsoftTeams.tasks.startTask({
            completionBotId: this.botId,
            title: this.localize('editContentTaskModuleHeaderText'),
            height: LearningModules.taskModuleHeight,
            width: LearningModules.taskModuleWidth,
            url: `${appBaseUrl}/createmodule?viewMode=1&resourceId=${learningModuleId}`,
            fallbackUrl: `${appBaseUrl}/createmodule?viewMode=1&resourceId=${learningModuleId}`,
        }, this.editLearningModuleSubmitHandler);
    }

    /**
    * handleAddToLearningModuleClick content task module handler.
    * @param {string | any} err  Task module submit handler error message.
    * @param {string | any} module  Task module submit handler result.
    */
    private editLearningModuleSubmitHandler = async (err: any, module: any) => {
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
    * Navigate to preview learningModule content task module.
    * @param {String} learningModuleId learning module identifier
    */
    private handlePreviewClick = (learningModuleId: string) => {
        let appBaseUrl = window.location.origin;
        this.setState({ clickedModuleId: learningModuleId });
        microsoftTeams.tasks.startTask({
            completionBotId: this.botId,
            title: this.localize('previewContentTaskModuleHeaderText'),
            height: LearningModules.taskModuleHeight,
            width: LearningModules.taskModuleWidth,
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
    * Get user role details.
    */
    private getUserRoles = async () => {
        const userRole = await getUserRole(this.handleAuthenticationFailure);
        if (userRole.status === 200 && userRole.data) {
            this.setState({ userRole: userRole.data });
        }
    }

    /**
    * Get modules based on current page number.
    * @param {number} page Current page number.
    */
    private getAllLearningModule = async (page: number) => {
        const allLearningModulesResponse = await searchUserLearningModules(page, { userObjectId: this.userAADObjectId, isSaved: this.state.isCreatedByFilter ? false : true, searchText: this.state.searchText });
        if (allLearningModulesResponse.status === 200 && allLearningModulesResponse.data) {
            let existingLearningModules = [...this.state.allLearningModules];
            allLearningModulesResponse.data.forEach((learningModule: ILearningModuleDetail) => {
                existingLearningModules.push(learningModule);
            });
            this.setState({
                allLearningModules: existingLearningModules,
                isPageInitialLoad: false,
                hasMorePosts: allLearningModulesResponse.data.length === LearningModules.recordsToLoad,
                showNoPostPage: existingLearningModules.length === 0,
                showPostLoader: false
            });
        }
    }

    /**
    * Invoked by your learning created by toggle button change.
    */
    private handleCreatedByToggleButtonChange = () => {
        let isCreatedByFilter = this.state.isCreatedByFilter;
        this.setState({
            isCreatedByFilter: !isCreatedByFilter,
            allLearningModules: [],
            showPostLoader: true
        },
            () => this.getAllLearningModule(0));
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
    * Invoked by Infinite scroll component when user scrolls down to fetch next set of modules.
    * @param {number} page Page number for which next set of modules needs to be fetched.
    */
    private loadMoreProjects = (page: number) => {
        this.getAllLearningModule(page);
    }

    /**
    * Add learning module to user saved module list.
    * @param {string} learningModuleId learningModule unique identifier.
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
    * Remove learningModule from user module list.
    * @param {string} learningModuleId learningModule unique identifier.
    */
    private handleRemoveFromUserModuleClick = async (learningModuleId: string) => {
        const deleteUserLearningModuleResponse = await deleteUserLearningModule(learningModuleId);
        if (deleteUserLearningModuleResponse.status === 200 && deleteUserLearningModuleResponse.data !== null) {
            this.showAlert(this.localize("moduleRemovedFromUserListSuccess"), NotificationType.Success);
            this.state.allLearningModules!.map((resource: ILearningModuleDetail, index) => {
                if (resource.id === learningModuleId) {
                    this.handleRemoveDeletedResourceFromList(index);
                }
            })
        }
        else {
            this.showAlert(this.localize("removeFromUserListModuleError"), NotificationType.Failure)
        }
    }

    /**
    * Remove module tile from tab.
    * @param {number} index Module tile index.
    */
    private handleRemoveDeletedResourceFromList = (index: number) => {
        let allLearningModule = [...this.state.allLearningModules];
        allLearningModule.splice(index, 1);
        this.setState({ allLearningModules: allLearningModule });
    }

    /**
    * Delete resource.
    * @param {string} learningModuleId Learning module unique identifier.
    */
    private handleDeleteClick = async (learningModuleId: string) => {
        let deleteResourceResponse = await deleteLearningModule(learningModuleId);
        if (deleteResourceResponse.status === 200) {

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
    * Set state of search text as per user input change.
    * @param {string} searchText Search text entered by user.
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
            allLearningModules: [],
            showPostLoader: true,
        }, () => this.getAllLearningModule(0));
    }

    /**
    * render your learning tab content.
    */
    private renderLearningModuleTabContent() {

        // Cards component array to be rendered on grid.
        const cards = new Array<JSX.Element>();
        const tiles = new Array<JSX.Element>();
        let allLearningModules = this.state.allLearningModules.map((module: ILearningModuleDetail) => ({ ...module }));
        allLearningModules.forEach((learningModule: ILearningModuleDetail) => {

            tiles.push(
                <LearningModuleTile index={learningModule.id}
                    learningModuleDetails={learningModule}
                    handleEditClick={this.handleEditClick}
                    handlePreviewClick={this.handlePreviewClick}
                    handleVoteClick={this.handleVoteClick}
                    currentUserId={this.userAADObjectId!}
                    userRole={this.state.userRole}
                    handleAddToUserModuleClick={this.handleAddToUserModuleClick}
                    handleRemoveFromUserModuleClick={this.handleRemoveFromUserModuleClick}
                    handleDeleteClick={this.handleDeleteClick}
                    isPrivateListTab={!this.state.isCreatedByFilter}
                />
            )
        });
        if (tiles.length > 0) {
            cards.push(
                <Grid columns={this.state.windowWidth > Resources.maxWidthForMobileView ? Resources.threeColumnGrid : Resources.oneColumnGrid}
                    accessibility={gridBehavior}
                    className="tile-render"
                    content={tiles}></Grid>);
        }

        let scrollViewStyle = { height: LearningModules.yourLearningHeight };
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
                                isValidUser={(this.state.userRole.isAdmin || this.state.userRole.isTeacher)}
                                handleCreatedByToggleButtonChange={this.handleCreatedByToggleButtonChange}
                                handleSearchIconClick={this.handleSearchIconClick}
                                windowWidth={this.state.windowWidth}
                            />
                            {

                                this.state.showPostLoader ?
                                    < div className="loader"><Loader /></div> :
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
                                    </div>
                            }
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
            : this.renderLearningModuleTabContent();
        return (
            <div>
                {contents}
            </div>
        );
    }
}

export default withTranslation()(YourLearningModulePage)