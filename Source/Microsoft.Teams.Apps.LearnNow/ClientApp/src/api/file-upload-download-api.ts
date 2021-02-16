/*
    <copyright file="file-upload-download-api.ts" company="Microsoft Corporation">
    Copyright (c) Microsoft. All rights reserved.
    </copyright>
*/

import axios from "./axios-decorator";
import { AxiosResponse } from "axios";

/**
* Get blob URL.
* @param resourceId {String} Resource id of the attachment for which blob URL need to obtained.
*/
export const getDownloadUri = async (resourceId: string): Promise<AxiosResponse<string>> => {
    let url =`/api/file/download/${resourceId}`;

    return await axios.get(url);
}

/**
* Upload file to storage blob.
* @param fileToUpload {Object} File information to be uploaded in blob.
 * Return attachment URL of the file that is uploaded.
*/
export const uploadFile = async (fileToUpload: FormData): Promise<AxiosResponse<string>> => {
    let url ='/api/file/upload';
    return await axios.post(url, fileToUpload);
}