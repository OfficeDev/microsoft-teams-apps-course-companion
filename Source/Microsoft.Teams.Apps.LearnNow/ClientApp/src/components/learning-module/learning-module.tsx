// <copyright file="learning-module.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { WithTranslation, withTranslation } from "react-i18next";
import * as microsoftTeams from "@microsoft/teams-js";
import { isNullorWhiteSpace, handleError, getTagById } from "../../helpers/helper";
import { Text, Flex, Input, Button, TextArea, Loader, ChevronStartIcon, InfoIcon, Dropdown } from "@fluentui/react-northstar";
import { TFunction } from "i18next";
import Constants from "../../constants/resources";
import { ILearningModuleDetail, IGrade, ISubject, RequestMode, IResourceDetail, IModuleResourceViewModel, ILearningModuleTag, ITag, IDropDownItem, PageType } from "../../model/type";
import { createLearningModule, getLearningModule, updateLearningModule, validateIfLearningModuleTitleExists } from '../../api/learning-module-api'
import { getAllSubjects } from "../../api/subject-api";
import { getAllGrades } from "../../api/grade-api";
import SelectImagePage from "../select-preview-image/select-preview-image"
import PreviewContent from "../preview-resource-content/preview-content-learning-module"
import LearningModuleResourceTable from "./learning-module-resource";
import LearningModuleEditPreviewItems from "./learning-module-edit-preview"
import { getResource } from "../../api/resource-api";
import { getAllTags } from "../../api/tag-api";
import Resources from "../../constants/resources";

import "../../styles/resource-content.css";

interface ILearningModuleState {
    learningModuleDetail: ILearningModuleDetail,
    allTags: IDropDownItem[],
    allSubjects: IDropDownItem[],
    allGrades: IDropDownItem[],
    imageArray: Array<any>,
    isTitlePresent: boolean,
    isDescriptionValid: boolean,
    isGradeValid: boolean,
    isSubjectValid: boolean,
    loading: boolean,
    isSaveButtonLoading: boolean,
    isImageNextButtonDisabled: boolean,
    error: string
    isEditMode: boolean,
    editTitleText: string,
    isTitleValid: boolean,
    moduleResources: IResourceDetail[],
    userSelectedItem: string;
    filterItemEdit: IResourceDetail[],
    isGradeSubjectDisabled: boolean,
    isTagsCountValid: boolean,
    learningModuleTags: ILearningModuleTag[],
    windowWidth: number,
    tag: string,
    selectedTags: IDropDownItem[],
    pageType: PageType,
}

/**
* Component for rendering learning module create/update task module.
*/
class LearningModule extends React.Component<WithTranslation, ILearningModuleState> {

    localize: TFunction;
    history: any;
    requestViewMode: RequestMode;
    resourceId: string | null = null;
    learningModuleId: string | null = null;
    isResourceAddMode: boolean | null = null;
    constructor(props: any) {
        super(props);
        this.localize = this.props.t;
        this.history = props.history;
        this.state = {
            learningModuleDetail: {} as ILearningModuleDetail,
            allSubjects: [],
            allGrades: [],
            allTags: [],
            imageArray: [],
            error: "",
            isTitlePresent: true,
            isDescriptionValid: true,
            isGradeValid: true,
            isSubjectValid: true,
            loading: true,
            isSaveButtonLoading: false,
            isImageNextButtonDisabled: true,
            isEditMode: false,
            editTitleText: "",
            isTitleValid: true,
            moduleResources: [],
            userSelectedItem: "",
            filterItemEdit: [],
            isGradeSubjectDisabled: false,
            learningModuleTags: [],
            isTagsCountValid: true,
            windowWidth: window.innerWidth,
            tag: "",
            selectedTags: [],
            pageType: PageType.Form,
        }
        let search = this.history.location.search;
        let params = new URLSearchParams(search);
        this.requestViewMode = params.get("viewMode") === RequestMode.edit ? RequestMode.edit : RequestMode.create;
        this.resourceId = params.get("resourceId") ? params.get("resourceId") : "";
        this.learningModuleId = params.get("resourceId") ? params.get("resourceId") : "";
        this.isResourceAddMode = params.get("addresource") ? true : false;
    }

