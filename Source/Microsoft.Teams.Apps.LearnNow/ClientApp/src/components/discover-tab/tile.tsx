// <copyright file="tile.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import { Flex, Image, Text, Dialog, Loader } from '@fluentui/react-northstar'
import * as React from 'react';
import { Button, Avatar, Popup } from '@fluentui/react-northstar'
import { MoreIcon, LikeIcon, AddIcon, EditIcon, TrashCanIcon, BookmarkIcon } from '@fluentui/react-icons-northstar'
import Tag from "../resource-content/tag";
import { IResourceDetail, IResourceTag, IUserRole } from '../../model/type';
import Thumbnail from "../discover-tab/thumbnail";
import { useTranslation } from "react-i18next";
import { getFileImageFromFileName } from '../../helpers/helper';

import "../../styles/tile.css";
import "../../styles/tags.css";

interface ITileProps {
    index: string;
    resourceDetails: IResourceDetail;
    handleEditClick?: (resourceId: string) => void;
    handleDeleteClick?: (resourceId: string) => void;
    handlePreviewClick: (resourceId: string) => void;
    handleVoteClick: (resourceId: string) => void;
    currentUserId: string;
    userRole: IUserRole;
    handleAddToLearningModuleClick?: (gradeId?: string, subjectId?: string, resourceId?: string) => void;
    handleAddToUserResourcesClick?: (resourceId: string) => void;
    handleRemoveFromUserResourcesClick?: (resourceId: string) => void;
    isCreatedByFilter?: boolean;
    isTeamsTab?: boolean;
}

