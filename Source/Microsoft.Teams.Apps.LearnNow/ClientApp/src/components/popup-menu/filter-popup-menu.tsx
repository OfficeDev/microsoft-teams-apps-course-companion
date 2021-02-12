// <copyright file="filter-popup-menu.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Popup, Button, Text } from "@fluentui/react-northstar";
import { ChevronDownIcon } from "@fluentui/react-icons-northstar";
import PopupMenuCheckboxesContent from "./popup-menu-checkboxes-content";

import "../../styles/popup-menu.css";

interface IFilterPopupMenuProps {
    checkboxes?: Array<any>,
    title: string,
    showSearchBar?: boolean,
    onCheckboxStateChange: (typeState: Array<any>) => void,
    isAddedBy: boolean
    showMaxCountError?: boolean;
}

const FilterPopupMenu: React.FunctionComponent<IFilterPopupMenuProps> = props => {
    const [isPopupOpen, setIsPopupOpen] = React.useState(false);

    if (props.checkboxes) {
        return (
            <>
                <Popup
                    open={isPopupOpen}
                    align="end"
                    position="below"
                    onOpenChange={(e, { open }: any) => setIsPopupOpen(open)}
                    trigger={
                        <Button
                            className={`mobile-button ${isPopupOpen ? "gray-background" : "no-background"}`}
                            content={<Text weight="light" content={props.title} />}
                            iconPosition="after" icon={isPopupOpen ? <ChevronDownIcon rotate={180} className={"gray-background"} /> : <ChevronDownIcon className={"no-background"} />} text />}
                    content={
                        <PopupMenuCheckboxesContent
                            isAddedBy={props.isAddedBy}
                            showSearchBar={props.showSearchBar!}
                            checkboxes={props.checkboxes}
                            onCheckboxStateChange={props.onCheckboxStateChange}
                            showMaxCountError={props.showMaxCountError!} />}
                    trapFocus
                />
            </>
        );
    }
    else {
        return (<></>);
    }
}

export default FilterPopupMenu;