    public async componentDidMount() {

        // Fetch data for grade, tags and subjects for drop down.
        await this.getDropDownData();

        if (this.requestViewMode === RequestMode.edit) {
            this.setState({ isEditMode: true }, this.getLearningModuleDetail);
        } else if (this.isResourceAddMode) {
            this.setState({ isGradeSubjectDisabled: true }, this.getResourceDetails);
        }

        if (this.requestViewMode === RequestMode.create) {
            this.setState({ loading: false })
        }

        window.addEventListener("resize", this.setWindowWidth);
    }

    public componentWillUnmount() {
        window.removeEventListener('resize', this.setWindowWidth);
    }

    /**
    * Get window width real time
    */
    private setWindowWidth = () => {
        if (window.innerWidth !== this.state.windowWidth) {
            this.setState({ windowWidth: window.innerWidth });
        }
    };

    /**
    * Get grade, subject and tags data for respective drop-downs.
    */
    private getDropDownData = async () => {
        await Promise.all([this.getGrades(), this.getSubjects(), this.getTags()]);
    }

    /**
    * Method to get all grades from database.
    */
    private getGrades = async () => {
        const gradesResponse = await getAllGrades(this.handleAuthenticationFailure);
        if (gradesResponse.status === 200 && gradesResponse.data) {
            let allGrades = gradesResponse.data.map((grade: IGrade) => ({ key: grade.id, header: grade.gradeName } as IDropDownItem));
            this.setState({ allGrades: allGrades });
        }
    }

    /**
    * Method to get all subjects from database.
    */
    private getSubjects = async () => {
        const subjectResponse = await getAllSubjects(this.handleAuthenticationFailure);
        if (subjectResponse.status === 200 && subjectResponse.data) {
            let allSubjects = subjectResponse.data.map((subject: ISubject) => ({ key: subject.id, header: subject.subjectName } as IDropDownItem));
            this.setState({ allSubjects: allSubjects });
        }
    }

    /**
    * Method to get all tags from database.
    */
    private getTags = async () => {
        const tagsResponse = await getAllTags(this.handleAuthenticationFailure);
        if (tagsResponse.status === 200 && tagsResponse.data) {
            let allTags = tagsResponse.data.map((tag: ITag) => ({ key: tag.id, header: tag.tagName } as IDropDownItem));
            this.setState({ allTags: allTags });
        }
    }

    /**
    * handle error occurred during authentication
    */
    private handleAuthenticationFailure = (error: string) => {
        // When the getAuthToken function returns a "resourceRequiresConsent" error, 
        // it means Azure AD needs the user's consent before issuing a token to the app. 
        // The following code redirects the user to the "Sign in" page where the user can grant the consent. 
        // Right now, the app redirects to the consent page for any error.
        console.error("Error from getAuthToken: ", error);
        this.history.push('/signin');
    }

    /**
    * Get learning module details
    */
    private getLearningModuleDetail = async () => {
        const learningModuleDetailResponse = await getLearningModule(this.learningModuleId!);
        if (learningModuleDetailResponse.data) {
            let moduleDetails = learningModuleDetailResponse.data.learningModule;
            let moduleResources = learningModuleDetailResponse.data.resources;

            this.setState({
                learningModuleDetail: moduleDetails,
                editTitleText: moduleDetails.title.trim(),
                loading: false,
                isImageNextButtonDisabled: false,
                isGradeSubjectDisabled: moduleResources.length > 0 ? true : false,
                selectedTags: moduleDetails.learningModuleTag.map((tag: ILearningModuleTag) => ({
                    key: tag.tagId,
                    header: tag.tag.tagName
                } as IDropDownItem)),
                learningModuleTags: moduleDetails.learningModuleTag.map((tag: ILearningModuleTag) => ({
                    tagId: tag.tagId
                } as ILearningModuleTag)),
            });

            moduleResources.forEach((resource: IResourceDetail) => {
                resource.checkItem = true;
            });
            this.setState({ moduleResources: moduleResources });
        }
    }

