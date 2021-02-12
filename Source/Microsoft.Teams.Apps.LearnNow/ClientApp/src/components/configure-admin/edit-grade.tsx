// <copyright file="edit-grade.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Button, Flex, Text, Input, Loader, ChevronStartIcon } from "@fluentui/react-northstar";
import * as microsoftTeams from "@microsoft/teams-js";
import { getGrade, updateGrade } from "../../api/grade-api";
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import ErrorMessage from "../error-message";

import "../../styles/admin-configure-wrapper-page.css";

interface IEditGradeState {
    loader: boolean;
    gradeName: string;
    isGradeValuePresent: boolean;
    isSubmitLoading: boolean;
    isGradeTitleExists: boolean;
    errorMessage: boolean;
}

/**
* Component for editing grade details.
*/
class EditGrade extends React.Component<WithTranslation, IEditGradeState> {
    id: string | undefined;
    localize: TFunction;
    history: any

    constructor(props) {
        super(props);
        this.localize = this.props.t;
        this.history = props.history;

        let search = this.history.location.search;
        let params = new URLSearchParams(search);
        this.id = params.get("id")?.toString();
        this.state = {
            loader: true,
            gradeName: "",
            isGradeValuePresent: true,
            isSubmitLoading: false,
            isGradeTitleExists: false,
            errorMessage: false
        }
    }

    /**
    * Used to initialize Microsoft Teams sdk
    */
    public async componentDidMount() {
        microsoftTeams.initialize();
        microsoftTeams.getContext((context) => {
            this.getGrade();
        });
    }

    /**
    * Calls API to get grade details for provided grade id
    */
    private getGrade = async () => {
        let response = await getGrade(this.id!);
        if (response.status === 200 && response.data) {
            this.setState({
                gradeName: response.data.gradeName,
                loader: false
            });
        } else {
            this.setState({
                loader: false
            });
        }
    }

    /**
    * Set State value of grade text box input control
    * @param {Any} event Object which describes event occurred
    */
    private onGradeValueChange = (event: any) => {
        this.setState({ gradeName: event.target.value, isGradeValuePresent: true, isGradeTitleExists: false });
    }

    /**
    * Triggers when user clicks back button
    */
    private onBackButtonClick = () => {
        this.history.goBack();
    }

    /**
    * Submits and adds new grade data
    */
    private onUpdateButtonClick = async () => {
        if (this.checkIfSubmitAllowed()) {
            this.setState({ errorMessage: false, isSubmitLoading: true });
            let details = { gradeName: this.state.gradeName, id: this.id };
            const response = await updateGrade(details, this.id!);
            if (response.status === 200) {
                this.history.goBack();
            } else if (response.status === 409) {
                this.setState({ isGradeTitleExists: true, isSubmitLoading: false });
            } else {
                this.setState({ errorMessage: true, isSubmitLoading: false });
            }
        }
    }

    /**
    * Checks whether all validation conditions are matched before user submits update grade request
    */
    private checkIfSubmitAllowed = () => {
        if (this.state.gradeName) {
            return true;
        } else {
            this.setState({ isGradeValuePresent: false });
            return false;
        }
    }

    /**
    * Returns text component containing error message when any generic error occurs.
    */
    private getGenericErrorMessage = () => {
        if (this.state.errorMessage) {
            return (<ErrorMessage errorMessage="generalErrorMessage" isGenericError={true} />);
        }
        return (<></>);
    }

    /**
    * Returns text component containing error message for failed grade field validations
    */
    private getErrorMessage = () => {

        if (!this.state.isGradeValuePresent) {
            return (<ErrorMessage errorMessage="adminGradeEmptyValidationMessage" isGenericError={false} />);
        } else if (this.state.isGradeTitleExists) {
            return (<ErrorMessage errorMessage="gradeAlreadyExistsValidationMessage" isGenericError={false} />);
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
                        <Text content={this.localize("adminEditGradeTitleLabelText")} size="medium" />
                        <Flex.Item push>
                            {this.getErrorMessage()}
                        </Flex.Item>
                    </div>
                    <div className="add-form-container">
                        <div>
                            <Flex gap="gap.small">
                                <Text content={this.localize("adminCreateGradeLabelText")} size="medium" />
                            </Flex>
                        </div>
                        <div className="add-form-input">
                            <Input placeholder={this.localize("adminTextGradeInputPlaceholder")} fluid required maxLength={200} value={this.state.gradeName} onChange={this.onGradeValueChange} />
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

export default withTranslation()(EditGrade);