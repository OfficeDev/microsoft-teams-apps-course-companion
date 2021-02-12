// <copyright file="filter-bar.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Flex, Text } from "@fluentui/react-northstar";
import { CloseIcon, RetryIcon } from "@fluentui/react-icons-northstar";
import FilterPopupMenu from "../popup-menu/filter-popup-menu";
import { ISubject, IGrade, ITag, ICreatedBy, IFilterModel } from "../../model/type";
import { useTranslation } from "react-i18next";
import { Icon } from '@fluentui/react/lib/Icon';

import "../../styles/filter-bar.css";

export interface ICheckBoxItem {
    key: number;
    id: string;
    title: string;
    checkboxLabel: JSX.Element,
    isChecked: boolean;
}

interface IFilterBarProps {
    isVisible: boolean;
    subjectList: Array<ISubject>;
    gradeList: Array<IGrade>;
    tagsList: Array<ITag>;
    addedByList: Array<ICreatedBy>;
    selectedTags?: Array<string>;
    selectedGrades?: Array<string>;
    selectedSubjects?: Array<string>;
    selectedCreatedBy?: Array<string>;
    onFilterBarCloseClick: () => void;
    onSubjectCheckboxStateChange: (currentValues: Array<ICheckBoxItem>) => void;
    onGradeCheckboxStateChange: (currentValues: Array<ICheckBoxItem>) => void;
    onTagsCheckboxStateChange: (currentValues: Array<ICheckBoxItem>) => void;
    onAddedByCheckboxStateChange: (currentValues: Array<ICheckBoxItem>) => void;
    onFilterChangesSaved: (userSettings: IFilterModel) => void;
    onResetFilterClick: () => void;
    isTagsFilterCountValid: boolean;
    isGradeFilterCountValid: boolean;
    isSubjectFilterCountValid: boolean;
    isCreatedByFilterCountValid: boolean;
}

