// <copyright file="error-message.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Text } from "@fluentui/react-northstar";
import { useTranslation } from "react-i18next";

interface IErrorMessageProps {
    errorMessage: string;
    isGenericError: boolean;
}

/**
* Component for rendering error message text.
*/
const ErrorMessage: React.FunctionComponent<IErrorMessageProps> = props => {
    const localize = useTranslation().t;

    /**
    * Renders the component.
    */
    return (
        <div >
            {props.errorMessage &&
                <Text content={localize(props.errorMessage)} className={props.isGenericError ? "generic-error-message" : "field-error-message"} error size="medium" />
            }
        </div>
    );
}

export default ErrorMessage;