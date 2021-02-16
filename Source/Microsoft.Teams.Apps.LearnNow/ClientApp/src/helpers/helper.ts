// <copyright file="helper.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import { FileType } from "../components/file-control/file-types";
import { IDropDownItem } from "../model/type";

/**
* Returns true if input field value is null or contains white space.
* @param input {Object} Input field value.
*/
export const isNullorWhiteSpace = (input: string): boolean => {
    return !input || !input.trim();
}

/**
* Handle error occurred during API call.
* @param error {Object} Error response object
*/
export const handleError = (error: any, token: any, handleErrorCallback: (url: string) => void): any => {
    const errorStatus = error.status;
    if (errorStatus === 403) {
        handleErrorCallback("/error?code=403&token=" + token);
    }
    else if (errorStatus === 401) {
        handleErrorCallback("/error?code=401&token=" + token);
    }
    else {
        handleErrorCallback("/error?token=" + token);
    }
}

/**
* Get tagId by tagName
* @param tagId {string} tag id of tag selected.
* @param allTags {ITag[]} all tags.
*/
export const getTagById = (tagId: string, allTags: IDropDownItem[]) => {
    let tagName = "";
    allTags.forEach((tag) => {
        if (tag.key === tagId) {
            tagName = tag.header;
            return tagName;
        }
    });

    return tagName;
}

/**
* Get attachment filename.
* @param filePath {string} filePath of file selected.
*/
export const getFileName = (filePath: string) => {
    let fileName = filePath.split(/[\\/]/).pop() || "";
    return fileName;
}

/**
* Get attachment file extension.
* @param fileName {string} fileName of file selected.
*/
export const getFileExtension = (fileName: string) => {
    let regex = new RegExp('[^.]+$');
    let fileExtension = fileName.match(regex);

    if (!fileExtension) {
        return "";
    }

    let fileExtensionValue = fileExtension as any;
    if (fileExtensionValue !== undefined && (fileExtensionValue.index < 1 || fileExtensionValue[0] === undefined)) {
        return "";
    } else {
        return fileExtensionValue[0].toLowerCase();
    }
}

/**
 * Set file icon based on file attachment type.
 */
export const getFileImageFromFileName = (fileUrl: string) => {
    // Set file name and if file is not attached show default link file icon.
    let fileName = fileUrl ? getFileName(fileUrl) : "";
    let fileExtensionValue = getFileExtension(fileName);

    switch (fileExtensionValue) {
        case FileType.XLSX:
        case FileType.XLS: {
            return "Artifacts/Images/Excel.png";
        }

        case FileType.DOC:
        case FileType.DOCX: {
            return "Artifacts/Images/word.png";
        }
        case FileType.PPTX:
        case FileType.PPT: {
            return "Artifacts/Images/ppt.png";
        }
        case FileType.PDF: {
            return "Artifacts/Images/pdf.png";
        }
        default: {
            return "Artifacts/Images/link.png";
        }
    }
};
