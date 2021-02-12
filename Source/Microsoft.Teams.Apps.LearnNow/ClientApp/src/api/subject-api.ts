// <copyright file="subject-api.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import axios from "./axios-decorator";
import { AxiosResponse } from "axios";
import { ISubject } from "../model/type";

/**
* Save subject details in the storage.
* @param subject {ISubject} subject object to be stored in database.
*/
export const createSubject = async (subject: ISubject): Promise<AxiosResponse<ISubject>> => {
    let url = '/api/subject';
    return await axios.post(url, subject);
}

/**
* Gets all subject details from the storage.
* @param  {Function} handleAuthFailure Authentication failure callback function.
*/
export const getAllSubjects = async (handleAuthFailure: (error: string) => void): Promise<AxiosResponse<any>> => {
    let url = '/api/subject';
    return await axios.get(url, handleAuthFailure);
}

/**
* Update subject details in the storage.
* @param subject {ISubject} subject object to be stored in database.
* @param id {String} subject id object of subject which is to be updated.
*/
export const updateSubject = async (subject: ISubject, id: string): Promise<AxiosResponse<ISubject>> => {
    let url = `/api/subject/${id}`;
    return await axios.patch(url, subject);
}

/**
* Delete subjects from the storage.
* @param {Array<any>} data Selected subject identifiers which needs to be deleted.
*/
export const deleteSubjects = async (data: any[]): Promise<AxiosResponse<Boolean>> => {

    let url = '/api/subject/subjectsdelete';
    return await axios.post(url, data);
}

/**
* Get subject details for given subject identifier from API.
* @param {number | null} id Unique subject identifier for which details will be fetched.
*/
export const getSubject = async (id: string | null): Promise<AxiosResponse<ISubject>> => {

    let url = `/api/subject/${id}`;
    return await axios.get(url);
}