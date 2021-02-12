// <copyright file="add-subject.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Button, Flex, Text, Input, ChevronStartIcon } from "@fluentui/react-northstar";
import { createSubject } from "../../api/subject-api";
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import Resources from "../../constants/resources";
import ErrorMessage from "../error-message";

import "../../styles/admin-configure-wrapper-page.css";

interface IAddSubjectState {
    subject: string;
    isSubjectValuePresent: boolean;
    isSubmitLoading: boolean;
    isSubjectTitleExists: boolean;
    showErrorMessage: boolean;
}

/**
 * This Component is used in messaging extension action task module for adding new subject.
 */
class AddSubject extends React.Component<WithTranslation, IAddSubjectState> {
    localize: TFunction;
    history: any;

    constructor(props: any) {
        super(props);
        this.localize = this.props.t;
        this.state = {
            subject: "",
            isSubjectValuePresent: true,
            isSubmitLoading: false,
            isSubjectTitleExists: false,
            showErrorMessage: false
        };
        this.history = props.history;
    }

    /**
    * Checks whether all validation conditions are matched before user submits new subject request
    */
    private checkIfSubmitAllowed = () => {
        if (this.state.subject) {
            return true;
        } else {
            this.setState({ isSubjectValuePresent: false });
            return false;
        }
    }

    /**
    * Set State value of subject text box input control
    * @param {Any} event Object which describes event occurred
    */
    private onSubjectValueChange = (event: any) => {
        this.setState({ subject: event.target.value, isSubjectValuePresent: true, isSubjectTitleExists: false });
    };

    /**
    * Triggers when user clicks back button
    */
    private onBackButtonClick = () => {
        this.history.goBack();
    };

    /**
    * Submits and adds new subject
    */
    private onAddButtonClick = async () => {
        if (this.checkIfSubmitAllowed()) {
            this.setState({ showErrorMessage: false, isSubmitLoading: true });
            let details = { subjectName: this.state.subject };
            const postSubjectResponse = await createSubject(details);
            if (postSubjectResponse.status === 200 && postSubjectResponse.data) {
                this.history.goBack();
            } else if (postSubjectResponse.status === 409) {
                this.setState({ isSubjectTitleExists: true, isSubmitLoading: false });
            } else {
                this.setState({ showErrorMessage: true, isSubmitLoading: false });
            }
        }
    };

    /**
    * Renders the component
    */
    public render(): JSX.Element {
        return (
            <div className="add-new-grade-page">
                <Text
                    content={this.localize("adminCreateSubjectTitleLabelText")}
                    size="medium"
                />
                <div className="add-form-container">
                    <Flex gap="gap.small">
                        <Text
                            content={this.localize("adminCreateSubjectLabelText")}
                            size="medium"
                        />
                        <Flex.Item push>
                            {this.getErrorMessage()}
                        </Flex.Item>
                    </Flex>
                    <div className="add-form-input">
                        <Input
                            placeholder={this.localize("adminTextInputSubjectPlaceholder")}
                            fluid
                            required
                            maxLength={Resources.subjectInputMaxLength}
                            value={this.state.subject}
                            onChange={this.onSubjectValueChange}
                        />
                    </div>
                </div>
                <div className="add-form-button-container">
                    <Flex space="between">
                        <Button
                            icon={<ChevronStartIcon />}
                            content={this.localize("adminBackButtonText")}
                            text
                            onClick={this.onBackButtonClick}
                        />
                        <Flex gap="gap.small">
                            <Button
                                content={this.localize("adminAddButtonText")}
                                primary
                                loading={this.state.isSubmitLoading}
                                disabled={this.state.isSubmitLoading}
                                onClick={this.onAddButtonClick}
                            />
                        </Flex>
                    </Flex>
                    {this.getGenericErrorMessage()}
                </div>
            </div>
        );
    }

    /**
    * Returns text component containing error message when any generic error occurs.
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
}

export default withTranslation()(AddSubject);
