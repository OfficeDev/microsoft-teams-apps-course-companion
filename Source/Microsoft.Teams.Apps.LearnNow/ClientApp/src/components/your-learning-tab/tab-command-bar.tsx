// <copyright file="tab-command-bar.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Flex, Input, Checkbox } from "@fluentui/react-northstar";
import { SearchIcon } from "@fluentui/react-icons-northstar";
import { useTranslation } from 'react-i18next';
import { ResourcesKeyCodes } from "../../constants/resources";
import Resources from "../../constants/resources";

import "../../styles/command-bar.css";

interface ICommandBarProps {
    handleSearchInputChange: (searchString: string) => void;
    isValidUser: boolean;
    handleCreatedByToggleButtonChange: () => void;
    handleSearchIconClick: () => void;
    windowWidth: number;
}

const CommandBar: React.FunctionComponent<ICommandBarProps> = props => {
    const localize = useTranslation().t;

    /**
    * Invokes for key press
    * @param event Object containing event details
    */
    const onTagKeyUp = (event: any) => {
        if (event.keyCode === ResourcesKeyCodes.keyCodeEnter) {
            props.handleSearchIconClick();
        }
    }

    return (
        <div>
            <Flex gap="gap.small" vAlign="center" hAlign="end" className="command-bar-wrapper">
                {props.isValidUser ?
                    <Checkbox
                        label={props.windowWidth! >= Resources.maxWidthForMobileView ? localize("createdToggleButtonText") : ""}
                        onChange={props.handleCreatedByToggleButtonChange}
                        toggle />
                    : <></>
                }
                <div className="search-bar-wrapper">
                    <Input
                        inverted
                        fluid
                        onKeyUp={onTagKeyUp}
                        onChange={(event: any) => props.handleSearchInputChange(event.target.value)}
                        placeholder={localize("searchPlaceholder")} />
                    <SearchIcon
                        key="search"
                        className="discover-search-icon"
                        onClick={props.handleSearchIconClick}
                    />
                </div>
            </Flex>
        </div>
    );
}

export default CommandBar;