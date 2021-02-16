// <copyright file="member-validation-api.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import { AxiosResponse } from "axios";
import axios from "./axios-decorator";
import { IUserRole } from "../model/type";

/**
* get current logged in user role.
* @param  {Function} handleAuthFailure Authentication failure callback function.
*/
export const getUserRole = async (handleAuthFailure: (error: string) => void): Promise<AxiosResponse<IUserRole>> => {
    let url = '/api/groupmember';
    return await axios.get(url, handleAuthFailure);
}

/**
* Validate if user is a member of moderator security group.
*/
export const validateIfUserIsModerator = async (): Promise<AxiosResponse<Boolean>> => {
    let url = '/api/groupmember/moderator';
    return await axios.get(url);
}
