// <copyright file="preview-image-api.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import axios from "./axios-decorator";

/**
* Get images as per search input provided by user.
* @param searchText Search text entered by user for filtering images.
*/
export const previewImages = async (searchText: string): Promise<any> => {
    let url = `/api/image?searchText=${searchText}`;
    return await axios.get(url);
}