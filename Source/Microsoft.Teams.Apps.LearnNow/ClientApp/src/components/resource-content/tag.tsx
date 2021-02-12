// <copyright file="tag.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Label, Text } from "@fluentui/react-northstar";
import { CloseIcon } from "@fluentui/react-icons-northstar";
import "../../styles/tags.css";

interface ITagProps {
    tagContent: string;
    index: number;
    showRemoveIcon: boolean;
    onRemoveClick?: (index: number) => void
}

/**
* Renders the tag component.
*/
const Tag: React.FunctionComponent<ITagProps> = props => {

    /**
    * Check whether remove icon is to be displayed or not
    */
    if (props.showRemoveIcon) {
        return (
            <Label
                circular
                content={<Text className="tag-text-form" content={props.tagContent} title={props.tagContent} size="small" />}
                className="tags-label-wrapper"
                icon={<CloseIcon outline onClick={() => props.onRemoveClick!(props.index)} className="close-icon-tag" />}
            />
        );
    }
    else {
        return (
            <Label
                circular
                content={<div className="tag-text-card"><Text className="tag-text-card" content={props.tagContent} title={props.tagContent} size="small" /></div>}
                className="tags-label-wrapper"
            />
        );
    }
}

export default React.memo(Tag);