// <copyright file="add-tag.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Button, Flex, Text, Input, ChevronStartIcon } from "@fluentui/react-northstar";
import { createTag } from "../../api/tag-api";
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import Resources from "../../constants/resources";
import ErrorMessage from "../error-message";

import "../../styles/admin-configure-wrapper-page.css";

interface IAddTagState {
    tag: string;
    isTagValuePresent: boolean;
    isSubmitLoading: boolean;
    isTagTitleExists: boolean;
    showErrorMessage: boolean;
}

/**
* This Component is used in messaging extension action task module for adding new tag.
*/
class AddTag extends React.Component<WithTranslation, IAddTagState> {
    localize: TFunction;
    history: any;

    constructor(props: any) {
        super(props);
        this.localize = this.props.t;
        this.state = {
            tag: "",
            isTagValuePresent: true,
            isSubmitLoading: false,
            isTagTitleExists: false,
            showErrorMessage: false
        };
        this.history = props.history;
    }

    /**
    *Checks whether all validation conditions are matched before user submits new tag request
    */
    private checkIfSubmitAllowed = () => {
        if (this.state.tag) {
            return true;
        }
        else {
            this.setState({ isTagValuePresent: false });
            return false;
        }
    }

    /**
    * Set State value of tag text box input control
    * @param {Any} event Object which describes event occurred
    */
    private onTagValueChange = (event: any) => {
        this.setState({ tag: event.target.value, isTagValuePresent: true, isTagTitleExists: false });
    };

    /**
    * Triggers when user clicks back button
    */
    private onBackButtonClick = () => {
        this.history.goBack();
    };

    /**
    * Submits and adds new tag
    */
    private onAddButtonClick = async () => {
        if (this.checkIfSubmitAllowed()) {
            this.setState({ showErrorMessage: false, isSubmitLoading: true });
            let details = { tagName: this.state.tag };
            let postTagResponse = await createTag(details);
            if (postTagResponse.status === 200 && postTagResponse.data) {
                this.history.goBack();
            } else if (postTagResponse.status === 409) {
                this.setState({ isTagTitleExists: true, isSubmitLoading: false });
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
                    content={this.localize("adminCreateTagTitleLabelText")}
                    size="medium"
                />
                <div className="add-form-container">
                    <Flex gap="gap.small">
                        <Text
                            content={this.localize("adminCreateTagLabelText")}
                            size="medium"
                        />
                        <Flex.Item push>
                            {this.getErrorMessage()}
                        </Flex.Item>
                    </Flex>
                    <div className="add-form-input">
                        <Input
                            placeholder={this.localize("adminTextInputTagPlaceholder")}
                            fluid
                            required
                            maxLength={Resources.tagInputMaxLength}
                            value={this.state.tag}
                            onChange={this.onTagValueChange}
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
    * Returns text component containing error message for failed tag field validations.
    */
    private getErrorMessage = () => {
        if (!this.state.isTagValuePresent) {
            return (<ErrorMessage errorMessage="adminTagEmptyValidationMessage" isGenericError={false} />);
        }
        else if (this.state.isTagTitleExists) {
            return (<ErrorMessage errorMessage="tagAlreadyExistsValidationMessage" isGenericError={false} />);
        }
        return (<></>);
    }
}

export default withTranslation()(AddTag);
