// <copyright file="preview-content-detail.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Text, Image, LikeIcon, Loader } from "@fluentui/react-northstar";
import { TFunction } from "i18next";
import ShowAttachment from "../resource-content/show-attachment"
import { WithTranslation, withTranslation } from "react-i18next";
import { IResourceDetail, IResourceTag, NotificationType } from "../../model/type";
import { getResource, userDownVoteResource, userUpVoteResource } from '../../api/resource-api'
import { getDownloadUri } from "../../api/file-upload-download-api";
import { getFileName } from "../../helpers/helper";
import { Flex } from "@fluentui/react-northstar";
import Tag from "../resource-content/tag";
import "../../styles/resource-content.css";
import * as microsoftTeams from "@microsoft/teams-js";
import NotificationMessage from "../notification-message/notification-message";
import Resources from "../../constants/resources";

interface IPreviewContentDetailState {
    resourceDetail: IResourceDetail
    fileName: string,
    fileUrl: string
    loading: boolean
    subject: string;
    grade: string;
    showAttachment: boolean;
    isVoteDisabled: boolean;
    showLink: boolean;
    alertMessage: string;
    alertType: number;
    showAlert: boolean;
}

/**
* Component for showing resource content details.
*/
class PreviewContentDetail extends React.Component<WithTranslation, IPreviewContentDetailState> {

    localize: TFunction;
    telemetry?: any = null;
    userAADObjectId?: string | null = null;
    resourceId: string | null = null;
    history: any

    constructor(props: any) {
        super(props);
        this.localize = this.props.t;

        this.state = {
            resourceDetail: {} as IResourceDetail,
            loading: true,
            fileName: "",
            fileUrl: "",
            subject: "",
            grade: "",
            showAttachment: false,
            isVoteDisabled: false,
            showLink: false,
            alertMessage: "",
            alertType: 0,
            showAlert: false,
        }

        this.history = props.history;
        let search = this.history.location.search;
        let params = new URLSearchParams(search);
        this.resourceId = params.get("resourceId") ? params.get("resourceId") : "";
    }

    /**
    * Used to initialize microsoft teams sdk and get initial resource details.
    */
    public async componentDidMount() {
        microsoftTeams.initialize();
        microsoftTeams.getContext((context) => {
            this.userAADObjectId = context.userObjectId;
        });

        let fileName: string = "";
        let showAttachment: boolean = false;
        let showLink: boolean = false;

        const resourceDetailsResponse = await getResource(this.resourceId!);
        if (resourceDetailsResponse !== null && resourceDetailsResponse.data) {
            let resourceDetail: IResourceDetail = resourceDetailsResponse.data

            if (resourceDetail != null) {
                if (resourceDetail.attachmentUrl && resourceDetail.attachmentUrl !== "") {
                    fileName = getFileName(resourceDetail.attachmentUrl);
                    showAttachment = true;
                }
                if (resourceDetail.linkUrl && resourceDetail.linkUrl !== "") {
                    showLink = true;
                }

                let grade = resourceDetail.grade.gradeName;
                let subject = resourceDetail.subject.subjectName;

                this.setState({
                    resourceDetail: resourceDetail,
                    grade: grade,
                    subject: subject,
                    showAttachment: showAttachment,
                    fileName: fileName,
                    showLink: showLink,
                    loading: false
                });
            }
        }
        else {
            this.setState({ loading: false });
        }
    }

    /**
    *   Method to download file attachment.
    */
    private handleDownloadButtonClick = async () => {

        // Get blob URL for resource attachment.
        var blobUrlResponse = await getDownloadUri(this.state.resourceDetail?.id);
        if (blobUrlResponse) {
            let blobUrl = blobUrlResponse.data;
            this.setState({ fileUrl: blobUrl });
            window.location.href = blobUrl;
            this.showAlert(this.localize("downloadSuccessText"), NotificationType.Success);
        }
    }

    /**
    * Sets state for showing alert notification.
    * @param {String} content Notification message
    * @param {Boolean} type Boolean value indicating 1- Success 2- Error
    */
    private showAlert = (content: string, type: number) => {
        this.setState({ alertMessage: content, alertType: type, showAlert: true }, () => {
            setTimeout(() => {
                this.setState({ showAlert: false })
            }, Resources.alertTimeOut);
        });
    }

    /**
    * Sets state for hiding alert notification.
    */
    private hideAlert = () => {
        this.setState({ showAlert: false })
    }

