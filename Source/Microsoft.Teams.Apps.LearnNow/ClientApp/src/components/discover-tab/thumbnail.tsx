﻿// <copyright file="thumbnail.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Image } from "@fluentui/react-northstar";
import { useTranslation } from 'react-i18next';

interface IThumbnailProps {
    isVisible: boolean;
    imageUrl?: string;
}

/**
* Component for showing image thumbnail on resource card.
*/
const Thumbnail: React.FunctionComponent<IThumbnailProps> = props => {
    const localize = useTranslation().t;
    if (props.isVisible) {
        return (
            <Image
                className="card-thumbnail"
                alt={localize("cardThumbnail")}
                src={props.imageUrl}
            />
        );
    }
    else {
        return (
            <></>
        );
    }
}

export default Thumbnail;