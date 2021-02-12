// <copyright file="learning-module-tile.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import { Flex, Text, Dialog } from '@fluentui/react-northstar'
import * as React from 'react';
import { Button, Avatar, Popup, Loader } from '@fluentui/react-northstar'
import { MoreIcon, LikeIcon, EditIcon, TrashCanIcon, BookmarkIcon } from '@fluentui/react-icons-northstar'
import { ILearningModuleDetail, ILearningModuleTag, IUserRole } from '../../model/type';
import { TFunction } from 'i18next';
import { WithTranslation, withTranslation } from 'react-i18next';
import Thumbnail from "../discover-tab/thumbnail";
import Tag from "../resource-content/tag";

import "./learning-module-tile.css";
import "../../styles/tags.css";

interface ILearningModuleTileProps extends WithTranslation {
    index: string,
    learningModuleDetails: ILearningModuleDetail,
    handleEditClick: (learningModuleId: string) => void;
    handleDeleteClick: (learningModuleId: string) => void;
    handlePreviewClick: (learningModuleId: string) => void;
    handleVoteClick: (learningModuleId: string) => void;
    currentUserId: string;
    userRole: IUserRole;
    handleAddToUserModuleClick: (resourceId: string) => void;
    handleRemoveFromUserModuleClick?: (resourceId: string) => void;
    isPrivateListTab?: boolean;
}
interface ILearningModuleTileState {
    showModuleVoteLoader: boolean,
    isPopUpOpen: boolean;
}
/**
* Component for rendering learning module tile.
*/
class LearningModuleTile extends React.Component<ILearningModuleTileProps, ILearningModuleTileState> {
    localize: TFunction;
    constructor(props: any) {
        super(props);
        this.localize = this.props.t;
        this.state = {
            showModuleVoteLoader: false,
            isPopUpOpen: false
        }
    }

    /**
    * Method to handle popup menu item click.
    * @param {String} item Menu item name clicked by user.
    */
    private onPopUpItemClick = (item: string) => {
        this.setState({ isPopUpOpen: false });
        let id = this.props.learningModuleDetails.id;
        if (item === this.localize('editResourceText')) {
            this.props.handleEditClick(id);
        }
        else if (item === this.localize('addToUserList')) {
            this.props.handleAddToUserModuleClick(id);
        }
        else if (item === this.localize('removeFromUserList')) {
            this.props.handleRemoveFromUserModuleClick!(id);
        }
        else if (item === this.localize('deleteResourceText')) {
            this.props.handleDeleteClick(id);
        }
    }

    /**
    * Method gets called when user clicks on vote.
    */
    private OnModuleVoteClick = async () => {
        let moduleId = this.props.learningModuleDetails.id;
        this.setState({ showModuleVoteLoader: true });
        await this.props.handleVoteClick(moduleId);
        this.setState({ showModuleVoteLoader: false });
    }

    /**
    * Method gets called when popup open changes
    */
    private onOpenChange = (open: boolean) => {
        this.setState({ isPopUpOpen: open });
    }

