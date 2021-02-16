// <copyright file="learning-module-content-preview-resources.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Flex, Text, Table, Image } from "@fluentui/react-northstar";
import { IResourceDetail } from "../../model/type";
import { useTranslation } from "react-i18next";
import { getFileImageFromFileName } from "../../helpers/helper";
import { resourceTableStyle } from "../../constants/customize.styled";
import Resources from "../../constants/resources";

import "../../styles/admin-configure-wrapper-page.css";
import "../../styles/learning-module.css";
import "../../styles/learning-module-preview.css";

interface ILearningModuleProps {
    responsesData: IResourceDetail[];
    handleResourceClick?: (resource: IResourceDetail) => void;
    windowWidth: number
}

/**
 * Component for rendering learning module resource details table, used in learning module preview task module.
 */
const LearningModuleTablePreview: React.FunctionComponent<ILearningModuleProps> = (props: any) => {
    const localize = useTranslation().t;

    return (
        <div className="table-div">
            { props.responsesData?.length <= 1 ? (

                <p className="resource-content">

                    {localize("editResourceInLMOneCount", {
                        numberOfResources: props.responsesData?.length,
                    })}
                </p>
            ) : (
                    <p className="resource-content">
                        {localize("editResourceInLM", {
                            numberOfResources: props.responsesData?.length,
                        })}
                    </p>
                )}
            <Table rows={getLearningModuleTableRows(props)} className="edit-preview-resource-table" />
        </div>
    );
};

const getLearningModuleTableRows = (learningModuleProps: any) => {
    let learningModuleTableRows = learningModuleProps.responsesData.map(
        (value: IResourceDetail, index: number) => ({
            key: index,
            styles: resourceTableStyle,
            items: [
                {
                    content: (
                        <Image
                            src={value.imageUrl}
                            onClick={() => learningModuleProps.handleResourceClick(value)}
                            className="image-module-style cursor-pointer"
                        />
                    ),
                },

                {
                    content: (
                        <div
                            className="column-style-preview cursor-pointer"
                            onClick={() => learningModuleProps.handleResourceClick(value)}
                        >
                            <Flex>
                                <Flex.Item>
                                    <div className="preview-file-icon">{
                                        learningModuleProps.windowWidth! >= Resources.maxWidthForMobileView ?
                                            <Image
                                                src={getFileImageFromFileName(value.attachmentUrl)}
                                            />
                                            :
                                            <></>
                                    }

                                    </div>
                                </Flex.Item>
                                <Flex.Item>
                                    <div>
                                        <Flex gap="gap.small">
                                            <Text
                                                className="resource-title"
                                                content={value.title}
                                                weight="bold"
                                            />
                                        </Flex>
                                        <Flex>
                                            <Text
                                                className="subject-name"
                                                content={value.subject.subjectName}
                                                weight="semibold"
                                            /> |
                                            <Text
                                                className="grade-name"
                                                content={value.grade.gradeName}
                                            />
                                        </Flex>
                                    </div>
                                </Flex.Item>
                            </Flex>
                            {learningModuleProps.windowWidth! >= Resources.maxWidthForMobileView ?
                                <Flex>
                                    <Text
                                        className="resource-description"
                                        title={value.description.trim()}
                                        content={
                                            value.description.trim()
                                        }
                                    />
                                </Flex>
                                :
                                <Flex></Flex>
                            }
                        </div>
                    )
                }
            ],
        })
    );
    return learningModuleTableRows;
};

export default LearningModuleTablePreview;
