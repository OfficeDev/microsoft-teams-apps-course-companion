// <copyright file="checkbox-wrapper.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Flex, Checkbox, Text, Avatar } from "@fluentui/react-northstar";

interface ICheckboxProps {
    title: JSX.Element;
    index: number;
    isChecked: boolean;
    onChange: (key: number, isChecked: boolean) => void
    isAddedBy: boolean;
    displayName: string;
}

const CheckboxWrapper: React.FunctionComponent<ICheckboxProps> = props => {
    return (
        <Flex gap="gap.small" className={"checkbox-wrapper-padding"}>
            <Checkbox className="checkbox-wrapper" label={props.isAddedBy ? <Text weight="light" content={<React.Fragment><Avatar
                name={props.displayName} size="smaller" className="avatar-image-margin" /><Text className="author-name" content={props.title} /></React.Fragment>} /> : props.title} key={props.index} checked={props.isChecked} onChange={(event, data: any) => props.onChange(props.index, data.checked)} />
        </Flex>
    );
}

export default CheckboxWrapper;