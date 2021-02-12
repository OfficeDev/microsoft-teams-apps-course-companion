// <copyright file="show-attachment.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Attachment, WordColorIcon, ExcelColorIcon, PowerPointColorIcon, Flex, FilesTxtIcon, FilesPdfIcon, CloseIcon, DownloadIcon } from "@fluentui/react-northstar";
import { FileType } from "../file-control/file-types";
import { getFileExtension } from "../../helpers/helper";
import { useTranslation } from "react-i18next";

import "../../styles/resource-content.css";
interface IShowAttachmentProps {
    fileName: string,
    showAttachment: boolean
    removeFileAttachment?: () => void,
    isViewOnly: boolean,
    adjustWidth: boolean,
    handleFileDownload?: () => void,
}

/**
* Component for showing file attachment.
*/
const ShowAttachment: React.FunctionComponent<IShowAttachmentProps> = props => {
    const localize = useTranslation().t;

    /**
    * Gets file icon from file extension.
    */
    const getFileIconFromFileName = () => {

        let fileExtension = getFileExtension(props.fileName);

        switch (fileExtension) {
            case FileType.XLSX:
            case FileType.XLS: {
                return <ExcelColorIcon />;
            }

            case FileType.DOC:
            case FileType.DOCX: {
                return <WordColorIcon />;
            }
            case FileType.PPTX:
            case FileType.PPT: {
                return <PowerPointColorIcon />;
            }
            case FileType.PDF: {
                return <FilesPdfIcon />;
            }
            default: {
                return <FilesTxtIcon />;
            }
        }
    };

    //Check whether view is preview only mode or edit mode.
    if (!props.isViewOnly) {
        return (
            <>
                {props.showAttachment &&
                    <Flex className="file-attachment-width">
                        <Flex.Item>
                            <Attachment icon={getFileIconFromFileName()} header={props.fileName}
                                action={{
                                    icon: <CloseIcon />,
                                    onClick: props.removeFileAttachment,
                                    title: localize("closeTitle"),
                                }} />
                        </Flex.Item>
                    </Flex>
                }
            </>
        );
    }
    else {
        return (
            <>
                {props.showAttachment && !props.adjustWidth &&
                    <Flex className="file-attachment-width attachment-text-overflow">
                        <Flex.Item>
                            <Attachment icon={getFileIconFromFileName()} header={props.fileName} />
                        </Flex.Item>
                    </Flex>
                }
                {props.showAttachment && props.adjustWidth &&
                    <Flex className="file-attachment-width-preview attachment-text-overflow">
                        <Flex.Item>
                            <Attachment icon={getFileIconFromFileName()} header={props.fileName}
                                action={{
                                    icon: <DownloadIcon />,
                                    onClick: props.handleFileDownload,
                                    title: localize("downloadButtonTitle"),
                                }}
                                title={props.fileName}
                            />
                        </Flex.Item>
                    </Flex>
                }
            </>
        );
    }
}
export default ShowAttachment;