/**
* Component for rendering tile for resource.
*/
const Tile: React.FunctionComponent<ITileProps> = props => {

    const localize = useTranslation().t;

    let [showResourceVoteLoader, setShowResourceVoteLoader] = React.useState(false);
    let [isPopUpOpen, setIsPopUpOpen] = React.useState(false);

    /**
    * Method gets called when user clicks on vote.
    */
    const handleResourceVoteClick = async () => {
        let resourceId = props.resourceDetails.id;
        setShowResourceVoteLoader(true);
        await props.handleVoteClick(resourceId)
        setShowResourceVoteLoader(false);
    }

    /**
    * Method gets called when popup item is clicked
    * @param {String} item Menu item name clicked by user.
    */
    const onPopUpItemClick = (item: string) => {
        setIsPopUpOpen(false);
        let id = props.resourceDetails.id;
        if (item === localize('addToUserList')) {
            props.handleAddToUserResourcesClick!(id);
        }
        else if (item === localize('removeFromUserList')) {
            props.handleRemoveFromUserResourcesClick!(id);
        }
        else if (item === localize('editResourceText')) {
            props.handleEditClick!(id);
        }
        else if (item === localize('deleteResourceText')) {
            props.handleDeleteClick!(id);
        }
        else if (item === localize('addLearningModuleText')) {
            props.handleAddToLearningModuleClick!(props.resourceDetails.gradeId, props.resourceDetails.subjectId, id);
        }
    }

    /**
    * Method gets called when popup open changes
    */
    const onOpenChange = (open: boolean) => {
        setIsPopUpOpen(open);
    }

    /**
    * Renders the component.
    */
    const renderTileContent = () => {
        let resourceDetail = props.resourceDetails;
        return (
            <div id={props.index.toString()} className="card-bg">
                <div onClick={() => props.handlePreviewClick(resourceDetail?.id)} className="cursor-pointer">
                    <Flex gap="gap.smaller" vAlign="center">
                        <Thumbnail isVisible={true} imageUrl={resourceDetail?.imageUrl} />
                    </Flex>
                    <div className="card-body">
                        <Flex gap="gap.smaller" column vAlign="start">
                            <Flex gap="gap.smaller" className="title-flex">
                            </Flex>
                            <Flex gap="gap.small" padding="padding.medium" className="tab-card-header">
                                <Image src={getFileImageFromFileName(resourceDetail?.attachmentUrl)} />
                                <Flex column>
                                    <Flex>
                                        <Text content={resourceDetail?.title} weight="bold" className="card-title-resource" title={resourceDetail?.title} />
                                    </Flex>
                                    <Flex>
                                        <Text content={resourceDetail?.subject?.subjectName?.toUpperCase()} className="card-subtitle-subject tile-text-overflow" title={resourceDetail?.subject?.subjectName?.toUpperCase()} />|
                                        <Text content={resourceDetail?.grade?.gradeName?.toUpperCase()} className="card-subtitle-grade tile-text-overflow" title={resourceDetail?.grade?.gradeName?.toUpperCase()} />
                                    </Flex>
                                </Flex>
                            </Flex>
                            <Flex className="content-flex" gap="gap.small">
                                <Text size="small" className="content-text" title={resourceDetail?.description} content={resourceDetail?.description} />
                            </Flex>
                        </Flex>
                    </div>
                </div>
                <div className="footer-flex">
                    <Flex className="tags-card">
                        {
                            resourceDetail?.resourceTag ?
                                resourceDetail?.resourceTag.map((value: IResourceTag, index) => {
                                    let tagName = value?.tag?.tagName
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
                                name={resourceDetail?.userDisplayName}
                            />
                            <Text content={resourceDetail?.userDisplayName} className="name-label" />
                        </Flex>
                        <Flex vAlign="center">
                            <Text content={resourceDetail?.voteCount} className="like-text" />
                            {
                                showResourceVoteLoader ?
                                    <Loader size="small" />
                                    :
                                    <LikeIcon
                                        size="medium"
                                        title={localize('likeButtonText')}
                                        outline={!props.resourceDetails.isLikedByUser}
                                        className={`cursor-pointer +' '+ ${props.resourceDetails.isLikedByUser ? 'vote-icon-filled' : ""}`}
                                        onClick={handleResourceVoteClick}
                                    />
                            }
                            {!props.isTeamsTab &&
                                <Popup
                                    align="start"
                                    open={isPopUpOpen}
                                    onOpenChange={(e, { open }: any) => onOpenChange(open)}
                                    content={
                                        <>
                                            {(props.userRole.isTeacher || props.userRole.isAdmin) && <p onClick={() => onPopUpItemClick(localize('addLearningModuleText'))} className="cursor-pointer"><AddIcon size="small" outline className="popup-list-icon" />{localize('addLearningModuleText')}</p>}
                                            {!props.isCreatedByFilter && <p onClick={() => onPopUpItemClick(localize('addToUserList'))} className="cursor-pointer"><BookmarkIcon outline size="small" className="popup-list-icon" />{localize('addToUserList')}</p>}
                                            {props.isCreatedByFilter && <p onClick={() => onPopUpItemClick(localize('removeFromUserList'))} className="cursor-pointer"><TrashCanIcon outline size="small" className="popup-list-icon" />{localize('removeFromUserList')}</p>}
                                            {((props.userRole.isTeacher && props.currentUserId === resourceDetail?.createdBy) || props.userRole.isAdmin) && <p onClick={() => onPopUpItemClick(localize('editResourceText'))} className="cursor-pointer"><EditIcon outline size="small" className="popup-list-icon" />{localize('editResourceText')}</p>}
                                            <Dialog
                                                className="delete-dialog-mobile"
                                                cancelButton={localize('deleteResourceCancelButtonText')}
                                                confirmButton={localize('deleteResourceConfirmButtonText')}
                                                header={localize('deleteResourceHeaderText')}
                                                content={localize('deleteResourceContentText')}
                                                onConfirm={() => onPopUpItemClick(localize('deleteResourceText'))}
                                                trigger={((props.userRole.isTeacher && props.currentUserId === resourceDetail?.createdBy) || props.userRole.isAdmin) ? <p className="cursor-pointer"><TrashCanIcon outline size="small" className="popup-list-icon" />{localize('deleteResourceText')}</p> : <></>}
                                            />
                                        </>
                                    }
                                    position="below"
                                    trigger={
                                        <Button icon={<MoreIcon />} iconOnly text title={localize('MoreMenuLabel')} />
                                    }
                                    className="more-pop-up"
                                />}
                        </Flex>
                    </Flex>
                </div>
            </div>
        )
    }

    /**
    * Renders the component.
    */
    return renderTileContent();
}

export default Tile

