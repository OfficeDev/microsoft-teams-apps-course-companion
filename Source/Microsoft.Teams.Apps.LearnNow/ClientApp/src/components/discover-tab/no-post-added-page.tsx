// <copyright file="no-post-added-page.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from 'react';
import { Text, Button } from "@fluentui/react-northstar";
import { EyeIcon } from "@fluentui/react-icons-northstar";
import { WithTranslation, withTranslation } from "react-i18next";
import { useTranslation } from 'react-i18next';

import "../../styles/no-post-added-page.css";

interface INoPostAddedProps extends WithTranslation {
    handleAddNewResource: () => void;
    isValidUser: boolean;
}

/**
* Component for showing no data found message and show add new resource or learning module if user is teacher.
*/
const NoPostAddedPage: React.FunctionComponent<INoPostAddedProps> = props => {
    const localize = useTranslation().t;

    return (
        <div className="no-post-added-container">
            <div className="app-logo">
                <EyeIcon size="largest" />
            </div>
            <div className="add-new-post">
                <Text content={localize("addNewPostNote")} />
            </div>
            <div className="add-new-post-btn">
                {props.isValidUser &&
                    < Button content={localize("addNewText")} primary onClick={props.handleAddNewResource} />
                }
            </div>
        </div>
    )
}

export default withTranslation()(NoPostAddedPage)