    /**
    * Render tile details for learning module.
    */
    private renderTileContent = () => {
        let learningModuleDetail = this.props.learningModuleDetails;
        return (
            <div id={this.props.index.toString()} className="card-bg">
                <div onClick={() => this.props.handlePreviewClick(learningModuleDetail.id)} className="cursor-pointer">
                    <Flex gap="gap.smaller" vAlign="center">
                        <Thumbnail isVisible={true} imageUrl={learningModuleDetail.imageUrl} />
                    </Flex>
                    <div className="card-body">
                        <Flex gap="gap.smaller" column vAlign="start">
                            <Flex column className="tab-card-header">
                                <Flex>
                                    <Text content={learningModuleDetail.title} weight="bold" className="card-title-lm" title={learningModuleDetail.title} />
                                </Flex>
                                <Flex>
                                    <Text content={learningModuleDetail.subject?.subjectName.toUpperCase()} className="card-subtitle-subject tile-text-overflow" title={learningModuleDetail.subject?.subjectName.toUpperCase()} />|
                                    <Text content={learningModuleDetail.grade?.gradeName.toUpperCase()} className="card-subtitle-grade tile-text-overflow" title={learningModuleDetail.grade?.gradeName.toUpperCase()} />
                                    <Flex.Item push>
                                        <Flex className="card-pill">
                                            <Text content={this.localize("moduleLabel")} weight="bold" className="card-subtitle-module-bold" />

                                            {learningModuleDetail?.resourceCount! <= 1 ?
                                                <Text content={this.localize("learningModuleItemText", { numberOfResources: learningModuleDetail?.resourceCount })} className="card-subtitle-module-light" />
                                                :
                                                <Text content={this.localize("learningModuleItemsText", { numberOfResources: learningModuleDetail?.resourceCount })} className="card-subtitle-module-light" />
                                            }
                                        </Flex>
                                    </Flex.Item>
                                </Flex>
                            </Flex>
                            <Flex className="content-flex" gap="gap.small">
                                <Text size="small" className="content-text-lm card-description-lm" title={learningModuleDetail.description} content={learningModuleDetail.description} />
                            </Flex>
                        </Flex>
                    </div>
                </div>
                <div className="footer-flex">
                    <Flex className="tags-card">
                        {
                            learningModuleDetail.learningModuleTag ?
                                learningModuleDetail.learningModuleTag.map((value: ILearningModuleTag, index) => {
                                    let tagName = value.tag?.tagName
                                    if (value) {
                                        return <Tag key={index} index={index} tagContent={tagName} showRemoveIcon={false} />
                                    }
                                })
                                : <div></div>
                        }
                    </Flex>
                    <Flex space="between">
                        <Flex>
                            <Avatar
                                name={learningModuleDetail.userDisplayName}
                            />
                            <Text content={learningModuleDetail.userDisplayName} className="name-label" />
                        </Flex>
                        <Flex vAlign="center">
                            <Text content={learningModuleDetail.voteCount} className="like-text" />
                            {
                                this.state.showModuleVoteLoader ?
                                    <Loader size="small" />
                                    :
                                    <LikeIcon
                                        size="small"
                                        title={this.localize('likeButtonText')}
                                        outline={!learningModuleDetail.isLikedByUser}
                                        className={`cursor-pointer +' '+ ${learningModuleDetail.isLikedByUser ? 'vote-icon-filled' : ''}`}
                                        onClick={this.OnModuleVoteClick}
                                    />
                            }
                            <Popup
                                align="start"
                                open={this.state.isPopUpOpen}
                                onOpenChange={(e, { open }: any) => this.onOpenChange(open)}
                                content={
                                    <>
                                        {!this.props.isPrivateListTab && <p onClick={() => this.onPopUpItemClick(this.localize('addToUserList'))} className="cursor-pointer"><BookmarkIcon outline size="small" className="popup-list-icon" />{this.localize('addToUserList')}</p>}
                                        {this.props.isPrivateListTab && <p onClick={() => this.onPopUpItemClick(this.localize('removeFromUserList'))} className="cursor-pointer"><TrashCanIcon outline size="small" className="popup-list-icon" />{this.localize('removeFromUserList')}</p>}
                                        {((this.props.userRole.isTeacher && this.props.currentUserId === learningModuleDetail?.createdBy) || this.props.userRole.isAdmin) && <p onClick={() => this.onPopUpItemClick(this.localize('editResourceText'))} className="cursor-pointer"><EditIcon size="small" outline className="popup-list-icon" />{this.localize('editResourceText')}</p>}
                                        <Dialog
                                            className="delete-dialog-mobile"
                                            cancelButton={this.localize('deleteResourceCancelButtonText')}
                                            confirmButton={this.localize('deleteResourceConfirmButtonText')}
                                            header={this.localize('deleteModuleHeaderText')}
                                            content={this.localize('deleteResourceContentText')}
                                            onConfirm={() => this.onPopUpItemClick(this.localize('deleteResourceText'))}
                                            trigger={((this.props.userRole.isTeacher && this.props.currentUserId === learningModuleDetail?.createdBy) || this.props.userRole.isAdmin) ? <p className="cursor-pointer"><TrashCanIcon outline size="small" className="popup-list-icon" />{this.localize('deleteResourceText')}</p> : <></>}
                                        />
                                    </>
                                }
                                position="below"
                                trigger={
                                    <Button icon={<MoreIcon />} iconOnly text title={this.localize('MoreMenuLabel')} />
                                }
                                className="more-pop-up"
                            />
                        </Flex>
                    </Flex>
                </div>
            </div>
        );
    }

    /**
    * Renders the component.
    */
    public render() {
        return (
            <>
                {this.renderTileContent()}
            </>
        );
    }
}
export default withTranslation()(LearningModuleTile)