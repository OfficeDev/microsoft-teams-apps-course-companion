// <copyright file="preview-content-learning-module.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import * as React from "react";
import { Flex, Text, Button, ChevronStartIcon, Image } from "@fluentui/react-northstar";
import { ILearningModuleDetail, IDropDownItem } from "../../model/type";
import Tag from "../resource-content/tag";

import "../../styles/resource-content.css";

interface IPreviewContentState {
    isSaveButtonLoading: boolean,
    isSaveButtonDisabled: boolean,
}

interface IPreviewContentProps extends WithTranslation {
    selectedTags?: IDropDownItem[],
    resourceDetail: ILearningModuleDetail,
    showImage: boolean,
    isViewOnly: boolean,
    handleShareButtonClick: (event: any) => void,
    handlePreviewBackButtonClick: (event: any) => void,
}

/**
* Component for rendering learning module preview page.
*/
class PreviewContent extends React.Component<IPreviewContentProps, IPreviewContentState> {

    localize: TFunction;

    constructor(props: any) {
        super(props);
        this.localize = this.props.t;
        this.state = {
            isSaveButtonLoading: false,
            isSaveButtonDisabled: false,
        }
    }

    /**
    * Handle save button click.
    */
    private handleSaveButtonClick = (event: any) => {
        this.setState({ isSaveButtonDisabled: true, isSaveButtonLoading: true })
        this.props.handleShareButtonClick(event);
    }

    /**
    * Renders the component.
    */
    public render() {
        return (
            <div className="preview-container-tab">
                <div className="preview-content-main">
                    <div className="preview-sub-div">
                        <Flex>
                            <Text size="large" content={this.props.resourceDetail.title} weight="bold" />
                        </Flex>
                        <div className="subtitle-preview-padding">
                            <Text size="medium" content={this.props.resourceDetail.subject!.subjectName} weight="semibold" />,
                        <Text size="medium" content={this.props.resourceDetail.grade!.gradeName} className="grade-text-padding" />
                        </div>
                        <div>
                            <Image className="preview-card-image" fluid src={this.props.resourceDetail.imageUrl} />
                        </div>
                        <div className="preview-input-padding">
                            <Text size="small" content={this.props.resourceDetail.description} />
                        </div>
                        <div className="preview-input-padding">
                            {
                                this.props.selectedTags ?
                                    this.props.selectedTags.map((value: IDropDownItem, index) => {
                                        if (value) {
                                            return <Tag key={index} index={index} tagContent={value.header} showRemoveIcon={false} />
                                        }
                                    })
                                    :
                                    <></>
                            }
                        </div>
                    </div>
                </div>
                <div className="add-lm-tab-footer-preview">
                    <div>
                        <Flex space="between">
                            <Button icon={<ChevronStartIcon />} content={this.localize("backButtonText")} text onClick={this.props.handlePreviewBackButtonClick} className="back-button-lm-preview" />
                            <Flex.Item>
                                <Button className="next-button" content={this.localize("shareButtonText")} primary onClick={this.handleSaveButtonClick} loading={this.state.isSaveButtonLoading} disabled={this.state.isSaveButtonDisabled} />
                            </Flex.Item>
                        </Flex>
                    </div>
                </div>
            </div>
        );
    }
}
export default withTranslation()(PreviewContent);