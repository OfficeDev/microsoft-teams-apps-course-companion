// <copyright file="learning-module-edit-preview.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from 'react';
import { Button, Flex, Text, Loader, ChevronStartIcon } from "@fluentui/react-northstar";
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import LearningModuleTablePreview from "../learning-module-tab/learning-module-content-preview-resources";
import { ILearningModuleDetail, IDropDownItem } from '../../model/type';
import Tag from "../resource-content/tag";

import "../../styles/resource-content.css";

interface ILearningModuleEditPreviewItemsState {
    userSelectedItem: string[];
    loading: boolean;
    learningModuleDetail: ILearningModuleDetail;
    isLearningModulePage: boolean,
    windowWidth: number,
    isSaveButtonLoading: boolean,
}

interface ILearningModuleEditPreviewItemsProps extends WithTranslation {
    handleShareButtonClick: () => void
    handlePreviewBackButtonClick: (event: any) => void,
    learningModuleDetails: ILearningModuleDetail;
    responsesData: any[]
    learningModuleTags: IDropDownItem[]
    getTagById: (tagId: string) => string,
}

/**
* This component is used in edit learning module task module for previewing the selected detail.
*/
class LearningModuleEditPreviewItems extends React.Component<ILearningModuleEditPreviewItemsProps, ILearningModuleEditPreviewItemsState> {
    localize: TFunction;

    constructor(props: any) {
        super(props);
        this.localize = this.props.t;
        this.state = {
            loading: false,
            userSelectedItem: [],
            learningModuleDetail: this.props.learningModuleDetails,
            isLearningModulePage: true,
            windowWidth: window.innerWidth,
            isSaveButtonLoading: false,
        };
    }

    public componentDidMount() {
        window.addEventListener("resize", this.setWindowWidth);
    }

    public componentWillUnmount() {
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
    * Handle save button click.
    * */
    private handleShareButtonClick = (event: any) => {
        this.setState({ isSaveButtonLoading: true })
        this.props.handleShareButtonClick();
    }

    /**
    * Renders the component
    */
    private showContent() {
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
                                <div className="search-container-edit-preview">
                                    <Flex gap="gap.small" >
                                        <Flex.Item size="50rem">
                                            <Flex>
                                                <div>
                                                    <Text className="subject-heading" content={this.props.learningModuleDetails.title} weight="semibold" />
                                                </div>
                                            </Flex>
                                        </Flex.Item>
                                    </Flex>
                                    <Flex>
                                        <Flex.Item>
                                            <Flex>
                                                <Text className="sub-detail" content={this.props.learningModuleDetails.subject!.subjectName + ', ' + this.props.learningModuleDetails.grade!.gradeName} />
                                            </Flex>
                                        </Flex.Item>
                                    </Flex>
                                    <Flex>
                                        <Flex.Item>
                                            <Flex>
                                                <Text className="sub-description" content={this.props.learningModuleDetails.description} />
                                            </Flex>
                                        </Flex.Item>
                                    </Flex>
                                    <div className="preview-input-padding">
                                        {
                                            this.props.learningModuleTags ?
                                                this.props.learningModuleTags.map((value: IDropDownItem, index) => {
                                                    if (value) {
                                                        return <Tag key={index} index={index} tagContent={value.header} showRemoveIcon={false} />
                                                    }
                                                })
                                                :
                                                <></>
                                        }
                                    </div>
                                </div>
                                <div >
                                    <LearningModuleTablePreview
                                        responsesData={this.props.responsesData}
                                        windowWidth={this.state.windowWidth}
                                    />
                                </div>
                            </div>
                            <div className="tab-footer-preview">
                                <div>
                                    <Flex space="between">
                                        <Button className="back-button-preview" icon={<ChevronStartIcon />} content={this.localize("backButtonText")} text onClick={this.props.handlePreviewBackButtonClick} />
                                        <Flex.Item>
                                            <Button className="next-button-preview" content={this.localize("shareButtonText")} primary onClick={this.handleShareButtonClick} loading={this.state.isSaveButtonLoading} disabled={this.state.isSaveButtonLoading} />
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

export default withTranslation()(LearningModuleEditPreviewItems);