// <copyright file="select-preview-image.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from 'react';
import { Button, Flex, Text, Input, Loader, ChevronStartIcon } from "@fluentui/react-northstar";
import { SearchIcon } from "@fluentui/react-icons-northstar";
import { Grid, Image, gridBehavior } from '@fluentui/react-northstar';
import { previewImages } from '../../api/preview-image-api';
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import Resources, { ResourcesKeyCodes } from '../../constants/resources';
import "../../styles/image-preview.css";

interface IPreviewImageState {
    urls: Array<string>;
    searchText: string;
    imageArray: Array<string>;
    selectedImageUrl: string;
    loading: boolean;
    isImageSearched: boolean;
    defaultImageSearchText?: string;
}

interface ISelectImagePageProps extends WithTranslation {
    handleImageNextButtonClick: (event: any) => void,
    handleImageBackButtonClick: (event: any) => void,
    handleImageClick: (url: any) => void,
    setImageArray: (image: Array<any>) => void,
    imageArray: Array<any>,
    isImageNextButtonDisabled: boolean,
    defaultImageSearchText?: string;
    existingImage?: string;
    windowWidth: number;
}

class SelectImagePage extends React.Component<ISelectImagePageProps, IPreviewImageState> {
    localize: TFunction;

    constructor(props: any) {
        super(props);
        this.localize = this.props.t;
        this.state = {
            urls: [],
            searchText: "",
            imageArray: [],
            loading: false,
            selectedImageUrl: "",
            isImageSearched: false
        }

        // Default search
        this.filterImages(props.defaultImageSearchText);
    }

    /**
    * Set input value of the search text
    * @param  {String} searchText text to search
    */
    private setInputValue = (searchText: string) => {
        if (searchText.length === 0) {
            this.setState({ urls: [], searchText: searchText });
        }
        else {
            this.setState({ searchText: searchText })
        }
    }

    /**
   * Fetch filtered images as per the given parameters
   * @param  {String} searchText text to search
   * @param  {String} height height of the image
   * @param  {String} widht width of the image
   */
    private filterImages = async (searchText: string) => {
        this.setState({ loading: true, isImageSearched: true });

        let response = await previewImages(searchText);
        if (response.status === 200 && response.data && response.data.length) {
            this.setState({ urls: response.data }, async () => {
                let images = await this.getImages();
                this.setState({ loading: false, imageArray: images })
            });

        }
        else {
            if (this.props.setImageArray) {
                this.props.setImageArray([]);
            }
            this.setState({ imageArray: [], urls: [], loading: false });
        }
    }

    /**
    * Fetch filtered images on enter key press
    * @param  {any} event event
    */
    private onEnterKeyPress = async (event: any) => {
        if (event.keyCode === ResourcesKeyCodes.keyCodeEnter) {
            await this.filterImages(this.state.searchText);
        }
    }

    private handleImageClick = (url: any, event: any) => {
        this.props.handleImageClick(url);

        var imageElement = document.getElementById("image-preview-border");
        imageElement?.removeAttribute("id");
        event.target.id = "image-preview-border";
    }

    /**
   * Get images.
   */
    private getImages = async () => {
        let images = new Array<any>();

        if (this.props.existingImage) {
            images.push(
                <div tabIndex={-1} onClick={(event) => this.handleImageClick(this.props.existingImage, event)}>
                    <Image
                        fluid
                        src={this.props.existingImage}
                        id="image-preview-border"
                    />
                </div>
            )
        }

        await this.state.urls.forEach((url) => {
            images.push(
                <div onClick={(event) => this.handleImageClick(url, event)}>
                    <Image
                        fluid
                        src={url}
                        data-is-focusable="true"
                    />
                </div>)
        });
        if (this.props.setImageArray) {
            this.props.setImageArray(images);
        }
        return images;
    }

    /**
    * Show Message text in select image preview.
    */
    private showMessage = () => {
        if (this.state.isImageSearched && this.props.imageArray.length === 0) {
            return (<Text content={this.localize("noImagesText")} className="image-text" weight="bold" />)
        }
        if (!this.state.isImageSearched) {
            return (<div className="image-text" >
                <div className="no-image-text">
                    <Text content={this.localize("noImageText")} weight="semibold" />
                </div>
                <div className="select-image-text">
                    <Text content={this.localize("selectImageText")} />
                </div>
            </div>)
        }
    }

    /**
    * Show images
    */
    private showContent = () => {
        if (this.props.imageArray.length > 0) {
            return (
                <div className="tab-container">
                    <Grid accessibility={gridBehavior} columns={this.props.windowWidth >= Resources.maxWidthForMobileView ? Resources.fiveColumnGrid : Resources.threeColumnGrid} content={this.props.imageArray ? this.props.imageArray : this.state.imageArray} className="grid-ui" />
                    <br />
                </div>)
        } else {
            return (
                <div className="tab-container">
                    <div className="empty-image-container">
                        {this.showMessage()}
                    </div>
                </div>
            )
        }
    }

    /**
    * Method to show loader.
    */
    private showLoader = () => {
        return (
            <div className="tab-container">
                <p className="loader-image"><Loader /></p>
            </div>)
    }

    /**
    * Renders the component
    */
    public render(): JSX.Element {
        return (
            <div className="image-container-div">
                <div>
                    <Flex gap="gap.smaller" className="select-image-label">
                        <Text content={"*" + this.localize("searchImageText")} />
                    </Flex>
                    <Flex className="select-image-search">
                        <Input onKeyDown={(event: any) => this.onEnterKeyPress(event)} fluid icon={<SearchIcon outline onClick={(event: any) => this.filterImages(this.state.searchText)} key="search" className="search-icon" />} onChange={(event: any) => this.setInputValue(event.target.value)} placeholder={this.localize("selectImagePlaceHolder")} title={this.localize("selectImagePlaceHolder")} />
                    </Flex>
                </div>
                {this.state.loading ? this.showLoader() : this.showContent()}
                <div className="tab-footer-image">
                    <div>
                        <Flex space="between">
                            <Flex className="back-image-button">
                                <Button icon={<ChevronStartIcon />} content={this.localize("backButtonText")} text onClick={this.props.handleImageBackButtonClick} />
                            </Flex>
                            <Flex gap="gap.small" className="next-button">
                                <Button content={this.localize("previewButtonText")} primary onClick={this.props.handleImageNextButtonClick} disabled={this.props.isImageNextButtonDisabled} />
                            </Flex>
                        </Flex>
                    </div>
                </div>
            </div>
        )
    }
}
export default withTranslation()(SelectImagePage);