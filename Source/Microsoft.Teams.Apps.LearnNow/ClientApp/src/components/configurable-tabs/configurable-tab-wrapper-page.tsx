// <copyright file="configurable-tab-wrapper-page.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from 'react';
import * as microsoftTeams from "@microsoft/teams-js";
import { Loader, Grid, gridBehavior } from '@fluentui/react-northstar'
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import { IResourceDetail } from "../../model/type";
import { userUpVoteResource, userDownVoteResource, getResourcesForModule, getResource } from '../../api/resource-api'
import Tile from "../discover-tab/tile"
import NoPostAddedPage from "./no-post-added-page"
import Resources from '../../constants/resources';
import { getTabConfiguration } from '../../api/tab-configuration-api';

import "../../styles/site.css";
import "../../styles/tile.css";

interface IConfigurableTeamsPageState {
    windowWidth: number;
    loader: boolean;
    allResources: IResourceDetail[];
    loading: boolean;
    isValidUser: boolean;
    clickedResourceId: string;
}

/**
* Teams tab page component used to render pinned learning module tab page.
*/
class ConfigurableTeamsPage extends React.Component<WithTranslation, IConfigurableTeamsPageState> {

    localize: TFunction;
    userAADObjectId?: string | null = null;
    botId: string = "";
    tabId?: string | null = null;
    teamId: string;
    groupId: string | null = null;
    learningModuleId?: string | null = null;

    constructor(props: any) {
        super(props);
        this.localize = this.props.t;
        this.teamId = "";
        this.state = {
            windowWidth: window.innerWidth,
            loader: false,
            allResources: [],
            loading: true,
            isValidUser: false,
            clickedResourceId: "",
        }
    }

    public async componentDidMount() {
        microsoftTeams.initialize();
        microsoftTeams.getContext((context) => {
            this.userAADObjectId = context.userObjectId!
            this.tabId = context.entityId!;
            this.groupId = context.groupId!;

            if (this.tabId) {
                setTimeout(async () => {
                    let response = await getTabConfiguration(this.tabId!, this.groupId!);
                    if (response.status === 200 && response.data) {
                        this.learningModuleId = response.data.learningModuleId;
                        this.getAllResources();
                    }
                }, Resources.alertTimeOut);
            }
        });
        window.addEventListener("resize", this.setWindowWidth);
    }

    public componentWillUnmount() {
        window.removeEventListener('resize', this.setWindowWidth);
    }

    /**
    * Get window width real time
    */
    private setWindowWidth = () => {
        if (window.innerWidth !== this.state.windowWidth) {
            this.setState({ windowWidth: window.innerWidth });
        }
    };

    /**
    * Navigate to preview resource content task module.
    * @param resourceId resource unique identifier.
    */
    private handlePreviewClick = (resourceId: string) => {
        let appBaseUrl = window.location.origin;
        this.setState({ clickedResourceId: resourceId });
        microsoftTeams.tasks.startTask({
            completionBotId: this.botId,
            title: this.localize('previewContentTaskModuleHeaderText'),
            height: Resources.taskModuleHeight,
            width: Resources.taskModuleWidth,
            url: `${appBaseUrl}/previewcontent?viewMode=1&resourceId=${resourceId}`,
            fallbackUrl: `${appBaseUrl}/previewcontent?viewMode=1&resourceId=${resourceId}`,
        }, this.previewClickSubmitHandler);
    }

    /**
    * Preview resource content task module handler.
    */
    private previewClickSubmitHandler = async () => {
        let resourceId = this.state.clickedResourceId;
        const resourceDetailsResponse = await getResource(resourceId);
        if (resourceDetailsResponse !== null && resourceDetailsResponse.data) {
            let resourceDetail: IResourceDetail = resourceDetailsResponse.data;
            let allResources = this.state.allResources.map((resource: IResourceDetail) => resource.id === resourceId ? resourceDetail : resource);
            this.setState({ allResources: allResources });
        }
    };

    /**
    * Get all resource based on current page count. 
    * @param pageCount Page count to get resource.
    */
    private getAllResources = async () => {
        const allResourcesResponse = await getResourcesForModule(this.learningModuleId!);
        if (allResourcesResponse.status === 200 && allResourcesResponse.data) {
            let existingResources = [...this.state.allResources];
            allResourcesResponse.data.forEach((resource: IResourceDetail) => {
                existingResources.push(resource);
            });
            this.setState({ allResources: existingResources, loading: false })
        }
    }

    /**
    * Method gets invoked when user clicks on like.
    * @param {String} resourceId resource unique identifier.
    */
    private handleVoteClick = async (resourceId: string) => {
        let allResources = this.state.allResources.map((resource: IResourceDetail) => ({ ...resource }));
        let resourceIndex = allResources.findIndex((resource: IResourceDetail) => resource.id === resourceId);
        let resourceDetail = allResources[resourceIndex];
        let resourceLikedByUser = resourceDetail.isLikedByUser;

        if (resourceLikedByUser) {
            let userDownVoteResourceResponse = await userDownVoteResource(resourceDetail.id);
            if (userDownVoteResourceResponse.status === 200 && userDownVoteResourceResponse.data) {
                resourceDetail.isLikedByUser = false;
                resourceDetail.voteCount = resourceDetail.voteCount ? resourceDetail.voteCount - 1 : resourceDetail.voteCount;
            }
        } else {
            let userUpVoteResourceResponse = await userUpVoteResource(resourceDetail.id);
            if (userUpVoteResourceResponse.status === 200 && userUpVoteResourceResponse.data) {
                resourceDetail.isLikedByUser = true;
                resourceDetail.voteCount = resourceDetail.voteCount! + 1;
            }
        }
        allResources[resourceIndex] = resourceDetail;
        this.setState({ allResources: allResources });
    }

    /**
    * Renders tab content.
    */
    private renderTabContent() {
        // Cards component array to be rendered on grid.
        const tiles = this.state.allResources.map((resource: IResourceDetail) => (<Tile index={resource.id}
            resourceDetails={resource}
            handlePreviewClick={this.handlePreviewClick}
            handleVoteClick={this.handleVoteClick}
            currentUserId={this.userAADObjectId!}
            userRole={{ isTeacher: false, isAdmin: false }}
            isTeamsTab={true}
        />));

        return (
            <div className="container-div">
                <div className="container-subdiv-cardview">
                    <div className="container-fluid-overriden config-tab-scroll">
                        {
                            tiles.length ? <Grid columns={this.state.windowWidth > Resources.maxWidthForMobileView ? Resources.threeColumnGrid : Resources.oneColumnGrid}
                                accessibility={gridBehavior}
                                className="tile-render"
                                content={tiles}>
                            </Grid> : <NoPostAddedPage />
                        }
                    </div>
                </div>
            </div>
        )
    }

    /**
    * Renders the component.
    */
    public render() {
        let contents = this.state.loading
            ? <p><Loader /></p>
            : this.renderTabContent();
        return (
            <div>
                {contents}
            </div>
        );
    }
}

export default withTranslation()(ConfigurableTeamsPage)