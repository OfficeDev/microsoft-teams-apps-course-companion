// <copyright file="learning-module-resources.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Flex, Text, Table, Image, InfoIcon } from "@fluentui/react-northstar";
import CheckboxBase from "./checkbox-base";
import { useTranslation } from "react-i18next";
import { getFileImageFromFileName } from '../../helpers/helper';
import { resourceTableStyle } from "../../constants/customize.styled";
import Resources from "../../constants/resources";

import "../../styles/admin-configure-wrapper-page.css";
import "../../styles/learning-module.css";

interface ILearningModuleResourceTableProps {
    responsesData: any[];
    onCheckBoxChecked: (responseId: string, isChecked: boolean) => void;
    isGradeSubjectDisabled?: boolean;
    windowWidth: number
}

/**
 * Component for rendering learning module's resources, used in add/update learning module task module.
 */
const LearningModuleResourceTable: React.FunctionComponent<ILearningModuleResourceTableProps> = (
    props
) => {
    const localize = useTranslation().t;

    /**
     * Renders the component
     */
    return (
        <div className="table-div-resource">
            <Flex.Item>
                {props.responsesData?.length <= 1 ?
                    <Text className="resource-content-edit" content={localize("editResourceInLMOneCount", { "numberOfResources": props.responsesData?.length })} />
                    :
                    <Text className="resource-content-edit" content={localize("editResourceInLM", { "numberOfResources": props.responsesData?.length })} />
                }
            </Flex.Item>
            <Flex.Item>
                {props.isGradeSubjectDisabled ? <span><InfoIcon outline className="info-icon" title={localize("notEditableFieldError")} /></span> : <></>}
            </Flex.Item>
            <Table
                rows={getresourceTable(props)}
                aria-label="Static headless table"
                className="lm-resource-table"
            />
        </div>
    );
};

const getresourceTable = (learningModuleProps: any) => {
    let resourceTable = learningModuleProps.responsesData.map(
        (value: any, index: number) => ({
            key: index,
            styles: resourceTableStyle,
            items: [
                {
                    content: (
                        <CheckboxBase
                            checked={value.checkItem === undefined ? false : value.checkItem}
                            onCheckboxChecked={learningModuleProps.onCheckBoxChecked}
                            value={value.id}
                        />
                    ),
                    className: "table-checkbox-cell-lm",
                },
                {
                    content: (
                        <Image src={value.imageUrl} className="image-module-style" />
                    ),
                },
                {
                    content: (
                        <div className="column-style">
                            <Flex>
                                <Flex.Item>
                                    <div className="preview-file-icon">
                                        {
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
                                                content={value.subject?.subjectName}
                                                weight="semibold"
                                            /> |
                                            <Text
                                                className="grade-name"
                                                content={"" + value.grade?.gradeName}
                                            />
                                        </Flex>
                                    </div>
                                </Flex.Item>
                            </Flex>
                            {learningModuleProps.windowWidth! >= Resources.maxWidthForMobileView ?
                                <Flex>
                                    <Text
                                        className="resource-description-table"
                                        title={value.description}
                                        content={value.description}
                                    />

                                </Flex>
                                :
                                <Flex></Flex>
                            }
                        </div>
                    ),
                    className: "resource-data",
                },
            ],
        })
    );
    return resourceTable;
};

export default LearningModuleResourceTable;
