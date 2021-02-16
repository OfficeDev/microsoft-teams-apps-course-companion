// <copyright file="resource-api.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import axios from "./axios-decorator";
import { IFilterModel, IFilterRequestModel, IResourceDetail } from "../model/type";
import { AxiosResponse } from "axios";
import Resources from "../constants/resources";

/**
* Save resource details in storage.
* @param resourceDetails {IResourceDetail} Resource details to be stored in storage.
*/
export const createResource = async (resourceDetails: IResourceDetail): Promise<AxiosResponse<IResourceDetail>> => {
    let url = '/api/resources';
    return await axios.post(url, resourceDetails);
}

/**
* Get resource detail by resource id.
* @param resourceId {String} Unique identifier of resource entity.
*/
export const getResource = async (resourceId?: string): Promise<AxiosResponse<IResourceDetail>> => {
    let url = `/api/resources/${resourceId}`;
    return await axios.get(url);
}

/**
* Validate whether the title exists while creating/updating the resource.
* @param titleText {String} Title text entered by user.
*/
export const validateIfResourceTitleExists = async (titleText: string): Promise<AxiosResponse<IResourceDetail[]>> => {
    let url = '/api/resources/search?exactMatch=true';
    return await axios.post(url, { searchText: titleText });
}

/**
* Update resource details in database.
* @param resourceDetails {IResourceDetail} Resource details to be stored in database.
* @param resourceId {String} Resource identifier.
*/
export const updateResource = async (resourceDetails: IResourceDetail, resourceId?: string): Promise<AxiosResponse<IResourceDetail>> => {
    let url = `/api/resources/${resourceId}`;
    return await axios.patch(url, resourceDetails);
}

/**
* Delete resource from storage.
* @param resourceId {String} Unique identifier of resource entity.
*/
export const deleteResource = async (resourceId?: string): Promise<AxiosResponse<void>> => {
    let url = `/api/resources/${resourceId}`;
    return await axios.delete(url);
}

/**
* Save vote, if user has liked a resource.
* @param resourceId {String} Unique identifier of resource entity.
*/
export const userUpVoteResource = async (resourceId: string): Promise<AxiosResponse<boolean>> => {
    let url = `/api/resources/${resourceId}/upvote`;
    return await axios.post(url);
}

/**
* Remove vote, if user has reverted resource like.
* @param resourceId {String} Unique identifier of resource entity.
*/
export const userDownVoteResource = async (resourceId: string): Promise<AxiosResponse<boolean>> => {
    let url = `/api/resources/${resourceId}/downvote`;
    return await axios.post(url);
}

/**
* Get resources to be shown in tab based on selected filter.
* @param page {Number} Current page count for which resource needs to be fetched
* @param filterRequestDetails {IFilterModel | IFilterRequestModel} filter settings done by user
*/
export const getResources = async (page: number, filterRequestDetails: IFilterModel | IFilterRequestModel): Promise<AxiosResponse<IResourceDetail[]>> => {
    let url = `/api/resources/search?page=${page}`;
    return await axios.post(url, filterRequestDetails);
}

/**
* Get resources which are added for given learning module.
* @param learningmoduleId {String} learning module id for which resources needs to be fetched.
*/
export const getResourcesForModule = async (learningmoduleId: string): Promise<AxiosResponse<IResourceDetail[]>> => {
    let url = `/api/learningmodules/${learningmoduleId}/resources`;
    return await axios.get(url);
}

/**
* Get unique authors.
* @param  {Function} handleAuthFailure Authentication failure callback function.
*/
export const getAuthors = async (handleAuthFailure: (error: string) => void): Promise<any> => {
    let url = `/api/resources/authors?recordCount=${Resources.maximumCreatorRecordsCount}`;
    return await axios.get(url, handleAuthFailure);
}