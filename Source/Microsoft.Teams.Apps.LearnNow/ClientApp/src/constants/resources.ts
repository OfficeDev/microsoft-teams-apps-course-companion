// <copyright file="resources.ts" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

export interface IPostType {
    name: string;
    id: number;
    color: string;
}

export default class Resources {

    // Themes
    public static readonly body: string = "body";
    public static readonly theme: string = "theme";
    public static readonly general: string = "default";
    public static readonly light: string = "light";
    public static readonly dark: string = "dark";
    public static readonly contrast: string = "contrast";

    // Admin configuration inputs 
    public static readonly gradeInputMaxLength: number = 200;
    public static readonly subjectInputMaxLength: number = 200;
    public static readonly tagInputMaxLength: number = 200;

    // Resource view mode
    public static readonly editResource: string = "edit request";
    public static readonly addResource: string = "add request";

    // create resource validations
    public static readonly tagMaxLength: number = 20;
    public static readonly tagsMaxCount: number = 3;
    public static readonly tagsMaxCountPreferences: number = 5;
    public static readonly titleMaxLength: number = 75;
    public static readonly descriptionMaxLength: number = 300;
    public static readonly linkMaxLength: number = 400;
    public static readonly fileSizeMaxAllowed: number = 4194304;

    //Text truncate
    public static readonly titleMaxCardLength: number = 27;

    //Task Module
    public static readonly taskModuleHeight: number = 600;
    public static readonly taskModuleWidth: number = 600;

    // Your learning 
    public static readonly yourLearningHeight: string = "85vh";

    public static readonly successFlag = "success";
    public static readonly errorFlag = "error";

    public static readonly recordsToLoad = 10;
    public static readonly maxSelectedFilters = 10;

    // KeyCodes
    public static readonly keyCodeEnter: number = 13;
    public static readonly keyCodeSpace: number = 32;

    // Average network timeout in milliseconds.
    public static readonly axiosDefaultTimeout: number = 10000;
    public static readonly alertTimeOut: number = 4000;

    public static readonly lazyLoadPerPagePostCount: number = 50;
    public static readonly urlValidationRegEx: RegExp = /^https:\/\/(www\.)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$/;

    // The max window width up to which mobile view to be active.
    public static readonly maxWidthForMobileView: number = 600;
    public static readonly threeColumnGrid: number = 3;
    public static readonly oneColumnGrid: number = 1;
    public static readonly fiveColumnGrid: number = 5;

    public static readonly resourceEntityType: string = "resource";
    public static readonly learningModuleEntityType: string = "learningmodule";

    public static readonly maximumCreatorRecordsCount: number = 50;
    public static readonly randomClicksTimeOut: number = 700;
}

export class ResourcesKeyCodes {
    // KeyCodes
    public static readonly keyCodeEnter: number = 13;
    public static readonly keyCodeSpace: number = 32;
    public static readonly alertTimeOut: number = 4000;
}