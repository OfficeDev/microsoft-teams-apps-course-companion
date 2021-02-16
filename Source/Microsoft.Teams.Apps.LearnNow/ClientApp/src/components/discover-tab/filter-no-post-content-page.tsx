﻿// <copyright file="filter-no-post-content-page.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from 'react';
import { Text } from "@fluentui/react-northstar";
import { EyeIcon } from "@fluentui/react-icons-northstar";
import { useTranslation } from 'react-i18next';

import "../../styles/no-post-added-page.css";

/**
* Component for showing no data found message if there is no data available for selected filters.
*/
const FilterNoPostContentPage: React.FunctionComponent<{}> = props => {
    const localize = useTranslation().t;

    return (
        <div className="no-post-added-container">
            <div className="app-logo">
                <EyeIcon size="largest" />
            </div>
            <div className="add-new-post">
                <Text content={localize("noDataPreviewNote")} />
            </div>
        </div>
    )
}

export default FilterNoPostContentPage;