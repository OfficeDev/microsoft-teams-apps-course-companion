// <copyright file="file-control.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from 'react';
import { Button, Flex } from '@fluentui/react-northstar'
import { PaperclipIcon } from '@fluentui/react-icons-northstar';
import "../../styles/resource-content.css";
import Resources from '../../constants/resources';
import { getFileExtension, getFileName } from '../../helpers/helper';
import { FileType } from './file-types';

interface IFileControlProps {
    localizer: any
    getFileName: (fileName: string) => void,
    setUploadedFileInformation?: (fileName: string, fileExtension: string, fileToUpload: FormData) => void,
    setFileUploadError?: (isFileValid: boolean) => void,
    setFileFormatError?: (isFileFormatValid: boolean) => void,
    isUploadDisabled: boolean
}

interface IFileControlState {

}

/**
* Component to handle file upload.
*/
class FileUploadDownload extends React.Component<IFileControlProps, IFileControlState> {
    inputElement: any = null;
    fileToUpload: FormData = new FormData();

    constructor(props: any) {
        super(props);
    }

    /**
    * Handles click on attach file button by opening file dialog.
    */
    private handleClick = () => {
        this.inputElement.click();
    }

    /**
    * Create a file object from selected file.
    */
    private onFileNameSelected = async (e) => {
        if (e.target.files && e.target.files.length > 0) {
            let isFileValid: boolean;

            let fileSize = e.target.files[0].size;

            // Check file size 
            if (fileSize < Resources.fileSizeMaxAllowed) {
                isFileValid = true;
                if (this.props.setFileUploadError) {
                    this.props.setFileUploadError(isFileValid);
                }
                let filePath = e.target.files[0].name;
                let fileName = getFileName(filePath);
                let fileExtension = getFileExtension(fileName);
                let isValidFileFormat = this.ValidateFileFormat(fileExtension);
                if (!isValidFileFormat) {
                    if (this.props.setFileFormatError) {
                        this.props.setFileFormatError(isValidFileFormat);
                    }
                }

                else {
                    let formData: FormData = new FormData();
                    formData.append("FileInfo", e.target.files[0], `${e.target.files[0].name}`);
                    this.fileToUpload = formData;

                    // Set the state of file information that is to be uploaded.
                    if (this.props.setUploadedFileInformation) {
                        this.props.setUploadedFileInformation(fileName, fileExtension, this.fileToUpload);
                    }
                }
            }
            else {
                // If file is less than 4MB, show the required error.
                isFileValid = false;
                if (this.props.setFileUploadError) {
                    this.props.setFileUploadError(isFileValid);
                }
            }
        }
    }

    /**
    * Check if resourceType is valid
    * @param {fileExtension} fileExtension fileExtension of the selected file.
    */
    private ValidateFileFormat = (fileExtension: string) => {
        let extension = fileExtension.toLowerCase();
        if (extension === FileType.PPTX || extension === FileType.PPT || extension === FileType.XLSX || extension === FileType.XLS || extension === FileType.DOC || extension === FileType.DOCX || extension === FileType.PDF) {
            return true
        }
        else {
            return false;
        }
    }

    /**
    * Renders the component.
    */
    public render() {
        return (
            <>
                <Flex>
                    <div>
                        <input type="file" id="attachment" ref={input => this.inputElement = input} style={{ display: "none" }} onChange={this.onFileNameSelected} />
                        <Button content={this.props.localizer('uploadFileText')} className="attach-file-button" icon={<PaperclipIcon className="attach-icon" outline />} secondary onClick={this.handleClick} value="File" disabled={this.props.isUploadDisabled} />
                    </div>
                </Flex>
            </>
        );
    }
}

export default FileUploadDownload;