// <copyright file="user-settings-api.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import axios from "./axios-decorator";
import { IFilterModel } from "../model/type";
import { AxiosResponse } from "axios";

/**
* Save filter details to storage.
* @param resourceFilterSettings {IFilterModel} resource filter settings selected by the user.
* @param entityType {String} type of entity for which user setting details needs to be updated.
*/
export const createUserSettingsAsync = async (resourceFilterSettings: IFilterModel, entityType: string): Promise<AxiosResponse<boolean>> => {
    let url = `/api/usersettings/${entityType}`;
    return await axios.post(url, resourceFilterSettings);
}

/**
* Get user selected filter settings from storage.
* @param entityType {String} type of entity for which user setting details needs to be fetch.
*/
export const getSelectedFilters = async (entityType: string): Promise<AxiosResponse<IFilterModel>> => {
    let url = `/api/usersettings/${entityType}`;
    return await axios.get(url);
}