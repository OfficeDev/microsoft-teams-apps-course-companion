// <copyright file="error-page.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { RouteComponentProps } from "react-router-dom";
import { Text } from "@fluentui/react-northstar";
import { WithTranslation, withTranslation } from "react-i18next";

import "../styles/site.css";

interface IErrorPageProps extends WithTranslation, RouteComponentProps {
}

class ErrorPage extends React.Component<IErrorPageProps, {}> {

    constructor(props: any) {
        super(props);
    }

    /**
    * Renders the component
    */
    public render(): JSX.Element {
        let localize = this.props.t;
        const params = this.props.match.params;
        let message = localize("generalErrorMessage");

        if ("code" in params) {
            const code = params["code"];
            if (code === "401") {
                message = localize("unauthorizedErrorMessage");
            } else if (code === "403") {
                message = localize("forbiddenErrorMessage");
            }
            else {
                message = localize("generalErrorMessage");
            }
        }

        return (
            <div className="container-div">
                <div className="container-subdiv">
                    <div className="error-message">
                        <Text content={message} error size="medium" />
                    </div>
                </div>
            </div>
        );
    }
}

export default withTranslation()(ErrorPage)