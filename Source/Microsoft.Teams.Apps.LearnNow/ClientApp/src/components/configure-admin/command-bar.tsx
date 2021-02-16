// <copyright file="command-bar.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Button, Flex, Input, Dialog, TrashCanIcon, AddIcon, EditIcon, SearchIcon } from "@fluentui/react-northstar";
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import Resources from '../../constants/resources';

import "../../styles/admin-configure-wrapper-page.css";
import { ResourcesKeyCodes } from "../../constants/resources";

interface ICommandBarProps extends WithTranslation {
    isEditEnable: boolean;
    isDeleteEnable: boolean;
    onAddButtonClick: () => void;
    onEditButtonClick: () => void;
    onDeleteButtonClick: () => void;
    handleTableFilter: (searchText: string) => void;
}

interface ICommandbarState {
    searchValue: string,
    windowWidth: number
}

/**
* Component for showing command bar menu and search input.
*/
class CommandBar extends React.Component<ICommandBarProps, ICommandbarState> {
    localize: TFunction;
    constructor(props: ICommandBarProps) {
        super(props);
        this.localize = this.props.t;
        this.state = {
            searchValue: "",
            windowWidth: window.innerWidth
        };
        this.handleChange = this.handleChange.bind(this);
        this.handleKeyPress = this.handleKeyPress.bind(this);
    }

    /**
    * Set State value of text box input control
    * @param  {Any} event Event object
    */
    private handleChange(event: any) {
        this.setState({ searchValue: event.target.value });
        if (event.target.value === "" || event.target.value.length > 2) {
            this.props.handleTableFilter(event.target.value);
        }
    }

    /**
    * Used to call parent search method on enter key press in text box
    * @param  {Any} event Event object
    */
    private handleKeyPress(event: any) {
        var keyCode = event.which || event.keyCode;
        if (keyCode === ResourcesKeyCodes.keyCodeEnter) {
            if (event.target.value === "" || event.target.value.length > 2) {
                this.props.handleTableFilter(event.target.value);
            }
        }
    }

    /**
    * Renders the component
    */
    public render(): JSX.Element {
        return (
            <Flex gap="gap.small" className="commandbar-wrapper">
                <Button icon={<AddIcon />} content={this.state.windowWidth > Resources.maxWidthForMobileView ? this.localize("commandBarAddNewLabel") : ""} text className="add-new-button" onClick={this.props.onAddButtonClick} />
                <Button icon={<EditIcon />} content={this.state.windowWidth > Resources.maxWidthForMobileView ? this.localize("commandBarEditLabel") : ""} text disabled={!this.props.isEditEnable} className="edit-button" onClick={this.props.onEditButtonClick} />
                <Dialog
                    className="delete-dialog-mobile"
                    cancelButton={this.localize("cancelButtonText")}
                    confirmButton={this.localize("confirmButtonText")}
                    content={this.localize("deletePopupBodyText")}
                    header={this.localize("deletePopupHeaderText")}
                    trigger={<Button icon={<TrashCanIcon />} content={this.state.windowWidth > Resources.maxWidthForMobileView ? this.localize("deleteButtonText") : ""} text disabled={!this.props.isDeleteEnable} className="delete-button" />}
                    onConfirm={this.props.onDeleteButtonClick}
                />
                <Flex.Item push>
                    <div style={{ width: "40rem" }}>
                        <Input
                            fluid icon={<SearchIcon onClick={(event: any) => this.handleKeyPress} />}
                            placeholder={this.localize("searchLabelText")}
                            value={this.state.searchValue}
                            onChange={this.handleChange}
                            onKeyUp={this.handleKeyPress}
                        />
                    </div>
                </Flex.Item>
            </Flex>
        );
    }
}

export default withTranslation()(CommandBar);