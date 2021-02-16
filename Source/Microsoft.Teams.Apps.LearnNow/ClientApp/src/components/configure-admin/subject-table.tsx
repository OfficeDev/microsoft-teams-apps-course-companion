// <copyright file="subject-table.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Text, Table } from "@fluentui/react-northstar";
import CheckboxBase from "./checkbox-base";
import { useTranslation } from 'react-i18next';
import moment from 'moment';
import "../../styles/admin-configure-wrapper-page.css";

interface ISubjectTableProps {
    showCheckbox: boolean,
    responsesData: any[],
    onCheckBoxChecked: (responseId: string, isChecked: boolean) => void,
}

/**
* Component for subject table detaiLs.
*/
const SubjectTable: React.FunctionComponent<ISubjectTableProps> = props => {
    const localize = useTranslation().t;
    const userResponsesTableHeader = {
        key: "header",
        items: props.showCheckbox ?
            [
                { content: <div />, key: "check-box", className: "table-checkbox-cell" },
                {
                    content: <Text weight="regular" content={localize("adminCreateSubjectLabelText")} />, key: "grade"
                },
                { content: <Text weight="regular" content={localize("updatedByLabelText")} />, key: "createdby" },
                { content: <Text weight="regular" content={localize("updatedOnLabelText")} />, key: "createdon", className: "table-label-cell" }
            ]
            :
            [
                { content: <Text weight="regular" content={localize("adminCreateSubjectLabelText")} />, key: "grade" },
                { content: <Text weight="regular" content={localize("updatedByLabelText")} />, key: "createdby" },
                { content: <Text weight="regular" content={localize("updatedOnLabelText")} />, key: "createdon", className: "table-label-cell" }
            ],
    };

    let UserResponsesTableRows = props.responsesData.map((value: any, index) => (
        {
            key: index,
            style: {},
            items: props.showCheckbox ?
                [
                    { content: <CheckboxBase onCheckboxChecked={props.onCheckBoxChecked} value={value.id} />, key: index + "1", className: "table-checkbox-cell" },
                    { content: <Text content={value.subjectName} title={value.subjectName} />, key: index + "2", truncateContent: true },
                    { content: <Text content={value.userDisplayName} title={value.updatedBy} />, key: index + "3", truncateContent: true },
                    { content: <Text content={localize(moment(value.updatedOn).local().format('L'))} title={localize(moment(value.updatedOn).local().format('L'))} />, key: index + "4", truncateContent: true, className: "table-label-cell" },
                ]
                :
                [
                    { content: <Text content={value.subjectName} title={value.subjectName} />, key: index + "2", truncateContent: true },
                    { content: <Text content={value.userDisplayName} title={value.userDisplayName} />, key: index + "3", truncateContent: true },
                    { content: <Text content={localize(moment(value.updatedOn).local().format('L'))} title={localize(moment(value.updatedOn).local().format('L'))} />, key: index + "4", truncateContent: true, className: "table-label-cell" },
                ],
        }
    ));

    return (
        <div>
            <Table rows={UserResponsesTableRows}
                header={userResponsesTableHeader} className="table-cell-content" />
        </div>
    );
}

export default SubjectTable;