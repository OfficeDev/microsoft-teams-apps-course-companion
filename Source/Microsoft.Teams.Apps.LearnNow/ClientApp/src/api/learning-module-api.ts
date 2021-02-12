// <copyright file="learning-module-api.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import axios from "./axios-decorator";
import { ILearningModuleDetail, IResourceModuleDetails, IModuleResourceViewModel, IFilterModel, IFilterRequestModel } from "../model/type";
import { AxiosResponse } from "axios";
import Resources from "../constants/resources";

/**
* Save learning module details in storage.
* @param learningModuleDetails {ILearningModuleDetail} learning module details to be stored in storage.
*/
export const createLearningModule = async (learningModuleDetails: ILearningModuleDetail): Promise<AxiosResponse<ILearningModuleDetail>> => {
    let url = '/api/learningmodules';
    return await axios.post(url, learningModuleDetails);
}

/**
* Update learning module details in storage.
* @param learningModuleDetails {IModuleResourceViewModel} learning module details to be stored in storage.
*/
export const updateLearningModule = async (id: string, learningModuleDetails: IModuleResourceViewModel): Promise<AxiosResponse<ILearningModuleDetail>> => {
    let url = `/api/learningmodules/${id}`;
    return await axios.patch(url, learningModuleDetails);
}

/**
* Get learning module details by given id.
* @param id {String} learning module identifier.
*/
export const getLearningModule = async (id: string): Promise<AxiosResponse<IModuleResourceViewModel>> => {
    let url = `/api/learningmodules/${id}`;
    return await axios.get(url);
}

/**
* Delete learning module.
* @param moduleId {String} Learning module identifier.
*/
export const deleteLearningModule = async (moduleId?: string): Promise<AxiosResponse<ILearningModuleDetail[]>> => {
    let url = `/api/learningmodules/${moduleId}`;
    return await axios.delete(url);
}

/**
* Get learning modules to be shown on tab based selected filters
* @param page Current page count for which learning modules needs to be fetched
* @param filterDetails {IFilterModel} User selected filter details.
*/
export const getLearningModules = async (page: number, filterDetails: IFilterModel | IFilterRequestModel): Promise<AxiosResponse<ILearningModuleDetail[]>> => {
    let url = `/api/learningmodules/search?page=${page}`;
    return await axios.post(url, filterDetails);
}

/**
* Validate whether the title exists while creating/updating the learning module.
* @param titleText {String} User entered Learning module title text.
*/
export const validateIfLearningModuleTitleExists = async (titleText: string): Promise<AxiosResponse<ILearningModuleDetail[]>> => {
    let url = '/api/learningmodules/search?exactMatch=true';
    return await axios.post(url, { searchText: titleText });
}

/**
* Save resource learning module mapping in storage.
* @param resourceModuleDetails {IResourceModuleDetails}  resource learning module mapping details to be stored in storage.
*/
export const createResourceModuleMapping = async (resourceModuleDetails: IResourceModuleDetails): Promise<AxiosResponse<void>> => {
    let url = `/api/learningmodules/${resourceModuleDetails.LearningModuleId}/resources`;
    return await axios.post(url, resourceModuleDetails);
}

/**
* Save vote, if user has liked a learning module.
* @param learningModuleId {String} Unique identifier of learning module entity.
*/
export const userUpVoteLearningModule = async (learningModuleId: string): Promise<AxiosResponse<boolean>> => {
    let url = `/api/learningmodules/${learningModuleId}/upvote`
    return await axios.post(url);
}

/**
* Get learning module for given grade and subject.
* @param gradeId grade id of the selected grade.
* @param subjectId subject id of the selected subject.
*/
export const getLearningModuleForGradeAndSubject = async (gradeId: string, subjectId: string): Promise<AxiosResponse<ILearningModuleDetail[]>> => {
    let url = `/api/learningmodules/search?excludeEmptyModules=true`;
    return await axios.post(url, { gradeIds: [gradeId], subjectIds: [subjectId] });
}

/**
* Revert vote, if user has removed vote on a learning module.
* @param learningModuleId {String | Null} Unique identifier of learning module entity.
*/
export const userDownVoteLearningModule = async (learningModuleId: string): Promise<AxiosResponse<boolean>> => {
    let url = `/api/learningmodules/${learningModuleId}/downvote`
    return await axios.post(url);
}

/**
* Get unique authors
* @param  {Function} handleAuthFailure Authentication failure callback function.
*/
export const getAuthors = async (handleAuthFailure: (error: string) => void): Promise<any> => {
    let url = `/api/learningmodules/authors?recordCount=${Resources.maximumCreatorRecordsCount}`;
    return await axios.get(url, handleAuthFailure);
}