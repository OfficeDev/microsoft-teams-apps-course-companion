// <copyright file="grade-api.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import axios from "./axios-decorator";
import { AxiosResponse } from "axios";
import { IGrade } from "../model/type";

/**
* posts grade details in the storage.
* @param grade {IGrade} grade object to be stored in database.
*/
export const createGrade = async (grade: IGrade): Promise<any> => {
    let url = '/api/grade';
    return await axios.post(url, grade);
}

/**
* Update grade details.
* @param grade {IGrade} grade object to be stored in database.
* @param id {String} identifier of grade which is to be updated.
*/
export const updateGrade = async (grade: IGrade, id: string): Promise<AxiosResponse<IGrade>> => {
    let url = `/api/grade/${id}`;
    return await axios.patch(url, grade);
}

/**
* Gets all grades.
* @param  {Function} handleAuthFailure Authentication failure callback function.
*/
export const getAllGrades = async (handleAuthFailure: (error: string) => void): Promise<AxiosResponse<any>> => {
    let url = '/api/grade';
    return await axios.get(url, handleAuthFailure);
}

/**
* Delete grades from the storage.
* @param {Array<any>} data Grades data which needs to be deleted.
*/
export const deleteGrades = async (data: any[]): Promise<AxiosResponse<Boolean>> => {

    let url = '/api/grade/gradesdelete';
    return await axios.post(url, data);
}

/**
* Get grade details from API.
* @param {string | null} id Unique grade ID for which details will be fetched.
*/
export const getGrade = async (id: string | null): Promise<AxiosResponse<IGrade>> => {

    let url = `/api/grade/${id}`;
    return await axios.get(url);
}