    /**
    * Method to get resource details.
    */
    private getResourceDetails = async () => {
        const resourceDataResponse = await getResource(this.resourceId!);
        if (resourceDataResponse.status === 200 && resourceDataResponse.data) {
            let resourceDetails: ILearningModuleDetail = this.state.learningModuleDetail;
            resourceDetails.title = "";
            resourceDetails.description = "";
            resourceDetails.grade = resourceDataResponse.data.grade;
            resourceDetails.subject = resourceDataResponse.data.subject;
            resourceDetails.gradeId = resourceDataResponse.data.gradeId;
            resourceDetails.subjectId = resourceDataResponse.data.subjectId;
            this.setState({
                learningModuleDetail: resourceDetails,
                loading: false
            });
        }
    }

    /**
    *Returns text component containing error message for tag input field.
    */
    private getTagError = () => {
        if (!this.state.isTagsCountValid) {
            return (<Text content={this.localize("tagsCountError")} error size="small" />);
        }
        return (<></>);
    }

    /**
    * Get the tag name for specified tagId.
    *@param {string} tagId selected tag's id.
    */
    private getTagById = (tagId: string) => {
        return getTagById(tagId, this.state.allTags);
    }

    /**
    * Handle grade change event.
    * @param {Any} event event object.
    * @param {string} dropdownProps props received on dropdown value click
    */
    private handleGradeChange = (event: any, dropdownProps?: any) => {
        let grade = dropdownProps.value;
        if (grade) {
            let moduleGrade = { id: grade.key, gradeName: grade.header } as IGrade;
            let learningModuleDetail = { ...this.state.learningModuleDetail, gradeId: moduleGrade.id, grade: moduleGrade };
            this.setState({ learningModuleDetail: learningModuleDetail, isGradeValid: true, error: "" });
        }
    }

    /**
    * Handle subject change event.
    * @param {Any} event event object.
    * @param {string} dropdownProps props received on dropdown value click
    */
    private handleSubjectChange = (event: any, dropdownProps?: any) => {
        let subject = dropdownProps.value;
        if (subject) {
            let moduleSubject = { id: subject.key, subjectName: subject.header } as ISubject;
            let learningModuleDetail = { ...this.state.learningModuleDetail, subjectId: moduleSubject.id, subject: moduleSubject };
            this.setState({ learningModuleDetail: learningModuleDetail, isSubjectValid: true, error: "" });
        }
    }

    /**
    * Handle tag change event.
    * @param {Any} event event object.
    * @param {string} dropdownProps props received on dropdown value click
    */
    private handleTagChange = (event: any, dropdownProps?: any) => {
        let tags = dropdownProps.value;
        if (tags.length > Resources.tagsMaxCount) {
            this.setState({ isTagsCountValid: false })
            return;
        }
        if (tags) {
            let learningModuleTags = tags.map((selectedTag: IDropDownItem) => ({
                tagId: selectedTag.key
            } as ILearningModuleTag))
            this.setState({ isTagsCountValid: true, learningModuleTags: learningModuleTags, selectedTags: tags });

        }
    }

    /**
    * Handle title change event.
    *@param {Any} event event details.
    */
    private handleTitleChange = (event: any) => {
        let learningModuleDetail = { ...this.state.learningModuleDetail, title: event.target.value };
        this.setState({ learningModuleDetail: learningModuleDetail, isTitleValid: true, isTitlePresent: true });
    }

    /**
    * Handle description change event.
    */
    private handleDescriptionChange = (event: any) => {
        let resourceDetail = { ...this.state.learningModuleDetail, description: event.target.value };
        this.setState({ learningModuleDetail: resourceDetail, isDescriptionValid: true });
    }

    /**
    * Handle image click event.
    */
    private handleImageClick = (url: string) => {
        let learningModuleDetail = { ...this.state.learningModuleDetail, imageUrl: url };
        this.setState({ learningModuleDetail: learningModuleDetail, isImageNextButtonDisabled: false });
    }

