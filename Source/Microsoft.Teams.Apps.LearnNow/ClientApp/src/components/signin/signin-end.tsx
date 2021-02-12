// <copyright file="signin-end.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import React, { useEffect } from "react";
import { RouteComponentProps } from "react-router-dom";
import * as microsoftTeams from "@microsoft/teams-js";


const SignInSimpleEnd: React.FunctionComponent<RouteComponentProps> = props => {

    const { history } = props;

    // Parse hash parameters into key-value pairs
    function getHashParameters() {
        const hashParams: any = {};
        window.location.hash.substr(1).split("&").forEach(function (item) {
            let source = item.split("="),
                key = source[0],
                value = source[1] && decodeURIComponent(source[1]);
            hashParams[key] = value;
        });
        return hashParams;
    }

    useEffect(() => {
        microsoftTeams.initialize();
        const hashParams: any = getHashParameters();
        if (hashParams["error"]) {
            // Authentication/authorization failed
            microsoftTeams.authentication.notifyFailure(hashParams["error"]);

        } else if (hashParams["id_token"]) {
            // Success
            microsoftTeams.authentication.notifySuccess();
            let search = history.location.search;
            let params = new URLSearchParams(search);
            let redirectUrl = params.get("redirect");
            if (redirectUrl !== 'null' || redirectUrl !== null || redirectUrl !== undefined || redirectUrl !== '') {
                console.log(redirectUrl);
                history.push(redirectUrl!);
            }
            else {
                history.push('/discover');
            }
        } else {
            // Unexpected condition: hash does not contain error or access_token parameter
            microsoftTeams.authentication.notifyFailure("UnexpectedFailure");
        }
    });

    return (<></>)
};

export default SignInSimpleEnd;