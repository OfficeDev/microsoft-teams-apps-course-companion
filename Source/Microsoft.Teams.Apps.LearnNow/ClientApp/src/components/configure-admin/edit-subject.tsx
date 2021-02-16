// <copyright file="edit-subject.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Button, Flex, Text, Input, Loader, ChevronStartIcon } from "@fluentui/react-northstar";
import * as microsoftTeams from "@microsoft/teams-js";
import { getSubject, updateSubject } from "../../api/subject-api";
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import ErrorMessage from "../error-message";

import "../../styles/admin-configure-wrapper-page.css";

interface IEditSubjectState {
    loader: boolean;
    subjectName: string;
    isSubjectValuePresent: boolean;
    isSubmitLoading: boolean;
    isSubjectTitleExists: boolean;
    showErrorMessage: boolean;
}

/**
* Component for editing subject details.
*/
class EditSubject extends React.Component<WithTranslation, IEditSubjectState> {
    id: string | undefined;
    localize: TFunction;
    history: any


    constructor(props) {
        super(props);
        this.history = props.history;

        let search = this.history.location.search;
        let params = new URLSearchParams(search);
        this.id = params.get("id")?.toString();
        this.localize = this.props.t;
        this.state = {
            loader: true,
            subjectName: "",
            isSubjectValuePresent: true,
            isSubmitLoading: false,
            isSubjectTitleExists: false,
            showErrorMessage: false
        }
    }

    /**
    * Used to initialize Microsoft Teams sdk
    */
    public async componentDidMount() {
        microsoftTeams.initialize();
        microsoftTeams.getContext((context) => {
            this.getSubject();
        });
    }

    /**
    * Calls API to get subject details for provided subject id
    */
    private getSubject = async () => {
        let response = await getSubject(this.id!);
        if (response.status === 200 && response.data) {
            this.setState({
                subjectName: response.data.subjectName,
                loader: false
            });
        }
        else {
            this.setState({
                loader: false
            });
        }
    }

    /**
    * Set State value of subject text box input control
    * @param {Any} event Object which describes event occurred 
    */
    private onSubjectValueChange = (event: any) => {
        this.setState({ subjectName: event.target.value, isSubjectValuePresent: true, isSubjectTitleExists: false });
    }

    /**
    * Triggers when user clicks back button
    */
    private onBackButtonClick = () => {
        this.history.goBack();
    }

    /**
    * Submits and adds new user response
    */
    private onUpdateButtonClick = async () => {
        if (this.checkIfSubmitAllowed()) {
            this.setState({ showErrorMessage: false, isSubmitLoading: true });
            let details = { subjectName: this.state.subjectName, id: this.id };
            const response = await updateSubject(details, this.id!);
            if (response.status === 200) {
                this.history.goBack();
            } else if (response.status === 409) {
                this.setState({ isSubjectTitleExists: true, isSubmitLoading: false });
            } else {
                this.setState({ showErrorMessage: true, isSubmitLoading: false });
            }
        }
    }

    /**
    * Checks whether all validation conditions are matched before user submits update subject request
    */
    private checkIfSubmitAllowed = () => {
        if (this.state.subjectName) {
            return true;
        }
        else {
            this.setState({ isSubjectValuePresent: false });
            return false;
        }
    }

    /**
    *Returns text component containing error message when any generic error occurs.
    */
    private getGenericErrorMessage = () => {
        if (this.state.showErrorMessage) {
            return (<ErrorMessage errorMessage="generalErrorMessage" isGenericError={true} />);
        }
        return (<></>);
    }

    /**
    * Returns text component containing error message for failed subject field validations
    */
    private getErrorMessage = () => {
        if (!this.state.isSubjectValuePresent) {
            return (<ErrorMessage errorMessage="adminSubjectEmptyValidationMessage" isGenericError={false} />);
        } else if (this.state.isSubjectTitleExists) {
            return (<ErrorMessage errorMessage="subjectAlreadyExistsValidationMessage" isGenericError={false} />);
        }
        return (<></>);
    }

    /**
    * Renders the component
    */
    public render(): JSX.Element {
        return (
            <>
                {this.getWrapperPage()}
            </>
        );
    }

    /**
    * Get wrapper for page which acts as container for all child components
    */
    private getWrapperPage = () => {
        if (this.state.loader) {
            return (
                <div className="loader">
                    <Loader />
                </div>
            );
        } else {
            return (
                <div className="add-new-grade-page">
                    <div>
                        <Text content={this.localize("adminEditSubjectTitleLabelText")} size="medium" />
                    </div>
                    <div className="add-form-container">
                        <div>
                            <Flex gap="gap.small">
                                <Text content={this.localize("adminCreateSubjectLabelText")} size="medium" />
                            </Flex>
                            <Flex.Item push>
                                {this.getErrorMessage()}
                            </Flex.Item>
                        </div>
                        <div className="add-form-input">
                            <Input placeholder={this.localize("adminTextInputSubjectPlaceholder")} fluid required maxLength={200} value={this.state.subjectName} onChange={this.onSubjectValueChange} />
                        </div>
                    </div>
                    <div className="add-form-button-container">
                        <div>
                            <Flex space="between">
                                <Button icon={<ChevronStartIcon />} content={this.localize("adminBackButtonText")} text onClick={this.onBackButtonClick} />
                                <Flex gap="gap.small">
                                    <Button content={this.localize("adminUpdateButtonText")} primary loading={this.state.isSubmitLoading} disabled={this.state.isSubmitLoading} onClick={this.onUpdateButtonClick} />
                                </Flex>
                            </Flex>
                        </div>
                        <div>
                            {this.getGenericErrorMessage()}
                        </div>
                    </div>
                </div>
            )
        }
    }
}

export default withTranslation()(EditSubject);