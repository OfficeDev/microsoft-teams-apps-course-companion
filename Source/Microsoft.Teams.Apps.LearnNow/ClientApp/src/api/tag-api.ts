// <copyright file="tag-api.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import axios from "./axios-decorator";
import { AxiosResponse } from "axios";
import { ITag } from "../model/type";

/**
* Saves tag details in the storage.
* @param tag {ITag} tag details to be stored in database.
*/
export const createTag = async (tag: ITag): Promise<AxiosResponse<ITag>> => {
    let url = '/api/tag';
    return await axios.post(url, tag);
}

/**
* Gets all tag details from the storage.
* @param  {Function} handleAuthFailure Authentication failure callback function.
*/
export const getAllTags = async (handleAuthFailure: (error: string) => void): Promise<AxiosResponse<any>> => {
    let url = '/api/tag';
    return await axios.get(url, handleAuthFailure);
}

/**
* posts updated tag details in the storage.
* @param tag {ITag} tag details to be stored in database.
* @param id {String} tag identifier of tag which is to be updated.
*/
export const updateTag = async (tag: ITag, id?: string): Promise<AxiosResponse<ITag>> => {
    let url = `/api/tag/${id}`;
    return await axios.patch(url, tag);
}

/**
* Delete tags for given tag identifiers from the storage.
* @param data {Array<any>} Selected data which needs to be deleted.
*/
export const deleteTags = async (data: any[]): Promise<AxiosResponse<Boolean>> => {

    let url = '/api/tag/tagsdelete';
    return await axios.post(url, data);
}

/**
* Get tag details from API for given tag identifier.
* @param {number | null} id Unique tag identifier for which details will be fetched.
*/
export const getTag = async (id: string | null): Promise<AxiosResponse<ITag>> => {

    let url = `/api/tag/${id}`;
    return await axios.get(url);
}