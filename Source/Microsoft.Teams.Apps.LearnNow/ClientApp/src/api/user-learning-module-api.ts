// <copyright file="user-learning-module-api.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import axios from "./axios-decorator";
import { AxiosResponse } from "axios";
import { ILearningModuleDetail, IUserLearningModule, IUserLearningFilterModel } from "../model/type";

/**
* Save learning module details in users saved list in the storage.
* @param userLearningModule {IUserLearningModule} user learning module details to be stored in database.
*/
export const createUserLearningModule = async (userLearningModule: IUserLearningModule): Promise<AxiosResponse<boolean>> => {
    let url = '/api/me/learningmodules';
    return await axios.post(url, userLearningModule);
}

/**
* Delete learning module from user saved list.
* @param learningmoduleId {String} module id of module to user private learning module from storage.
*/
export const deleteUserLearningModule = async (userLearningmoduleId: string): Promise<AxiosResponse<boolean>> => {
    let url = `/api/me/learningmodules/${userLearningmoduleId}`;
    return await axios.delete(url);
}

/**
* Get user learning modules details for based on current page and selected filters..
* @param page {Number} Current page number for which respective learning module needs to be fetched.
* @param filterDetails {IUserLearningFilterModel} User selected filter details.
*/
export const searchUserLearningModules = async (page: number, filterDetails: IUserLearningFilterModel): Promise<AxiosResponse<ILearningModuleDetail[]>> => {
    let url = `/api/me/learningmodules/search?page=${page}`;
    return await axios.post(url, filterDetails);
}