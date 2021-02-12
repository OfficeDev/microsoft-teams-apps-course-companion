// <copyright file="tab-command-bar.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Flex, Input, Button, Text, AddIcon } from "@fluentui/react-northstar";
import { SearchIcon } from "@fluentui/react-icons-northstar";
import { Icon } from "@fluentui/react/lib/Icon";
import { useTranslation } from 'react-i18next'
import Resources, { ResourcesKeyCodes } from "../../constants/resources";

import "../../styles/command-bar.css";
import { IUserRole } from "../../model/type";

interface ICommandBarProps {
    onFilterButtonClick: () => void;
    onSearchInputChange: (searchString: string) => void;
    searchFilterPostsUsingAPI: () => void;
    commandBarSearchText: string;
    showSolidFilterIcon: boolean;
    handleAddClick: () => void;
    userRole: IUserRole;
}

const CommandBar: React.FunctionComponent<ICommandBarProps> = props => {
    const localize = useTranslation().t;
    const windowWidth = window.innerWidth;

    /**
    * Invokes for key press
    * @param event Object containing event details
    */
    const onTagKeyUp = (event: any) => {
        if (event.keyCode === ResourcesKeyCodes.keyCodeEnter) {
            props.searchFilterPostsUsingAPI();
        }
    }

    return (
        <div>
            <Flex gap="gap.small" vAlign="center" hAlign="end" className="command-bar-wrapper">
                <div>
                    <Flex.Item push>
                        {windowWidth > Resources.maxWidthForMobileView ?
                            <Button className="filter-button" icon={props.showSolidFilterIcon ? <Icon iconName="FilterSolid" className="filter-icon-filled" /> : <Icon iconName="Filter" className="menu-filter-icon" />} content={<Text content={localize("filterText")} className={props.showSolidFilterIcon ? "filter-icon-filled" : ""} />} text onClick={props.onFilterButtonClick} /> : <Button className="filter-button" icon={props.showSolidFilterIcon ? <Icon iconName="FilterSolid" className="filter-icon-filled" /> : <Icon iconName="Filter" className="menu-filter-icon" />} iconOnly text onClick={props.onFilterButtonClick} />}
                    </Flex.Item>
                </div>
                <div className="search-bar-wrapper">
                    <Input inverted fluid
                        onKeyUp={onTagKeyUp}
                        onChange={(event: any) => props.onSearchInputChange(event.target.value)}
                        value={props.commandBarSearchText}
                        placeholder={localize("searchPlaceholder")} />
                    <SearchIcon
                        key="search"
                        className="discover-search-icon"
                        onClick={(event: any) => props.searchFilterPostsUsingAPI()} />
                </div>
                {
                    (props.userRole.isTeacher || props.userRole.isAdmin)
                        ? windowWidth > Resources.maxWidthForMobileView ?
                            <Button content={localize("addNewText")} primary onClick={props.handleAddClick} />
                            : <><AddIcon outline onClick={props.handleAddClick} /></>
                        : <></>
                }
            </Flex>
        </div>
    );
}

export default CommandBar;