const FilterBar: React.FunctionComponent<IFilterBarProps> = props => {

    const localize = useTranslation().t;
    let [searchText, setSearchText] = React.useState("");

    // Grade list
    let [gradeList, setGradeList] = React.useState([] as Array<ICheckBoxItem>);

    // Subject list
    let [subjectList, setSubjectList] = React.useState([] as Array<ICheckBoxItem>);

    // Tag list
    let [tagsList, setTagsList] = React.useState([] as Array<ICheckBoxItem>);

    // Created by list
    let [createdByList, setCreatedByList] = React.useState([] as Array<ICheckBoxItem>);

    // Set state of grade list only when selected grades or master list of grade changes in parent component.
    React.useEffect(() => {
        const gradeList = props.gradeList.map((value: IGrade, index: number) => {
            let isChecked = props.selectedGrades?.find(x => x === value.id!) ? true : false
            return { isChecked: isChecked, key: index, id: value.id!, title: value.gradeName, checkboxLabel: <Text content={value.gradeName} /> };
        });
        setGradeList(gradeList);
    }, [props.gradeList, props.selectedGrades]);

    // Set state of subject list only when selected subjects or master list of subject changes in parent component.
    React.useEffect(() => {
        const subjectList = props.subjectList.map((value: ISubject, index: number) => {
            let isChecked = props.selectedSubjects?.find(x => x === value.id!) ? true : false
            return { isChecked: isChecked, key: index, id: value?.id!, title: value?.subjectName, checkboxLabel: <Text content={value?.subjectName} /> };
        });
        setSubjectList(subjectList);
    }, [props.subjectList, props.selectedSubjects]);

    // Set state of tags list only when selected tags or master list of tags changes in parent component.
    React.useEffect(() => {
        const tagList = props.tagsList.map((value: ITag, index: number) => {
            let isChecked = props.selectedTags?.find(x => x === value.id!) ? true : false
            return { isChecked: isChecked, key: index, id: value.id!, title: value.tagName, checkboxLabel: <Text content={value.tagName} /> };
        });

        setTagsList(tagList);
    }, [props.tagsList, props.selectedTags]);

    React.useEffect(() => {
        const addedByList = props.addedByList.map((value: ICreatedBy, index: number) => {
            let isChecked = props.selectedCreatedBy?.find(x => x === value.userId!) ? true : false
            return { isChecked: isChecked, key: index, id: value.userId, title: value.displayName, checkboxLabel: <Text content={value.displayName} /> };
        });
        setCreatedByList(addedByList);
    }, [props.addedByList, props.selectedCreatedBy]);

    /**
    *Sets state of 'Subject' filter item when checkbox value changes.
    *@param subjectValues Array of subject checkboxes with updated user selection.
    */
    const onSubjectCheckboxStateChange = (subjectValues: Array<ICheckBoxItem>) => {
        let subjectDetails = [...subjectValues];
        setSubjectList(subjectDetails);
        props.onSubjectCheckboxStateChange(subjectDetails);
    }

    /**
    *Sets state of 'Grade' filter item when checkbox value changes.
    *@param gradeValues Array of grades checkboxes with updated user selection.
    */
    const onGradeCheckboxStateChange = (gradeValues: Array<ICheckBoxItem>) => {
        let gradeDetails = [...gradeValues];
        setGradeList(gradeDetails);
        props.onGradeCheckboxStateChange(gradeDetails);
    }

    /**
    *Sets state of 'Tags' filter item when checkbox value changes.
    *@param tagsValues Array of 'tags' checkboxes with updated user selection.
    */
    const onTagsCheckboxStateChange = (tagsValues: Array<ICheckBoxItem>) => {
        let tagDetails = [...tagsValues];
        setTagsList(tagDetails);
        props.onTagsCheckboxStateChange(tagDetails);
    }

    /**
    *Sets state of 'Author' filter item when checkbox value changes.
    *@param addedByValues Array of 'author' checkboxes with updated user selection
    */
    const onAddedByCheckboxStateChange = (addedByValues: Array<ICheckBoxItem>) => {
        let addedByDetails = [...addedByValues]
        setCreatedByList(addedByDetails)
        props.onAddedByCheckboxStateChange(addedByDetails);
    }

    /**
    *Removes all filters and hides filter bar.
    */
    const onCloseIconClick = () => {
        if (searchText.trim().length > 0) {
            setSearchText("");
        }
        props.onFilterBarCloseClick();
    }

    /**
    *Resets all filters and hides filter bar.
    */
    const onResetIconClick = () => {
        if (searchText.trim().length > 0) {
            setSearchText("");
        }

        if (subjectList.filter((subject: ICheckBoxItem) => { return subject.isChecked }).length) {
            const updatedList = subjectList.map((subject: ICheckBoxItem) => ({ ...subject, isChecked: false }));
            setSubjectList(updatedList);
        }

        if (tagsList.filter((tag: ICheckBoxItem) => { return tag.isChecked }).length) {
            const updatedList = tagsList.map((tag: ICheckBoxItem) => ({ ...tag, isChecked: false }));
            setTagsList(updatedList)
        }

        if (createdByList.filter((addedBy: ICheckBoxItem) => { return addedBy.isChecked }).length) {
            const updatedList = createdByList.map((addedBy: ICheckBoxItem) => ({ ...addedBy, isChecked: false }));
            setCreatedByList(updatedList);
        }

        if (gradeList.filter((grade: ICheckBoxItem) => { return grade.isChecked }).length) {
            const updatedList = gradeList.map((grade: ICheckBoxItem) => ({ ...grade, isChecked: false }));
            setGradeList(updatedList)
        }
        props.onResetFilterClick();
    }

    /**
    *Saves all filters and hides filter bar.
    */
    const onSaveButtonClick = () => {

        if (searchText.trim().length > 0) {
            setSearchText("");
        }

        var resourceSettings = {} as IFilterModel;

        resourceSettings.gradeIds = gradeList.filter((grade: ICheckBoxItem) => grade.isChecked).map((grade: ICheckBoxItem) => grade.id);
        resourceSettings.subjectIds = subjectList.filter((subject: ICheckBoxItem) => subject.isChecked).map((subject: ICheckBoxItem) => subject.id);
        resourceSettings.tagIds = tagsList.filter((tag: ICheckBoxItem) => tag.isChecked).map((tag: ICheckBoxItem) => tag.id);
        resourceSettings.createdByObjectIds = createdByList.filter((createdBy: ICheckBoxItem) => createdBy.isChecked).map((createdBy: ICheckBoxItem) => createdBy.id);
        props.onFilterChangesSaved(resourceSettings);
    }

    const renderFilterBar = () => {
        if (props.isVisible) {
            return (
                <Flex className="filter-bar">
                    <Flex gap="gap.small" vAlign="center" className="filter-bar-wrapper">
                        <Flex.Item>
                            <>
                                <div className="filter-bar-item-container">
                                    <FilterPopupMenu title={localize("gradeLabelText")} isAddedBy={false} showSearchBar={true} checkboxes={gradeList} onCheckboxStateChange={onGradeCheckboxStateChange} showMaxCountError={!props.isGradeFilterCountValid} />
                                    <FilterPopupMenu title={localize("subjectLabelText")} isAddedBy={false} showSearchBar={true} checkboxes={subjectList} onCheckboxStateChange={onSubjectCheckboxStateChange} showMaxCountError={!props.isSubjectFilterCountValid} />
                                    <FilterPopupMenu title={localize("tagsLabelText")} isAddedBy={false} showSearchBar={true} checkboxes={tagsList} onCheckboxStateChange={onTagsCheckboxStateChange} showMaxCountError={!props.isTagsFilterCountValid} />
                                    <FilterPopupMenu title={localize("addedByLabelText")} isAddedBy={true} showSearchBar={true} checkboxes={createdByList} onCheckboxStateChange={onAddedByCheckboxStateChange} showMaxCountError={!props.isCreatedByFilterCountValid} />
                                </div>
                                <div className="filter-bar-icons">
                                    <Icon iconName="save" className="filter-icon-save" onClick={onSaveButtonClick} />
                                    <RetryIcon outline className="filter-icon" onClick={onResetIconClick} />
                                    <CloseIcon outline className="filter-icon" onClick={onCloseIconClick} />
                                </div>
                            </>
                        </Flex.Item>
                    </Flex>
                </Flex>
            );
        }
        else {
            return (<></>);
        }
    }
    return renderFilterBar();
}

export default FilterBar;