    /**
    * Set image array
    *@param {Array<any>} images image URL collection.
    */
    private setImageArray = (images: Array<any>) => {
        this.setState({ imageArray: images });
    }

    /**
    * Returns text component containing error message for failed name field validation.
    *@param {boolean} isValuePresent Indicates whether value is present or not.
    */
    private getRequiredFieldError = (isValuePresent: boolean) => {
        if (!isValuePresent) {
            return (<Text content={this.localize("emptyFieldErrorMessage")} error size="small" />);
        }

        return (<></>);
    }

    /**
    *Returns text component containing error message for failed resource title field validation.
    *@param {boolean} isTitleValid Indicates whether title is valid.
    */
    private getTitleExistsError = (isTitleValid: boolean) => {
        if (!isTitleValid) {
            return (<Text content={this.localize("resourceTitleAlreadyExists")} error size="small" />);
        }
        return (<></>);
    }

    /**
    *Validate input fields
    */
    private checkIfSubmitAllowed = async () => {
        let learningModule = this.state.learningModuleDetail;
        var isTitleValid = await this.ValidateIfTitleExists(learningModule.title);
        let isTitlePresent = true;
        let isDescriptionValid = true;
        let isSubjectValid = true;
        let isGradeValid = true;
        let isSubmitAllowed = true;

        if (learningModule.title && !isTitleValid) {
            isTitleValid = false;
        }

        if (isNullorWhiteSpace(learningModule.title)) {
            isTitlePresent = false;
        }

        if (isNullorWhiteSpace(learningModule.description)) {
            isDescriptionValid = false;
        }

        if (!learningModule.subjectId) {
            isSubjectValid = false;
        }

        if (!learningModule.gradeId) {
            isGradeValid = false;
        }


        if (!isGradeValid || !isSubjectValid || !isDescriptionValid || !isTitlePresent || !isTitleValid) {
            isSubmitAllowed = false;
        }

        this.setState({
            isTitlePresent: isTitlePresent,
            isTitleValid: isTitleValid!,
            isDescriptionValid: isDescriptionValid,
            isGradeValid: isGradeValid,
            isSubjectValid: isSubjectValid,
        });

        return isSubmitAllowed;
    }

    /**
    *Check if learning module title already exists. Returns true if title is valid.
    *@param {title} title selected title.
    */
    private ValidateIfTitleExists = async (title: string) => {
        if (title) {
            // Returns resource list with same title.
            let response = await validateIfLearningModuleTitleExists(title);
            if (response.status === 200 && response.data) {
                if (this.state.isEditMode && response.data.length === 1) {  // Edit Mode
                    return response.data[0].id === this.resourceId;
                } else {
                    return !response.data.length;
                }
            }
            return false;
        }
    }

    /**
    * Handle share button click to store resource details.
    */
    private handleShareButtonClick = async () => {
        if (await this.checkIfSubmitAllowed()) {
            let moduleData: ILearningModuleDetail;
            if (this.state.isEditMode) {
                moduleData = await this.updateLearningModuleAsync() as ILearningModuleDetail;
                let isSuccess = moduleData ? Resources.successFlag : Resources.errorFlag;
                let tags = this.state.selectedTags.map((dropDownItem: IDropDownItem) => {
                    let tags: ITag = { tagName: dropDownItem.header, id: dropDownItem.key }
                    let learningModuleTags: ILearningModuleTag = {
                        tag: tags,
                        tagId: dropDownItem.key
                    }
                    return learningModuleTags
                });

                moduleData.learningModuleTag = tags;
                let details: any = { isSuccess: isSuccess, title: this.state.learningModuleDetail.title, saveResponse: moduleData }
                microsoftTeams.tasks.submitTask(details);
            } else {
                moduleData = await this.saveLearningModuleAsync();
                if (this.isResourceAddMode) {
                    this.history.push(`addlearningitems?gradeId=${moduleData.gradeId}&subjectId=${moduleData.subjectId}&resourceId=${this.resourceId}`);
                } else {
                    let isSuccess = moduleData ? Resources.successFlag : Resources.errorFlag;

                    let tags = this.state.selectedTags.map((dropDownItem: IDropDownItem) => {
                        let tags: ITag = { tagName: dropDownItem.header, id: dropDownItem.key }
                        let learningModuleTags: ILearningModuleTag = {
                            tag: tags,
                            tagId: dropDownItem.key
                        }
                        return learningModuleTags
                    });
                    moduleData.grade = this.state.learningModuleDetail.grade;
                    moduleData.subject = this.state.learningModuleDetail.subject;
                    moduleData.learningModuleTag = tags
                    let details: any = { isSuccess: isSuccess, title: this.state.learningModuleDetail.title, saveResponse: moduleData }
                    microsoftTeams.tasks.submitTask(details);
                }
            }
        }
    }

