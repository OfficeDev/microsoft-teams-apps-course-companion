// <copyright file="tab-configuration-api.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import axios from "./axios-decorator";
import { ITabConfiguration } from "../model/type";
import { AxiosResponse } from "axios";

/**
* Save tab configuration details in the storage.
* @param tabConfigurationDetail {ITabConfiguration} tab configuration object to be stored in database.
* @param groupId {String} group id of current team.
*/
export const createTabConfiguration = async (tabConfigurationDetail: ITabConfiguration, groupId: string): Promise<AxiosResponse<ITabConfiguration>> => {
    let url = `/api/tab-configuration?groupId=${groupId}`;
    return await axios.post(url, tabConfigurationDetail);
}

/**
* update tab configuration details in the storage.
* @param tabConfigurationDetail {ITabConfiguration} tab configuration object to be updated in database.
* @param tabId {String} Unique tab identifier.
* @param groupId {String} group id of current team.
*/
export const updateTabConfiguration = async (tabConfigurationDetail: ITabConfiguration, tabId: string, groupId: string): Promise<AxiosResponse<ITabConfiguration>> => {
    let url = `/api/tab-configuration/${tabId}?groupId=${groupId}`;
    return await axios.patch(url, tabConfigurationDetail);
}

/**
* Get tab configuration details for given tab id.
* @param tabId {String} tab id of the teams tab for which tab configuration detail need to obtained.
* @param groupId {String} group id of current team  .
*/
export const getTabConfiguration = async (tabId: string, groupId: string): Promise<any> => {
    let url = `/api/tab-configuration/${tabId}?groupId=${groupId}`;
    return await axios.get(url);
}