    /**
    * Method gets invoked when user clicks on like.
    */
    private OnVoteClick = async (resourceId: string) => {
        this.setState({ isVoteDisabled: true })
        let resourceDetail = Object.assign({}, this.state.resourceDetail);
        let resourceLikedByUser = resourceDetail?.isLikedByUser;

        if (resourceLikedByUser) {
            let voteDeleteResponse = await userDownVoteResource(resourceId);
            if (voteDeleteResponse.status === 200) {
                resourceDetail.isLikedByUser = false;
                resourceDetail.voteCount = resourceDetail.voteCount ? resourceDetail.voteCount - 1 : resourceDetail.voteCount;
            }
        }
        else {
            let voteSaveResponse = await userUpVoteResource(resourceId);
            if (voteSaveResponse.status === 200 && voteSaveResponse.data) {
                resourceDetail.isLikedByUser = true;
                resourceDetail.voteCount = resourceDetail.voteCount! + 1;
            }
        }
        this.setState({ resourceDetail: resourceDetail, isVoteDisabled: false })
    }

    /**
    * Renders the component.
    */
    private renderPreviewContent() {
        let resourceDetails = this.state.resourceDetail;
        return (
            <div className="preview-container">
                <Flex>
                    <div className="preview-content-main">
                        <div className="preview-sub-div">
                            <div className="preview-notification"> <NotificationMessage
                                onClose={this.hideAlert}
                                showAlert={this.state.showAlert}
                                content={this.state.alertMessage}
                                notificationType={this.state.alertType}
                            />
                            </div>
                            <Flex>
                                <Text size="large" content={resourceDetails?.title} weight="bold" />
                            </Flex>
                            <div className="subtitle-preview-padding">
                                <Text size="medium" content={this.state.subject} weight="semibold" />,
                            <Text size="medium" content={this.state.grade} className="grade-text-padding" />
                            </div>
                            <div>
                                <Image className="preview-card-image" fluid src={resourceDetails?.imageUrl} />
                            </div>
                            <div className="preview-input-padding">
                                <Text size="small" content={resourceDetails?.description} />
                            </div>

                            <div className="preview-input-padding">
                                {
                                    resourceDetails.resourceTag ?
                                        resourceDetails.resourceTag.map((value: IResourceTag, index: number) => {
                                            if (value) {
                                                return <Tag key={index} index={index} tagContent={value?.tag?.tagName} showRemoveIcon={false} />
                                            }
                                        })
                                        :
                                        <></>
                                }
                            </div>
                            <div className="preview-input-padding-attachment">
                                <ShowAttachment fileName={this.state.fileName} showAttachment={this.state.showAttachment} isViewOnly={true} adjustWidth={true} handleFileDownload={this.handleDownloadButtonClick} />
                            </div>
                            <div className="preview-input-padding-link">
                                {this.state.showLink &&
                                    <div>
                                        <div>
                                            <Text size="small" content={this.localize('supportedDocLink')} />
                                        </div>
                                        <div className="link-truncate">
                                            <a href={resourceDetails?.linkUrl} target="_blank" title={resourceDetails?.linkUrl}>{resourceDetails?.linkUrl}</a>
                                        </div>
                                    </div>
                                }
                            </div>
                        </div>
                    </div>
                </Flex>
                <Flex>

                    <div className="tab-footer">
                        <div>
                            <Flex space="between">
                                <Flex.Item>
                                    <div></div>
                                </Flex.Item>
                                <Flex.Item push>
                                    {
                                        this.state.isVoteDisabled ?
                                            <Loader size="small" />
                                            :
                                            <>
                                                {resourceDetails?.isLikedByUser ? <LikeIcon outline={false} className="vote-icon-filled cursor-pointer" onClick={() => this.OnVoteClick(resourceDetails?.id)} disabled={this.state.isVoteDisabled} title={this.localize('likeButtonText')} size="larger" /> : <LikeIcon outline={true} className="vote-icon cursor-pointer" onClick={() => this.OnVoteClick(resourceDetails?.id)} disabled={this.state.isVoteDisabled} title={this.localize('likeButtonText')} size="larger" />}
                                            </>
                                    }
                                </Flex.Item>
                            </Flex>
                        </div>
                    </div>
                </Flex>
            </div>
        );
    }

    /**
  *  Renders settings layout or loader on UI depending upon data is fetched from storage.
  * */
    public render() {
        let contents = this.state.loading
            ? <p><Loader /></p>
            : this.renderPreviewContent();
        return (
            <div>
                {contents}
            </div>
        );
    }
}
export default withTranslation()(PreviewContentDetail);