// <copyright file="edit-tag.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Button, Flex, Text, Input, Loader, ChevronStartIcon } from "@fluentui/react-northstar";
import * as microsoftTeams from "@microsoft/teams-js";
import { getTag, updateTag } from "../../api/tag-api";
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import ErrorMessage from "../error-message";

import "../../styles/admin-configure-wrapper-page.css";

interface IEditTagState {
    loader: boolean;
    tagName: string;
    isTagValuePresent: boolean;
    isSubmitLoading: boolean;
    isTagTitleExists: boolean;
    showErrorMessage: boolean;
}

/**
* Component for editing tag details.
*/
class EditTag extends React.Component<WithTranslation, IEditTagState> {
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
            tagName: "",
            isTagValuePresent: true,
            loader: true,
            isSubmitLoading: false,
            isTagTitleExists: false,
            showErrorMessage: false
        }
    }

    /**
    * Used to initialize Microsoft Teams sdk
    */
    public async componentDidMount() {
        microsoftTeams.initialize();
        microsoftTeams.getContext((context) => {
            this.getTag();
        });
    }

    /**
    * Calls API to get tag details for provided tag id
    */
    private getTag = async () => {
        let response = await getTag(this.id!);
        if (response.status === 200 && response.data) {
            this.setState({
                tagName: response.data.tagName,
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
    * Set State value of category text box input control
    * @param {Any} event Object which describes occurred event
    */
    private onTagValueChange = (event: any) => {
        this.setState({ tagName: event.target.value, isTagValuePresent: true, isTagTitleExists: false });
    }

    /**
    * Triggers when user clicks back button
    */
    private onBackButtonClick = () => {
        this.history.goBack();
    }

    /**
    *Submits and adds new user response
    */
    private onUpdateButtonClick = async () => {
        if (this.checkIfSubmitAllowed()) {
            this.setState({ showErrorMessage: false, isSubmitLoading: true });
            let details = { tagName: this.state.tagName, id: this.id };
            let response = await updateTag(details, this.id);
            if (response.status === 200) {
                this.history.goBack();
            } else if (response.status === 409) {
                this.setState({ isTagTitleExists: true, isSubmitLoading: false });
            } else {
                this.setState({ showErrorMessage: true, isSubmitLoading: false });
            }
        }
    }

    /**
    * Checks whether all validation conditions are matched before user submits update tag request
    */
    private checkIfSubmitAllowed = () => {
        if (this.state.tagName) {
            return true;
        }
        else {
            this.setState({ isTagValuePresent: false });
            return false;
        }
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
    *Get wrapper for page which acts as container for all child components
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
                        <Text content={this.localize("adminEditTagTitleLabelText")} size="medium" />
                    </div>
                    <div className="add-form-container">
                        <div>
                            <Flex gap="gap.small">
                                <Text content={this.localize("adminCreateTagLabelText")} size="medium" />
                                <Flex.Item push>
                                    {this.getErrorMessage()}
                                </Flex.Item>
                            </Flex>
                        </div>
                        <div className="add-form-input">
                            <Input placeholder={this.localize("adminTextInputTagPlaceholder")} fluid required maxLength={200} value={this.state.tagName} onChange={this.onTagValueChange} />
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

export default withTranslation()(EditTag);