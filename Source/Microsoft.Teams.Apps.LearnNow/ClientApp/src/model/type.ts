// <copyright file="type.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

export interface IResourceDetail {
    id: string,
    title: string,
    description: string,
    subjectId?: string,
    subject: ISubject,
    grade: IGrade,
    gradeId?: string,
    imageUrl: string,
    linkUrl: string,
    attachmentUrl: string,
    fileType: number,
    createdOn: Date,
    updatedOn: Date,
    createdBy: string,
    updatedBy: string,
    resourceTag: IResourceTag[],
    resourceType: ResourceType,
    userDisplayName: string,
    voteCount?: number
    isLikedByUser?: boolean,
    checkItem?: boolean,
}

export enum RequestMode {
    create = "create",
    edit = "edit",
}

export enum ResourceType {
    PowerPoint = 1,
    Excel = 2,
    Word = 3,
    PDF = 4,
    WebLink = 5,
}

export interface IGrade {
    id?: string,
    gradeName: string,
    createdBy?: string,
    updatedBy?: string,
    createdOn?: string,
    updatedOn?: string,
}

export interface ISubject {
    id?: string,
    subjectName: string,
    createdBy?: string,
    updatedBy?: string,
    createdOn?: string,
    updatedOn?: string,
}

export interface IResourceTag {
    tagId: string
    tag: ITag
}

export interface ITag {
    id?: string,
    tagName: string,
    createdBy?: string,
    updatedBy?: string,
    createdOn?: string,
    updatedOn?: string,
}

export interface ICreatedBy {
    userId: string,
    displayName: string
}

/**
* This class is used to store the mapping of teams tab with learning module.
*/
export interface ITabConfiguration {
    id?: string,
    learningModuleId: string,
    teamId: string,
    channelId: string,
}

export interface ILearningModuleDetail {
    id: string
    title: string
    description: string
    subjectId?: string
    subject?: ISubject
    grade?: IGrade
    gradeId?: string
    imageUrl: string
    createdOn: Date
    updatedOn: Date
    createdBy: string
    updatedBy: string
    learningModuleVoteCount?: number;
    userDisplayName: string
    voteCount?: number;
    isLikedByUser?: boolean;
    learningModuleTag: ILearningModuleTag[],
    resourceCount?: number;
}

export interface ILearningModuleTag {
    tagId: string
    tag: ITag
}

export interface ILearningModule {
    id: string,
    title: string,
}

export interface IResourceModuleDetails {
    ResourceId: string;
    LearningModuleId: string;
}

export interface IModuleResourceViewModel {
    learningModule: ILearningModuleDetail,
    resources: IResourceDetail[],
}

export interface ILearningModuleVote {
    moduleId: string
    userId: string
}

export interface ITagValidationParameters {
    isExisting: boolean;
    isTagsCountValid: boolean;
}

export interface IModuleResourceViewModel {
    learningModule: ILearningModuleDetail,
    resources: IResourceDetail[],
}

export interface IResourceModuleDetails {
    ResourceId: string;
    LearningModuleId: string;
}

export interface IUserResource {
    userid?: string,
    resourceid: string
}

export interface IUserLearningModule {
    userid?: string,
    learningmoduleid: string,
}

export interface IFilterModel {
    subjectIds: string[],
    gradeIds: string[],
    tagIds: string[],
    createdByObjectIds: string[],
    searchText?: string;
}

export interface IUserLearningFilterModel {
    userObjectId?: string,
    searchText?: string,
    isSaved: boolean
}
export interface IFilterRequestModel {
    searchText?: string;
    resourceId?: string
}

export enum NotificationType {
    Success = 1,
    Failure = 2,
}

export enum PageType {
    Form = 1,
    Image = 2,
    Preview = 3,
}

export interface IUserRole {
    isAdmin: boolean,
    isTeacher: boolean
}

export interface IDropDownItem {
    key: string,
    header: string,
}

export interface ILearningModuleItem {
    id: string;
    grade?: any;
    imageUrl: string;
    subject?: any;
    title: string;
    description: string;
    isItemChecked?: boolean;
} 