    /**
    * Handle next button click on content page to on select image page.
    */
    private handleContentNextButtonClick = async (event: any) => {
        if (await this.checkIfSubmitAllowed()) {
            this.setState({ pageType: PageType.Image })
        }
    }

    /**
    * Handle next button click on select image page to go to preview resource details.
    */
    private handleImageNextButtonClick = async () => {
        if (this.state.learningModuleDetail.imageUrl) {
            this.setState({ pageType: PageType.Preview, isImageNextButtonDisabled: false })
        } else {
            this.setState({ isImageNextButtonDisabled: true })
        }
    }

    /**
    * Handle back button click to go to content page.
    */
    private handleImageBackButtonClick = async () => {
        this.setState({ pageType: PageType.Form })
    }

    /**
    * Handle back button click to go to add to learning module page.
    */
    private handleBackButtonClick = async () => {
        this.history.push(`addlearningitems?resourceId=${this.resourceId}`);
    }


    /**
    * Handle back button click to go to select image page page.
    */
    private handlePreviewBackButtonClick = async () => {
        this.setState({ pageType: PageType.Image })
    }

    /**
    * Handle error callback to redirect to error page.
    */
    private handleErrorCallback = (url: string) => {
        this.history.push(url)
    }

    /**
    * Save module details to storage.
    */
    private saveLearningModuleAsync = async () => {
        this.setState({ isSaveButtonLoading: true });
        let module = { ...this.state.learningModuleDetail, learningModuleTag: this.state.learningModuleTags };

        // Store new learning module details in storage.
        let response = await createLearningModule(module);

        if (response.status !== 200 && response.status !== 204) {
            this.setState({ isSaveButtonLoading: false });
            handleError(response, null, this.handleErrorCallback);
        }

        return response.data;
    }

    /**
    * Update resource details in storage.
    */
    private updateLearningModuleAsync = async () => {
        this.setState({ isSaveButtonLoading: true });
        let module = { ...this.state.learningModuleDetail };
        let moduleResources = this.state.filterItemEdit;
        module.learningModuleTag = this.state.learningModuleTags;

        let learningModuleDetail: IModuleResourceViewModel = {
            learningModule: module,
            resources: moduleResources
        }

        // Store new resource details in storage.            
        let response = await updateLearningModule(learningModuleDetail.learningModule.id, learningModuleDetail);
        if (response.status !== 200 && response.status !== 204) {
            this.setState({ isSaveButtonLoading: false });
            handleError(response, null, this.handleErrorCallback);
            return null;
        }

        return response.data;
    }

    /**
    * Renders learning module details when learning module is selected.
    * @param {String} resourceId resource identifier.
    * @param {Boolean} isSelected represents whether resource is selected or not.
    */
    private onLearningModuleSelected = (resourceId: string, isSelected: boolean) => {
        // array of resource to show in preview
        let moduleResources = this.state.moduleResources
        let filterItemEdit: IResourceDetail[] = [];
        if (isSelected) {
            let userSelectedModules = this.state.userSelectedItem;
            userSelectedModules = resourceId;
            moduleResources!.map((resource: IResourceDetail) => {
                if (resource.id === resourceId) {
                    resource.checkItem = true;
                }
            });
            this.setState({ userSelectedItem: userSelectedModules, moduleResources: moduleResources })
        } else {
            moduleResources!.map((resource: IResourceDetail) => {
                if (resource.id === resourceId) {
                    resource.checkItem = false;
                }
            });

            this.setState({ userSelectedItem: "" })
        }
        moduleResources!.map((resource: IResourceDetail) => {
            if (resource.checkItem) {
                filterItemEdit.push(resource);
            }

        });
        this.setState({ filterItemEdit: filterItemEdit, moduleResources: moduleResources })
    }

