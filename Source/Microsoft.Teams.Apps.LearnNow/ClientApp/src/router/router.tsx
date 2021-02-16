// <copyright file="router.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Suspense } from "react";
import { Route, Switch, Redirect, BrowserRouter } from "react-router-dom";
import "../i18n";
import ErrorPage from "../components/error-page";
import ConfigureAdminPage from "../components/configure-admin/configure-admin-page";
import AddGrade from "../components/configure-admin/add-grade";
import AddSubject from "../components/configure-admin/add-subject";
import AddTag from "../components/configure-admin/add-tag";
import EditGrade from "../components/configure-admin/edit-grade";
import EditSubject from "../components/configure-admin/edit-subject";
import EditTag from "../components/configure-admin/edit-tag";
import SignInPage from "../components/signin/signin";
import SignInSimpleStart from "../components/signin/signin-start";
import SignInSimpleEnd from "../components/signin/signin-end";
import ConfigureTabPreference from "../components/configurable-tabs/configure-tab-preference";
import ConfigurableTeamsTab from "../components/configurable-tabs/configurable-tab-wrapper-page";
import ResourceContent from "../components/resource-content/resource-content"
import PreviewContentDetail from "../components/preview-resource-content/preview-content-detail";
import LearningModulePreviewItems from "../components/learning-module-tab/learning-module-content-preview";
import LearningModuleItems from "../components/learning-module/learning-module-items";
import DiscoverTabMenu from "../components/discover-tab/discover-menu-wrapper-page";
import LearningModule from "../components/learning-module/learning-module";
import YourLearningTabMenu from "../components/your-learning-tab/your-learning-menu-wrapper-page";

export const AppRoute: React.FunctionComponent<{}> = () => {

    return (
        <Suspense fallback={<div className="container-div"><div className="container-subdiv"></div></div>}>
            <BrowserRouter>
                <Switch>
                    <Route exact path="/error" component={ErrorPage} />
                    <Route exact path="/discover" component={DiscoverTabMenu} />
                    <Route exact path="/configure" component={ConfigureAdminPage} />
                    <Route exact path="/add-grade" component={AddGrade} />
                    <Route exact path="/add-subject" component={AddSubject} />
                    <Route exact path="/add-tag" component={AddTag} />
                    <Route exact path="/edit-grade" component={EditGrade} />
                    <Route exact path="/edit-subject" component={EditSubject} />
                    <Route exact path="/edit-tag" component={EditTag} />
                    <Route exact path="/signin" component={SignInPage} />
                    <Route exact path="/signin-simple-start" component={SignInSimpleStart} />
                    <Route exact path="/signin-simple-end" component={SignInSimpleEnd} />
                    <Route exact path="/configurable-tab" component={ConfigureTabPreference} />
                    <Route exact path="/teams-tab" component={ConfigurableTeamsTab} />
                    <Route exact path="/resourcecontent" component={ResourceContent} />
                    <Route exact path="/previewcontent" component={PreviewContentDetail} />
                    <Route exact path="/learningmodulepreview" component={LearningModulePreviewItems} />
                    <Route exact path="/addlearningitems" component={LearningModuleItems} />
                    <Route exact path="/createmodule" component={LearningModule} />
                    <Route exact path="/your-learning" component={YourLearningTabMenu} />
                    <Route component={Redirect} />
                </Switch>
            </BrowserRouter>
        </Suspense>
    );
}