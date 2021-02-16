// <copyright file="learning-module-content-preview.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from 'react';
import { Button, Flex, Text, Loader, ChevronStartIcon, Image, LikeIcon } from "@fluentui/react-northstar";
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import LearningModuleTablePreview from "./learning-module-content-preview-resources";
import { userDownVoteLearningModule, userUpVoteLearningModule, getLearningModule } from '../../api/learning-module-api';
import { ILearningModuleDetail, IResourceDetail, IModuleResourceViewModel, IResourceTag, ILearningModuleTag, NotificationType } from '../../model/type';
import ShowAttachment from '../resource-content/show-attachment';
import Tag from "../resource-content/tag";
import { getDownloadUri } from '../../api/file-upload-download-api';
import { getFileName } from "../../helpers/helper";
import "../../styles/learning-module-preview.css";
import "../../styles/resource-content.css";
import { userDownVoteResource, userUpVoteResource } from '../../api/resource-api';
import NotificationMessage from '../notification-message/notification-message';
import Resources from '../../constants/resources';
interface ILearningModulePreviewItemState {
    userSelectedItem: string[];
    moduleResources: IResourceDetail[];
    searchValue: string;
    subject?: string;
    grade?: string;
    loading: boolean;
    learningModuleDetail: ILearningModuleDetail;
    selectedResource: IResourceDetail;
    isLearningModulePage: boolean;
    isResourcePage: boolean;
    fileName: string;
    fileUrl: string;
    showAttachment: boolean;
    showModuleVoteLoader: boolean;
    showResourceVoteLoader: boolean;
    alertMessage: string;
    alertType: NotificationType;
    showAlert: boolean;
    windowWidth: number;
}

/**
* Component for rendering learning module preview task module.
*/
class LearningModulePreviewItems extends React.Component<WithTranslation, ILearningModulePreviewItemState> {
    localize: TFunction;
    history: any;
    learningModuleId: string | null = null;
    constructor(props: any) {
        super(props);
        this.localize = this.props.t;
        this.history = props.history;
        this.state = {
            windowWidth: window.innerWidth,
            loading: true,
            userSelectedItem: [],
            searchValue: "",
            moduleResources: [],
            subject: "",
            grade: "",
            learningModuleDetail: {} as ILearningModuleDetail,
            isLearningModulePage: true,
            isResourcePage: false,
            selectedResource: {} as IResourceDetail,
            fileName: "",
            fileUrl: "",
            showAttachment: false,
            showModuleVoteLoader: false,
            showResourceVoteLoader: false,
            alertMessage: "",
            alertType: 0,
            showAlert: false,
        };
        this.history = props.history;
        let search = this.history.location.search;
        let params = new URLSearchParams(search);
        this.learningModuleId = params.get("learningModuleId") ? params.get("learningModuleId") : "";
    }

    /**
    * Get learning module data when component is mounted
    */
    public async componentDidMount() {
        this.getLearningModuleDetail();
        window.addEventListener("resize", this.setWindowWidth);
    }