    /**
    * Render the component.
    */
    private renderResourceContent() {
        return (
            <div>
                {
                    this.state.pageType === PageType.Form &&
                    <div className="container-tab-lm">
                        <div className="create-content-lm">
                            <div className={this.requestViewMode === RequestMode.create || this.state.moduleResources.length ? "create-sub-div-add" : "create-sub-div"}>
                                <Flex gap="gap.small">
                                    <Flex.Item size="size.half">
                                        <Flex>
                                            <Text size="small" content={"*" + this.localize('gradeText')} />
                                            <Flex.Item push>
                                                {this.getRequiredFieldError(this.state.isGradeValid)}
                                            </Flex.Item>
                                        </Flex>
                                    </Flex.Item>
                                    <Flex.Item size="size.half">
                                        <Flex>
                                            <Text className="subject-text" size="small" content={"*" + this.localize('subjectText')} />
                                            <Flex.Item push>
                                                {this.getRequiredFieldError(this.state.isSubjectValid)}
                                            </Flex.Item>
                                        </Flex>
                                    </Flex.Item>
                                </Flex>
                                <Flex gap="gap.small" className="input-padding">
                                    <Flex.Item size="size.half">
                                        <Dropdown
                                            search
                                            items={this.state.allGrades}
                                            defaultSearchQuery={this.state.learningModuleDetail.grade ? this.state.learningModuleDetail.grade.gradeName : ""}
                                            placeholder={this.localize('gradePlaceHolderText')}
                                            noResultsMessage={this.localize("noGradeFoundError")}
                                            toggleIndicator={{ styles: { display: 'none' } }}
                                            fluid
                                            onChange={this.handleGradeChange}
                                            className="dropdown-suggestion-box"
                                            disabled={this.state.isGradeSubjectDisabled}
                                        />
                                    </Flex.Item>
                                    <Flex.Item size="size.half">
                                        <Dropdown
                                            search
                                            items={this.state.allSubjects}
                                            defaultSearchQuery={this.state.learningModuleDetail.subject ? this.state.learningModuleDetail.subject.subjectName : ""}
                                            placeholder={this.localize('subjectPlaceHolderText')}
                                            noResultsMessage={this.localize("noSubjectFoundError")}
                                            onChange={this.handleSubjectChange}
                                            toggleIndicator={{ styles: { display: 'none' } }}
                                            fluid
                                            className="dropdown-suggestion-box"
                                            disabled={this.state.isGradeSubjectDisabled}
                                        />
                                    </Flex.Item>
                                </Flex>
                                <Flex>
                                    <Text size="small" content={"*" + this.localize('titleText')} />
                                    <Flex.Item push>
                                        {this.getRequiredFieldError(this.state.isTitlePresent)}
                                    </Flex.Item>
                                    <Flex.Item push>
                                        {this.state.isTitlePresent ?
                                            this.getTitleExistsError(this.state.isTitleValid) : <></>
                                        }
                                    </Flex.Item>
                                </Flex>
                                <Flex>
                                    <Input placeholder={this.localize('titlePlaceHolderText')} className="input-padding-module" fluid value={this.state.learningModuleDetail.title} onChange={(event: any) => this.handleTitleChange(event)} maxLength={Constants.titleMaxLength} /></Flex>
                                <Flex>
                                    <Text size="small" content={"*" + this.localize('descriptionText')} />
                                    <Flex.Item push>
                                        {this.getRequiredFieldError(this.state.isDescriptionValid)}
                                    </Flex.Item>
                                </Flex>
                                <Flex>
                                    <TextArea placeholder={this.localize('descriptionPlaceHolderText')} className="input-padding-description-module" fluid value={this.state.learningModuleDetail.description} onChange={this.handleDescriptionChange} maxLength={Constants.descriptionMaxLength} />
                                </Flex>
                                <Flex className="tag-padding">
                                    <Text size="small" content={this.localize('tagsText')} />
                                    <Flex.Item push>
                                        {this.getTagError()}
                                    </Flex.Item>
                                </Flex>
                                <Dropdown
                                    multiple
                                    search
                                    items={this.state.allTags}
                                    placeholder={this.localize('tagPlaceholderText')}
                                    noResultsMessage={this.localize("noTagFoundError")}
                                    toggleIndicator={{ styles: { display: 'none' } }}
                                    fluid
                                    onChange={(e, selectedOption) => { this.handleTagChange(e, selectedOption) }}
                                    className="tag-dropdown-input"
                                    value={this.state.selectedTags}
                                />

                                {this.state.isEditMode &&
                                    <LearningModuleResourceTable responsesData={this.state.moduleResources} onCheckBoxChecked={this.onLearningModuleSelected} isGradeSubjectDisabled={this.state.isGradeSubjectDisabled} windowWidth={this.state.windowWidth} />}

                            </div>
                        </div>
                        <Flex>
                            <div className="tab-footer">
                                <Flex space="between">
                                    {
                                        this.resourceId !== "" && !this.state.isEditMode &&
                                        <Flex className="back-image-button-create">
                                            <Button icon={<ChevronStartIcon />} content={this.localize("backButtonText")} text onClick={this.handleBackButtonClick} />
                                        </Flex>
                                    }
                                    {
                                        this.state.isEditMode && this.state.moduleResources.length > 0 &&
                                        <Flex className="info-div">
                                            <InfoIcon outline className="info-icon" title={this.localize("editLMValidationText")} /><Text content={this.localize("editLMValidationText")} />
                                        </Flex>
                                    }
                                    <Flex.Item push>
                                        <Button className="next-button" content={this.localize("nextButtonText")} primary loading={this.state.isSaveButtonLoading} onClick={this.handleContentNextButtonClick} disabled={this.state.isSaveButtonLoading} />
                                    </Flex.Item>
                                </Flex>
                            </div>
                        </Flex>
                    </div>
                }
                {
                    this.state.pageType === PageType.Image &&
                    <SelectImagePage
                        handleImageNextButtonClick={this.handleImageNextButtonClick}
                        handleImageBackButtonClick={this.handleImageBackButtonClick}
                        handleImageClick={this.handleImageClick}
                        setImageArray={this.setImageArray}
                        imageArray={this.state.imageArray}
                        defaultImageSearchText={this.state.learningModuleDetail!.title}
                        isImageNextButtonDisabled={this.state.isImageNextButtonDisabled}
                        existingImage={this.state.learningModuleDetail!.imageUrl}
                        windowWidth={this.state.windowWidth}
                    />
                }
                {
                    this.state.pageType === PageType.Preview && !this.state.isEditMode &&
                    <PreviewContent
                        selectedTags={this.state.selectedTags}
                        resourceDetail={this.state.learningModuleDetail}
                        showImage={true}
                        isViewOnly={true}
                        handlePreviewBackButtonClick={this.handlePreviewBackButtonClick}
                        handleShareButtonClick={this.handleShareButtonClick}
                    />
                }
                {
                    this.state.pageType === PageType.Preview && this.state.isEditMode &&
                    <Flex>
                        <LearningModuleEditPreviewItems handleShareButtonClick={this.handleShareButtonClick} handlePreviewBackButtonClick={this.handlePreviewBackButtonClick} learningModuleDetails={this.state.learningModuleDetail} responsesData={this.state.filterItemEdit} learningModuleTags={this.state.selectedTags} getTagById={this.getTagById} />
                    </Flex>
                }
            </div>
        );
    }

    /**
    * Renders the component.
    */
    public render() {
        let contents = this.state.loading
            ? <p><em><Loader /></em></p>
            : this.renderResourceContent();
        return (
            <div>
                {contents}
            </div>
        );
    }
}
export default withTranslation()(LearningModule);