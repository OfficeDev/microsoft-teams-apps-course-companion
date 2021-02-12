// <copyright file="user-resource-api.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import axios from "./axios-decorator";
import { AxiosResponse } from "axios";
import { IUserResource, IResourceDetail, IUserLearningFilterModel } from "../model/type";


/**
* Save resource details in users saved list in the storage.
* @param userResource {IUserResource} user resource details to be stored in database.
*/
export const createUserResource = async (userResource: IUserResource): Promise<AxiosResponse<boolean>> => {
    let url = `/api/me/resources`;
    return await axios.post(url, userResource);
}

/**
* Delete resource details from users saved list in the storage.
* @param userResourceId {String} user resource id using which resource details to be deleted from storage.
*/
export const deleteUserResource = async (userResourceId: string): Promise<AxiosResponse<boolean>> => {
    let url = `/api/me/resources/${userResourceId}`;
    return await axios.delete(url);
}

/**
* Search user resources created or saved by user.
* @param page {Number} Current page number for which respective resources needs to be fetched.
* @param filterDetails {IFilterRequestModel} User selected filter details.
*/
export const searchUserResources = async (page: number, filterDetails: IUserLearningFilterModel): Promise<AxiosResponse<IResourceDetail[]>> => {
    let url = `/api/me/resources/search?page=${page}`;
    return await axios.post(url, filterDetails);
}