// <copyright file="popup-menu-checkboxes-content.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Input, Button, Provider, Divider, Text } from "@fluentui/react-northstar";
import { CloseIcon, SearchIcon } from "@fluentui/react-icons-northstar";
import CheckboxWrapper from "../checkbox-wrapper";
import { useTranslation } from 'react-i18next';
import { ICheckBoxItem } from "../resource-filter-bar/filter-bar";

import "../../styles/popup-menu.css";

interface IPopupMenuCheckboxesContentProps {
    showSearchBar: boolean,
    checkboxes: Array<ICheckBoxItem>,
    onCheckboxStateChange: (checkboxState: Array<ICheckBoxItem>) => void
    isAddedBy: boolean
    showMaxCountError: boolean
}

/**
* Component for rendering filter checkboxes and handle selection , filtering and clearing all selection.
*/
const PopupMenuCheckboxesContent: React.FunctionComponent<IPopupMenuCheckboxesContentProps> = props => {
    const localize = useTranslation().t;
    const [searchString, setSearchString] = React.useState("");
    const [disableClear, setDisableClear] = React.useState(false);

    React.useEffect(() => {
        let checkCount = props.checkboxes.reduce((counter, checkbox: ICheckBoxItem) => checkbox.isChecked ? counter + 1 : counter, 0);
        setDisableClear(checkCount === 0);
    }, [props.checkboxes])

    /**
    * Updates particular checkbox's isChecked state and passes changed state back to parent component.
    * @param {Number} key Unique key for checkbox which needs to be updated
    * @param {Boolean} checked Boolean indicating checkbox current value
    */
    const onCheckboxValueChange = (key: number, checked: boolean) => {
        let checkboxList = props.checkboxes.map((checkbox: ICheckBoxItem) => { return checkbox.key === key ? { ...checkbox, isChecked: checked } : checkbox; });
        let checkCount = checkboxList.reduce((counter, checkbox: ICheckBoxItem) => checkbox.isChecked ? counter + 1 : counter, 0);
        setDisableClear(checkCount === 0);

        if (searchString.trim().length) {
            let filteredCheckBoxItem = checkboxList.filter((checkboxItem: ICheckBoxItem) => {
                return checkboxItem.title.toLocaleLowerCase().includes(searchString.toLocaleLowerCase());
            })
            props.onCheckboxStateChange(filteredCheckBoxItem);
        }
        else {
            props.onCheckboxStateChange(checkboxList);
        }
    }

    /**
    * Sets all checkbox's isChecked to false to unselect all and passes changed state back to parent component.
    */
    const clearAll = () => {
        let checkboxList = props.checkboxes.map((checkbox: ICheckBoxItem) => ({ ...checkbox, isChecked: false }));
        props.onCheckboxStateChange(checkboxList);
        setDisableClear(true);
        setSearchString("");
    }

    /**
    * Renders the component.
    */
    return (
        <Provider>
            <div className="content-items-wrapper">
                {props.showMaxCountError &&
                    <Text content={localize("maxfilterCountError")} error size="small" />}
                {props.showSearchBar &&
                    <div className="content-items-headerfooter">
                        <Input icon={<SearchIcon />} placeholder={localize("searchPlaceholder")} value={searchString} fluid onChange={(event: any) => setSearchString(event.target.value.trim())} />
                    </div>}
                <Divider className="filter-popup-menu-divider" />
                <Button disabled={disableClear} primary className="clear-button" icon={<CloseIcon />} size="small" text onClick={() => clearAll()} content={localize("unselectedAll")} />
                <div className="content-items-body">
                    {
                        props.checkboxes.map((checkbox: ICheckBoxItem) => {
                            const checkboxTitle = checkbox.title.trim();
                            if (checkboxTitle.length && searchString.length ? checkboxTitle.toLocaleLowerCase().includes(searchString.toLocaleLowerCase()) : true) {
                                return (
                                    <CheckboxWrapper title={checkbox.checkboxLabel} displayName={checkboxTitle} isAddedBy={props.isAddedBy} isChecked={checkbox.isChecked} index={checkbox.key} onChange={(key, isChecked) => onCheckboxValueChange(key, isChecked)} />
                                );
                            }
                        })
                    }
                </div>
            </div>
        </Provider>
    );
}

export default PopupMenuCheckboxesContent;