    public componentDidUnmount() {
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
    * Get learning module for given id from API
    */
    private getLearningModuleDetail = async () => {
        const learningModuleDetailResponse = await getLearningModule(this.learningModuleId!);
        if (learningModuleDetailResponse.status === 200 && learningModuleDetailResponse.data) {
            let learningModuleDetail: IModuleResourceViewModel = learningModuleDetailResponse.data;
            this.setState({
                subject: learningModuleDetail.learningModule.subject?.subjectName,
                grade: learningModuleDetail.learningModule.grade?.gradeName,
                learningModuleDetail: learningModuleDetail.learningModule,
                moduleResources: learningModuleDetail.resources,
                loading: false
            })
        } else {
            this.setState({ loading: false });
        }
    }

    /**
    * Method called when user clicks on any resource of learning module.
    * @param {IResourceDetail} resourceDetail selected resource details.
    */
    private handleResourceClick = (resourceDetail: IResourceDetail) => {
        if (resourceDetail.attachmentUrl && resourceDetail.attachmentUrl !== "") {
            let fileName = getFileName(resourceDetail.attachmentUrl);
            this.setState({ showAttachment: true, fileName: fileName, selectedResource: resourceDetail, isResourcePage: true, isLearningModulePage: false })
        } else {
            this.setState({ showAttachment: false, fileName: "", selectedResource: resourceDetail, isResourcePage: true, isLearningModulePage: false })
        }
    }

    /**
    * Method to download file attachment.
    */
    private handleDownloadButtonClick = async () => {
        let resourceId = this.state.selectedResource.id;
        // Get blob URL for resource attachment.
        let blobUrlResponse = await getDownloadUri(resourceId);
        if (blobUrlResponse.status === 200 && blobUrlResponse.data) {
            let blobUrl = blobUrlResponse.data;
            this.setState({ fileUrl: blobUrl });
            window.location.href = blobUrl;
            this.showAlert(this.localize("downloadSuccessText"), NotificationType.Success);
        }
    }

    /**
    * Sets state for showing alert notification.
    * @param {String} content Notification message
    * @param {Number} type NotificationType value indicating 1- Success 2- Error
    */
    private showAlert = (content: string, type: NotificationType) => {
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
    * Method gets invoked when user clicks on module like.
    */
    private onModuleVoteClick = async () => {
        this.setState({ showModuleVoteLoader: true })
        let learningModuleDetail = this.state.learningModuleDetail;
        let moduleLikedByUser = learningModuleDetail?.isLikedByUser;
        if (moduleLikedByUser) {
            let userDownVoteLearningModuleResponse = await userDownVoteLearningModule(learningModuleDetail.id);
            if (userDownVoteLearningModuleResponse.status === 200 && userDownVoteLearningModuleResponse.data) {
                learningModuleDetail.isLikedByUser = false;
            }
        } else {
            let userUpVoteLearningModuleResponse = await userUpVoteLearningModule(learningModuleDetail.id);
            if (userUpVoteLearningModuleResponse.status === 200 && userUpVoteLearningModuleResponse.data) {
                learningModuleDetail.isLikedByUser = true;
            }
        }
        this.setState({ learningModuleDetail: learningModuleDetail, showModuleVoteLoader: false })
    }

    /**
    * Method gets invoked when user clicks on resource like.
    */
    private onResourceVoteClick = async () => {
        let resourceId = this.state.selectedResource.id;
        let moduleResources = this.state.moduleResources.map((moduleResource: IResourceDetail) => ({ ...moduleResource }));
        let resourceIndex = moduleResources.findIndex((resource: IResourceDetail) => resource.id === resourceId);
        let resourceDetail = moduleResources[resourceIndex];
        let resourceLikedByUser = resourceDetail.isLikedByUser;
        this.setState({ showResourceVoteLoader: true })
        if (resourceLikedByUser) {
            let userDownVoteResourceResponse = await userDownVoteResource(resourceId);
            if (userDownVoteResourceResponse.status === 200 && userDownVoteResourceResponse.data) {
                resourceDetail.isLikedByUser = false;
            }
        } else {
            let userUpVoteResourceResponse = await userUpVoteResource(resourceId);
            if (userUpVoteResourceResponse.status === 200 && userUpVoteResourceResponse.data) {
                resourceDetail.isLikedByUser = true;
            }
        }

        moduleResources[resourceIndex] = resourceDetail;
        this.setState({ moduleResources: moduleResources, selectedResource: resourceDetail, showResourceVoteLoader: false });
    }


    /**
    * Handle back button click on preview page.
    */
    private handlePreviewBackButtonClick = () => {
        this.setState({ isLearningModulePage: true, isResourcePage: false })
    }

    /**
    * Renders content
    */
    private showContent = () => {
        if (this.state.loading) {
            return (
                <div className="container-div">
                    <div className="container-subdiv">
                        <div className="loader">
                            <Loader />
                        </div>
                    </div>
                </div>
            );
        } else {
            return (
                <div>
                    {
                        this.state.isLearningModulePage &&
                        <div>
                            <div className="add-module-container">
                                <div className="search-container">
                                    <Flex gap="gap.small" >
                                        <Flex.Item size="50rem">
                                            <Flex>
                                                <Text className="subject-heading" content={this.state.learningModuleDetail.title} weight="semibold" />
                                            </Flex>
                                        </Flex.Item>
                                    </Flex>
                                    <Flex>
                                        <Flex>
                                            <div className="subtitle-preview-padding">
                                                <Text size="medium" content={this.state.subject} weight="semibold" />,
                                                <Text size="medium" content={this.state.grade} className="grade-text-padding" />
                                            </div>
                                        </Flex>
                                    </Flex>
                                    <Flex>
                                        <Flex>
                                            <Text className="sub-description" content={this.state.learningModuleDetail.description} />
                                        </Flex>
                                    </Flex>
                                    <div className="preview-input-padding">
                                        {
                                            this.state.learningModuleDetail.learningModuleTag ?
                                                this.state.learningModuleDetail.learningModuleTag.map((value: ILearningModuleTag, index) => {
                                                    if (value) {
                                                        return <Tag key={index} index={index} tagContent={value.tag.tagName} showRemoveIcon={false} />
                                                    }
                                                })
                                                :
                                                <></>
                                        }
                                    </div>
                                </div>
                                <div>
                                    <LearningModuleTablePreview
                                        responsesData={this.state.moduleResources}
                                        handleResourceClick={this.handleResourceClick}
                                        windowWidth={this.state.windowWidth}
                                    />
                                </div>
                            </div>
                            <div className="footer-flex-lm-preview">
                                <Flex space="between">
                                    {
                                        this.state.showModuleVoteLoader ?
                                            <Loader size="small" />
                                            :
                                            <LikeIcon
                                                size="larger"
                                                title={this.localize('likeButtonText')}
                                                outline={!this.state.learningModuleDetail.isLikedByUser}
                                                className={`cursor-pointer +' '+ ${this.state.learningModuleDetail.isLikedByUser ? 'vote-icon-filled' : ''}`}
                                                onClick={this.onModuleVoteClick}
                                            />
                                    }
                                </Flex>
                            </div>
                        </div>
                    }
                    {
                        this.state.isResourcePage &&
                        <div>
                            <div className="preview-container">
                                <Flex>
                                    <div className="preview-content-main">
                                        <div className="preview-notification"> <NotificationMessage
                                            onClose={this.hideAlert}
                                            showAlert={this.state.showAlert}
                                            content={this.state.alertMessage}
                                            notificationType={this.state.alertType}
                                        />
                                        </div>
                                        <div className="preview-sub-div">
                                            <Flex>
                                                <Text size="large" content={this.state.selectedResource.title} weight="bold" />
                                            </Flex>
                                            <div className="subtitle-preview-padding">
                                                <Text size="medium" content={this.state.subject} weight="semibold" />,
                                                <Text size="medium" content={this.state.grade} className="grade-text-padding" />
                                            </div>
                                            <div>
                                                <Image className="preview-card-image" fluid src={this.state.selectedResource.imageUrl} />
                                            </div>
                                            <div className="preview-input-padding">
                                                <Text size="small" content={this.state.selectedResource.description} />
                                            </div>
                                            <div className="preview-input-padding">
                                                {
                                                    this.state.selectedResource.resourceTag ?
                                                        this.state.selectedResource.resourceTag.map((value: IResourceTag, index: number) => {
                                                            if (value) {
                                                                return <Tag key={index} index={index} tagContent={value.tag.tagName} showRemoveIcon={false} />
                                                            }
                                                        })
                                                        :
                                                        <></>
                                                }
                                            </div>
                                            <div className="preview-input-padding-attachment">
                                                <ShowAttachment
                                                    fileName={this.state.fileName}
                                                    showAttachment={this.state.showAttachment}
                                                    isViewOnly={true}
                                                    adjustWidth={true}
                                                    handleFileDownload={this.handleDownloadButtonClick} />
                                            </div>
                                            <div className="preview-input-padding-link">
                                                {this.state.selectedResource.linkUrl &&
                                                    <div>
                                                        <div>
                                                            <Text size="small" content={this.localize('supportedDocLink')} />
                                                        </div>
                                                        <div className="link-truncate">
                                                            <a href={this.state.selectedResource.linkUrl} target="_blank" title={this.state.selectedResource.linkUrl}>{this.state.selectedResource.linkUrl}</a>
                                                        </div>
                                                    </div>
                                                }
                                            </div>
                                        </div>
                                    </div>
                                </Flex>
                            </div>
                            <div>
                                <div className="tab-footer-preview">
                                    <Flex space="between">
                                        <Flex.Item>
                                            <Button
                                                icon={<ChevronStartIcon />}
                                                className="back-button-preview-resource"
                                                content={this.localize("backButtonText")}
                                                text onClick={this.handlePreviewBackButtonClick} />
                                        </Flex.Item>
                                        <Flex.Item push>
                                            {
                                                this.state.showResourceVoteLoader ?
                                                    <Loader size="small" />
                                                    :
                                                    <LikeIcon
                                                        size="larger"
                                                        title={this.localize('likeButtonText')}
                                                        outline={!this.state.selectedResource.isLikedByUser}
                                                        className={`cursor-pointer +' '+ ${this.state.selectedResource.isLikedByUser ? 'vote-icon-filled' : ""}`}
                                                        onClick={this.onResourceVoteClick}
                                                    />
                                            }
                                        </Flex.Item>
                                    </Flex>
                                </div>
                            </div>
                        </div>
                    }
                </div>
            )
        }
    }

    /**
    * Renders the component
    */
    public render(): JSX.Element {
        return (
            <>
                {this.showContent()}
            </>
        );
    }
}
export default withTranslation()(LearningModulePreviewItems);