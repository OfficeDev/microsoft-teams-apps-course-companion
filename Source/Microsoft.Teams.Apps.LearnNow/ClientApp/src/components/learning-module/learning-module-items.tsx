// <copyright file="learning-module-items.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Button, Flex, Input, Loader } from "@fluentui/react-northstar";
import * as microsoftTeams from "@microsoft/teams-js";
import { SearchIcon } from "@fluentui/react-icons-northstar";
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import LearningModuleTable from "./learning-module-table";
import { createResourceModuleMapping, getLearningModules } from "../../api/learning-module-api";
import { IFilterModel, ILearningModuleItem, IResourceModuleDetails, IUserRole } from "../../model/type";
import Resources from "../../constants/resources";
import { getUserRole } from "../../api/member-validation-api";

import "../../styles/learning-module.css";

interface ILearningModuleItemState {
    isLoading: boolean;
    userSelectedItem: string;
    filteredItem: ILearningModuleItem[];
    learningModuleData: ILearningModuleItem[];
    searchValue: string;
    isSubmitLoading: boolean;
    windowWidth: number;
    userRole: IUserRole;
}

/**
 * Component to render learning module collection in add to learning module page.
 */
class LearningModuleItems extends React.Component<
    WithTranslation,
    ILearningModuleItemState
    > {
    localize: TFunction;
    userAADObjectId?: string | null = null;
    history: any;
    gradeId: string | null = null;
    subjectId: string | null = null;
    resourceId: string | null = null;

    constructor(props: any) {
        super(props);
        this.localize = this.props.t;
        this.state = {
            isLoading: true,
            userSelectedItem: "",
            searchValue: "",
            filteredItem: [],
            learningModuleData: [],
            isSubmitLoading: false,
            windowWidth: window.innerWidth,
            userRole: {
                isAdmin: false,
                isTeacher: false
            },
        };
        this.history = props.history;
        let search = this.history.location.search;
        let params = new URLSearchParams(search);
        this.gradeId = params.get("gradeId") ? params.get("gradeId") : "";
        this.subjectId = params.get("subjectId") ? params.get("subjectId") : "";
        this.resourceId = params.get("resourceId") ? params.get("resourceId") : "";
        this.handleKeyPress = this.handleKeyPress.bind(this);
    }

    /**
     * Used to initialize Microsoft Teams sdk
     */
    public async componentDidMount() {
        this.setState({ isLoading: true });
        microsoftTeams.initialize();
        microsoftTeams.getContext((context) => {
            this.userAADObjectId = context.userObjectId!
        });
        await this.getUserRoles();
        this.getAllLearningModules();
        window.addEventListener("resize", this.setWindowWidth);
    }

    public componentWillUnmount() {
        window.removeEventListener('resize', this.setWindowWidth);
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
    * Get window width real time
    */
    private setWindowWidth = () => {
        if (window.innerWidth !== this.state.windowWidth) {
            this.setState({ windowWidth: window.innerWidth });
        }
    };

    /**
     * Fetch posts for user private list tab from API
     */
    private getAllLearningModules = async () => {
        var filterRequestDetails: IFilterModel = {
            subjectIds: [this.subjectId!],
            gradeIds: [this.gradeId!],
            tagIds: [],
            createdByObjectIds: this.state.userRole.isTeacher ? [this.userAADObjectId!] : [],
            searchText: this.state.searchValue
        };

        let response = await getLearningModules(0, filterRequestDetails);
        if (response.status === 200 && response.data) {
            // bind all learning modules data
            this.setState({
                filteredItem: response.data,
                learningModuleData: response.data,
                isLoading: false,
            });
        } else {
            this.setState({
                isLoading: false,
            });
        }
    };

    /**
     * Set State value of text box input control
     * @param  {Any} event Event object
     */
    private handleChange = (event: any) => {
        this.setState({ searchValue: event.target.value });
    };

    /**
     * Used to call parent search method on enter key press in text box
     * @param  {Any} event Event object
     */
    private handleKeyPress = (event: any) => {
        var keyCode = event.which || event.keyCode;
        if (keyCode === Resources.keyCodeEnter) {
            this.getAllLearningModules();
        }
    };

    /**
     *Submits and adds new learning module resource.
     */
    private onAddButtonClick = async () => {
        if (this.checkIfSubmitAllowed()) {
            this.setState({ isSubmitLoading: true });
            var resourceModuleData: IResourceModuleDetails = {
                ResourceId: this.resourceId!,
                LearningModuleId: this.state.userSelectedItem,
            };

            let response = await createResourceModuleMapping(
                resourceModuleData
            );
            if (response.status === 200) {
                let details: any = { isSuccess: true };
                microsoftTeams.tasks.submitTask(details);
            } else if (response.status === 409) {
                let details: any = { isDuplicate: true };
                microsoftTeams.tasks.submitTask(details);
            }
            this.setState({ isSubmitLoading: true });
        }
    };

    /**
     *Checks whether all validation conditions are matched before user submits new grade request
     */
    private checkIfSubmitAllowed = () => {
        return this.state.userSelectedItem.length > 0;

    };

    /**
     * Navigate to add new module page
     */
    private handleCreateNewButtonClick = () => {
        this.history.push(
            `/createmodule?addresource=${true}&resourceId=${this.resourceId}`
        );
    };

    /**
     * Update selected learning modules data
     * @param {String} moduleId Learning module id
     * @param {Boolean} isSelected Represents whether module is selected or not
     */
    private onLearningModuleSelected = (moduleId: string, isSelected: boolean) => {
        let filteredItems: ILearningModuleItem[] = [];
        if (isSelected) {
            this.state.filteredItem!.map((resource: ILearningModuleItem) => {
                if (resource.id === moduleId) {
                    resource.isItemChecked = true;
                } else {
                    resource.isItemChecked = false;
                }
                filteredItems.push(resource);
            });
            this.setState({
                userSelectedItem: moduleId,
                filteredItem: filteredItems,
            });
        } else {
            this.state.filteredItem!.map((resource: ILearningModuleItem) => {
                if (resource.id === moduleId) {
                    resource.isItemChecked = false;
                }
                filteredItems.push(resource);
            });

            this.setState({
                userSelectedItem: "",
                filteredItem: filteredItems,
            });
        }
    };

    /**
     * Renders the component
     */
    public render(): JSX.Element {

        if (this.state.isLoading) {
            return (
                <div className="container-div">
                    <div className="container-subdiv">
                        <div className="loader">
                            <Loader />
                        </div>
                    </div>
                </div>
            );
        } else {
            return (
                <div>
                    <div className="add-lm-module-container">
                        <div className="search-container-add-lm">
                            <Flex gap="gap.small">
                                <Flex.Item>
                                    <Flex className="add-lm-search">
                                        <Input
                                            fluid
                                            icon={
                                                <SearchIcon
                                                    onClick={this.getAllLearningModules}
                                                />
                                            }
                                            placeholder={this.localize("searchModulePlaceHolder")}
                                            value={this.state.searchValue}
                                            onChange={this.handleChange}
                                            onKeyUp={this.handleKeyPress}
                                        />
                                    </Flex>
                                </Flex.Item>
                                <Flex.Item>
                                    <Flex column gap="gap.small" vAlign="start">
                                        <Button
                                            className="create-new-button"
                                            content={this.localize(
                                                "createNewLearningModuleButtonText"
                                            )}
                                            onClick={() => {
                                                this.handleCreateNewButtonClick();
                                            }}
                                            disabled={this.state.userSelectedItem.length > 0}
                                        />
                                    </Flex>
                                </Flex.Item>
                            </Flex>
                        </div>
                        <div className="learning-module-table-container">
                            {this.state.filteredItem.length > 0 ?
                                <LearningModuleTable
                                    showCheckbox={true}
                                    learningModuleItems={this.state.filteredItem}
                                    onCheckBoxChecked={this.onLearningModuleSelected}
                                    windowWidth={this.state.windowWidth}
                                />
                                :
                                <div className="resource-validation">{this.localize("noModuleForResource")}</div>
                            }
                        </div>
                    </div>
                    <div className="add-form-button">
                        <Button
                            content={this.localize("doneButtonText")}
                            primary
                            loading={this.state.isSubmitLoading}
                            disabled={
                                this.state.isSubmitLoading ||
                                this.state.userSelectedItem.length <= 0
                            }
                            onClick={this.onAddButtonClick}
                        />
                    </div>
                </div>
            );
        }
    }
}

export default withTranslation()(LearningModuleItems);
