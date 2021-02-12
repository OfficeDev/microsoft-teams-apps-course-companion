// <copyright file="configure-preference.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Flex, Text, Loader, Dropdown } from '@fluentui/react-northstar'
import * as microsoftTeams from "@microsoft/teams-js";
import { getBaseUrl } from "../../configVariables";
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import { getAllGrades } from "../../api/grade-api";
import { createTabConfiguration, updateTabConfiguration } from "../../api/tab-configuration-api";
import { IGrade, ILearningModule, ISubject } from "../../model/type";
import { getAllSubjects } from "../../api/subject-api";
import { getLearningModuleForGradeAndSubject } from "../../api/learning-module-api";

import "../../styles/configure-tab-preference.css";

interface IConfigureTabPreference {
    grade: IGrade,
    subject: ISubject,
    learningModule: ILearningModule,
    isGradeValid: boolean,
    isSubjectValid: boolean,
    isLearningModuleValid: boolean,
    loading: boolean,
    allSubjects: ISubject[],
    allGrades: IGrade[],
    allLearningModules: ILearningModule[],
    error: string,
    showMessage: boolean,
    disableSubmitButton: boolean,
    message?: string | null,
}

/**
* Tab configuration page component used for setting up teams tab page.
*/
class ConfigureTabPreference extends React.Component<WithTranslation, IConfigureTabPreference> {
    localize: TFunction;
    groupId: string;
    teamId: string;
    channelId: string;
    tabId: string;
    history: any
    constructor(props: any) {
        super(props);
        this.history = props.history;
        this.localize = this.props.t;
        this.groupId = "";
        this.teamId = "";
        this.channelId = "";
        this.tabId = "";
        this.state = {
            grade: {} as IGrade,
            subject: {} as ISubject,
            isGradeValid: true,
            isSubjectValid: true,
            isLearningModuleValid: true,
            learningModule: {} as ILearningModule,
            loading: true,
            allSubjects: [],
            allGrades: [],
            allLearningModules: [],
            error: "",
            disableSubmitButton: false,
            showMessage: false,
            message: null,
        }
    }

    /**
    * Used to initialize Microsoft Teams SDK
    */
    public componentDidMount() {
        microsoftTeams.initialize();
        microsoftTeams.getContext(async (context: microsoftTeams.Context) => {
            this.groupId = context.groupId!;
            this.teamId = context.teamId!;
            this.channelId = context.channelId!;
            this.tabId = context.entityId!;
        });

        this.getAllSubjectsAndGrades();

        microsoftTeams.settings.registerOnSaveHandler(async (saveEvent: microsoftTeams.settings.SaveEvent) => {

            if (this.state.grade.id === null) {
                this.setState({ isGradeValid: false })
                saveEvent.notifyFailure();
                return;
            }
            else if (this.state.subject.id === null) {
                this.setState({ isSubjectValid: false })
                saveEvent.notifyFailure();
                return;
            }

            if (this.state.learningModule.id === null || this.state.learningModule.id === undefined) {
                this.setState({
                    error: this.localize("learningModuleError"),
                    isLearningModuleValid: false,
                })
                saveEvent.notifyFailure();
                return;
            }

            let configureDetail = {
                learningModuleId: this.state.learningModule.id,
                teamId: this.teamId,
                channelId: this.channelId,
                id: this.tabId ? this.tabId : undefined,
            };

            let response = this.tabId ? await updateTabConfiguration(configureDetail, this.tabId!, this.groupId) : await createTabConfiguration(configureDetail, this.groupId);

            if (response.status === 200 && response.data) {
                microsoftTeams.settings.setSettings({
                    entityId: response.data.id!,
                    contentUrl: getBaseUrl() + "/teams-tab",
                    suggestedDisplayName: this.state.learningModule.title,
                });
                saveEvent.notifySuccess();
            } else {
                this.setState({
                    error: this.localize("UnableToSavePreference"),
                })
            }
        });
    }

    /**
    * Get all grades and subjects.
    */
    private getAllSubjectsAndGrades = async () => {

        // Fetch grades.
        await this.getGrades();

        // Fetch subjects
        await this.getSubjects();

        this.setState({ loading: false })
    }

    /**
    * Fetch all subject list.
    */
    private getSubjects = async () => {
        this.setState({ loading: true });
        const subjectResponse = await getAllSubjects(this.handleAuthenticationFailure);
        if (subjectResponse.status === 200 && subjectResponse.data) {
            this.setState({ allSubjects: subjectResponse.data });
        } else {
            this.setState({ showMessage: true, message: this.localize("defaultErrorMessage") });
        }
    }

