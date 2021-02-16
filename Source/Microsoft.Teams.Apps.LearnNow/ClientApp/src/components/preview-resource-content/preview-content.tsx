// <copyright file="preview-content.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as microsoftTeams from "@microsoft/teams-js";
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import * as React from "react";
import { Flex, Text, Button, ChevronStartIcon, Image } from "@fluentui/react-northstar";
import ShowAttachment from "../resource-content/show-attachment"
import Tag from "../resource-content/tag";
import { IResourceDetail, IResourceTag } from "../../model/type";

import "../../styles/resource-content.css";

interface IPreviewContentState {
    showAttachment: boolean
    isSaveButtonLoading: boolean,
    isSaveButtonDisabled: boolean,
}

interface IPreviewContentProps extends WithTranslation {
    resourceDetail: IResourceDetail,
    fileName: string,
    subject: string,
    grade: string,
    showAttachment: boolean,
    showImage: boolean,
    isViewOnly: boolean,
    getTagById: (tagId: string) => string,
    handleSaveButtonClick: () => void,
    handlePreviewBackButtonClick: (event: any) => void,
    resourceTags: IResourceTag[]
}

/**
* Component for rendering resource details preview page.
*/
class PreviewContent extends React.Component<IPreviewContentProps, IPreviewContentState> {

    localize: TFunction;
    theme?: string | null;

    constructor(props: any) {
        super(props);
        this.localize = this.props.t;
        this.state = {
            showAttachment: true,
            isSaveButtonLoading: false,
            isSaveButtonDisabled: false,
        }
    }
    public componentDidMount() {
        microsoftTeams.initialize();
        microsoftTeams.getContext((context) => {
            this.theme = context.theme!
        });
    }

    /**
    * Handle save button click.
    * */
    private handleSaveButtonClick = () => {
        this.setState({ isSaveButtonDisabled: true, isSaveButtonLoading: true })
        this.props.handleSaveButtonClick();
    }

    /**
    * Renders the component.
    */
    public render() {
        return (
            <div className="preview-container-tab">
                <Flex>
                    <div className="preview-content-main">
                        <div className="preview-sub-div">
                            <Flex>
                                <Text size="large" content={this.props.resourceDetail?.title} weight="bold" />
                            </Flex>
                            <div className="subtitle-preview-padding">
                                <Text size="medium" content={this.props.subject} weight="semibold" />,
                        <Text size="medium" content={this.props.grade} className="grade-text-padding" />
                            </div>
                            <div>
                                <Image className="preview-card-image" fluid src={this.props.resourceDetail?.imageUrl} />
                            </div>
                            <div className="preview-input-padding">
                                <Text size="small" content={this.props.resourceDetail?.description} />
                            </div>
                            <div className="preview-input-padding">
                                {
                                    this.props.resourceTags ?
                                        this.props.resourceTags?.map((value: IResourceTag, index: number) => {
                                            if (value) {
                                                return <Tag key={index} index={index} tagContent={this.props.getTagById(value.tagId)} showRemoveIcon={false} />
                                            }
                                        })
                                        :
                                        <></>
                                }
                            </div>
                            <div className="preview-input-padding-attachment">
                                <ShowAttachment fileName={this.props.fileName} showAttachment={this.props.showAttachment} isViewOnly={true} adjustWidth={false} />
                            </div>
                            <div className="preview-input-padding-link">
                                {this.props.resourceDetail?.linkUrl &&
                                    <div>
                                        <div>
                                            <Text size="small" content={this.localize('supportedDocLink')} />
                                        </div>
                                        <div className="link-truncate">
                                            <a href={this.props.resourceDetail?.linkUrl} target="_blank" title={this.props.resourceDetail?.linkUrl}>{this.props.resourceDetail?.linkUrl}</a>
                                        </div>
                                    </div>
                                }
                            </div>
                        </div>
                    </div>
                </Flex>
                <Flex>
                    <div className="tab-footer-preview-resource">
                        <Flex space="between">
                            <Flex.Item >
                                <Button icon={<ChevronStartIcon />} className="back-preview-button" content={this.localize("backButtonText")} text onClick={this.props.handlePreviewBackButtonClick} />
                            </Flex.Item>
                            <Flex.Item push>
                                <Button className="next-button" content={this.localize("shareButtonText")} primary onClick={this.handleSaveButtonClick} loading={this.state.isSaveButtonLoading} disabled={this.state.isSaveButtonDisabled} />
                            </Flex.Item>
                        </Flex>
                    </div>
                </Flex>
            </div>
        );
    }
}
export default withTranslation()(PreviewContent);