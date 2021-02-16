// <copyright file="title-bar.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import FilterBar from "./filter-bar";
import CommandBar from "./tab-command-bar";
import { ICheckBoxItem } from "./filter-bar";

import { ISubject, IGrade, ITag, ICreatedBy, IFilterModel, IUserRole } from "../../model/type";

interface ITitleBarProps {
    commandBarSearchText: string;
    onFilterClear: (isFilterOpened: boolean) => void;
    hideFilterbar: boolean;
    onSearchInputChange: (searchString: string) => void;
    onResetFilterClick: () => void;
    onGradeCheckboxStateChange: (currentValues: Array<ICheckBoxItem>) => void;
    onSubjectCheckboxStateChange: (currentValues: Array<ICheckBoxItem>) => void;
    onAddedByCheckboxStateChange: (currentValues: Array<ICheckBoxItem>) => void;
    onTagsCheckboxStateChange: (currentValues: Array<ICheckBoxItem>) => void;
    searchFilterPostsUsingAPI: () => void;
    handleAddClick: () => void;
    userRole: IUserRole;
    onFilterChangesSaved: (userSettings: IFilterModel) => void;
    selectedTags?: Array<string>
    selectedGrades?: Array<string>;
    selectedSubjects?: Array<string>;
    selectedCreatedBy?: Array<string>;
    allGrades: Array<IGrade>;
    allSubjects: Array<ISubject>;
    allTags: Array<ITag>;
    allCreatedBy: Array<ICreatedBy>
    isTagsFilterCountValid: boolean;
    isGradeFilterCountValid: boolean;
    isSubjectFilterCountValid: boolean;
    isCreatedByFilterCountValid: boolean;
}

const TitleBar: React.FunctionComponent<ITitleBarProps> = props => {

    let [showSolidFilter, setShowSolidFilter] = React.useState(false);

    let [allTags, setAllTags] = React.useState([] as Array<ITag>);
    let [allGrades, setAllGrades] = React.useState([] as Array<IGrade>);
    let [allSubjects, setAllSubjects] = React.useState([] as Array<ISubject>);
    let [createdByList, setCreatedByList] = React.useState([] as Array<ICreatedBy>);

    let [selectedTags, setSelectedTags] = React.useState([] as Array<string> | undefined);
    let [selectedGrades, setSelectedGrades] = React.useState([] as Array<string> | undefined);
    let [selectedSubjects, setSelectedSubjects] = React.useState([] as Array<string> | undefined);
    let [selectedCreatedBy, setCreatedBy] = React.useState([] as Array<string> | undefined);

    // Set state of allGrades only when master list of grade changes in parent component.
    React.useEffect(() => {
        setAllGrades(props.allGrades)
    }, [props.allGrades])

    // Set state of allSubjects only when master list of subject changes in parent component.
    React.useEffect(() => {
        setAllSubjects(props.allSubjects)
    }, [props.allSubjects])

    // Set state of allTags only when master list of tags changes in parent component.
    React.useEffect(() => {
        setAllTags(props.allTags)
    }, [props.allTags])

    // Set state of allAddedBy only when master list of addedBy changes in parent component.
    React.useEffect(() => {
        setCreatedByList(props.allCreatedBy)
    }, [props.allCreatedBy])

    // Set state of selectedGrades only when selected grades changes in parent component.
    React.useEffect(() => {
        setSelectedGrades(props.selectedGrades)
    }, [props.selectedGrades])

    // Set state of selectedSubjects only when selected subjects changes in parent component.
    React.useEffect(() => {
        setSelectedSubjects(props.selectedSubjects)
    }, [props.selectedSubjects])

    // Set state of selectedTags only when selected tags changes in parent component.
    React.useEffect(() => {
        setSelectedTags(props.selectedTags)
    }, [props.selectedTags])

    // Set state of selectedCreatedBy only when selected addedBy changes in parent component.
    React.useEffect(() => {
        setCreatedBy(props.selectedCreatedBy)
    }, [props.selectedCreatedBy])

    /**
    * Sets state to show/hide filter bar
    */
    const onOpenStateChange = () => {
        setShowSolidFilter(!showSolidFilter);
        props.onFilterClear(!showSolidFilter);
    }

    return (
        <>
            <CommandBar
                onFilterButtonClick={onOpenStateChange}
                onSearchInputChange={props.onSearchInputChange}
                showSolidFilterIcon={showSolidFilter}
                commandBarSearchText={props.commandBarSearchText}
                handleAddClick={props.handleAddClick}
                userRole={props.userRole}
                searchFilterPostsUsingAPI={props.searchFilterPostsUsingAPI}
            />
            <FilterBar
                tagsList={allTags}
                addedByList={createdByList}
                gradeList={allGrades}
                subjectList={allSubjects}
                selectedTags={selectedTags}
                selectedSubjects={selectedSubjects}
                selectedGrades={selectedGrades}
                selectedCreatedBy={selectedCreatedBy}
                isVisible={showSolidFilter}
                onFilterBarCloseClick={onOpenStateChange}
                onGradeCheckboxStateChange={props.onGradeCheckboxStateChange}
                onSubjectCheckboxStateChange={props.onSubjectCheckboxStateChange}
                onTagsCheckboxStateChange={props.onTagsCheckboxStateChange}
                onFilterChangesSaved={props.onFilterChangesSaved}
                onAddedByCheckboxStateChange={props.onAddedByCheckboxStateChange}
                onResetFilterClick={props.onResetFilterClick}
                isTagsFilterCountValid={props.isTagsFilterCountValid}
                isGradeFilterCountValid={props.isGradeFilterCountValid}
                isSubjectFilterCountValid={props.isSubjectFilterCountValid}
                isCreatedByFilterCountValid={props.isCreatedByFilterCountValid}
            />
        </>
    )
}

export default TitleBar;