    /**
    * Fetch all grade list.
    */
    private getGrades = async () => {
        this.setState({ loading: true });
        const gradeResponse = await getAllGrades(this.handleAuthenticationFailure);
        if (gradeResponse.status === 200 && gradeResponse.data) {
            this.setState({ allGrades: gradeResponse.data });
        } else {
            this.setState({ showMessage: true, message: this.localize("defaultErrorMessage") });
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
    * Fetch Learning module based on selected grade and subject
    */
    private getLearningModules = async () => {
        if (this.state.grade.id == null) {
            return;
        }
        else if (this.state.subject.id == null) {
            return;
        }
        this.setState({ allLearningModules: [], learningModule: {} as ILearningModule });
        const learningModuleResponse = await getLearningModuleForGradeAndSubject(this.state.grade.id!, this.state.subject.id!);
        if (learningModuleResponse.status === 200 && learningModuleResponse.data.length) {
            this.setState({ allLearningModules: learningModuleResponse.data, showMessage: false });
        } else if (learningModuleResponse.status === 200 && learningModuleResponse.data.length === 0) {
            this.setState({ showMessage: true, message: this.localize("noLearningModuleFoundError") });
        } else {
            this.setState({ showMessage: true, message: this.localize("defaultErrorMessage") });
        }
    }

    /**
    * Returns text component containing error message for failed field validation
    * @param {Boolean} isValid Indicates whether field value is valid
    */
    private getRequiredFieldError = (isValid: boolean) => {
        if (!isValid) {
            var errorMsg = this.localize("fieldRequiredError");
            return (<Text content={errorMsg} error size="medium" />);
        }
        return (<></>);
    }

    /**
    * Show validation error message.
    */
    private showError() {
        if (this.state.showMessage) {
            return (
                <Text error content={this.state.message} />
            );
        }
    }

    /**
    * Handle grade change event.
    * @param {any} event event object.
    * @param {any} dropdownProps props received on dropdown value click
    */
    private handleGradeChange = (event: any, dropdownProps?: any) => {
        let gradeProp = dropdownProps.value;
        if (gradeProp) {
            let grade: IGrade = {
                id: gradeProp.key,
                gradeName: gradeProp.header
            };

            this.setState({ grade: grade, isGradeValid: true }, this.getLearningModules);
        }
    }

    /**
    * Handle subject change event.
    * @param {Any} event event object.
    * @param {string} dropdownProps props received on dropdown value click
    */
    private handleSubjectChange = (event: any, dropdownProps?: any) => {
        let subjectProp = dropdownProps.value;
        if (subjectProp) {
            let subject: ISubject = {
                id: subjectProp.key,
                subjectName: subjectProp.header,
            }

            this.setState({ subject: subject, isSubjectValid: true }, this.getLearningModules);
        }
    }

    /**
    * Handle subject change event.
    * @param {Any} event event object.
    * @param {string} dropdownProps props received on dropdown value click
    */
    private handleLearningModuleChange = (event: any, dropdownProps?: any) => {
        let moduleProp = dropdownProps.value;
        if (moduleProp) {
            let learningModule: ILearningModule = {
                id: moduleProp.key,
                title: moduleProp.header
            }

            this.setState({ learningModule: learningModule, isLearningModuleValid: true });
            microsoftTeams.settings.setValidityState(true);
        }
    }

    /**
    * Render component.
    */
    private renderTabConfigurationContent() {
        return (
            <div>
                <div className="container-style">
                    <Flex.Item size="size.half">
                        <Flex className="grade-label">
                            <Text size="small" content={"*" + this.localize("gradeLabel")} />
                            <Flex.Item push>
                                {this.getRequiredFieldError(this.state.isGradeValid)}
                            </Flex.Item>
                        </Flex>
                    </Flex.Item>
                    <Flex.Item size="size.half">
                        <Dropdown
                            search
                            items={this.state.allGrades.map((grade: IGrade) => ({ key: grade.id, header: grade.gradeName }))}
                            defaultSearchQuery={this.state.grade ? this.state.grade.gradeName : ""}
                            placeholder={this.localize('gradePlaceHolderText')}
                            noResultsMessage={this.localize("noGradeFoundError")}
                            toggleIndicator={{ styles: { display: 'none' } }}
                            onChange={this.handleGradeChange}
                            className="configure-dropdown-box"
                        />
                    </Flex.Item>
                    <Flex.Item>
                        <Flex className="subject-label">
                            <Text size="small" content={"*" + this.localize("subjectLabel")} />
                            <Flex.Item push>
                                {this.getRequiredFieldError(this.state.isSubjectValid)}
                            </Flex.Item>
                        </Flex>
                    </Flex.Item>
                    <Flex.Item className="config-subject-input">
                        <Dropdown
                            search
                            items={this.state.allSubjects.map((subject: ISubject) => ({ key: subject.id, header: subject.subjectName }))}
                            defaultSearchQuery={this.state.subject ? this.state.subject.subjectName : ""}
                            placeholder={this.localize('subjectPlaceHolderText')}
                            noResultsMessage={this.localize("noSubjectFoundError")}
                            onChange={this.handleSubjectChange}
                            toggleIndicator={{ styles: { display: 'none' } }}
                            className="configure-dropdown-box"
                        />
                    </Flex.Item>
                    <Flex.Item size="size.large">
                        <Flex className="learning-module-label">
                            <Text size="small" content={"*" + this.localize("learningModuleLabel")} />
                            <Flex.Item push>
                                {this.getRequiredFieldError(this.state.isLearningModuleValid)}
                            </Flex.Item>
                        </Flex>
                    </Flex.Item>
                    <Flex.Item size="size.large">
                        <Dropdown
                            search
                            fluid
                            items={this.state.allLearningModules.map((module: ILearningModule) => { return { key: module.id, header: module.title } })}
                            defaultSearchQuery={this.state.learningModule.title ? this.state.learningModule.title : ""}
                            placeholder={this.localize('learningModulePlaceHolderText')}
                            noResultsMessage={this.localize("noLearningModuleFoundError")}
                            toggleIndicator={{ styles: { display: 'none' } }}
                            onChange={this.handleLearningModuleChange}
                            className="configure-dropdown-box"
                        />
                    </Flex.Item>
                    <div className="footer">
                        <Flex gap="gap.small">
                            <Flex.Item grow>
                                {this.showError()}
                            </Flex.Item>
                        </Flex>
                    </div>
                </div>
            </div>
        );
    }

    /**
    *    Renders the component.
    */
    public render() {
        let contents = this.state.loading
            ? <p><em><Loader /></em></p>
            : this.renderTabConfigurationContent();
        return (
            <div>
                {contents}
            </div>
        );
    }
}
export default withTranslation()(ConfigureTabPreference);