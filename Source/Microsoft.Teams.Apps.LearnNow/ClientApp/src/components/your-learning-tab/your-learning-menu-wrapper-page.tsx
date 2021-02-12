// <copyright file="your-learning-menu-wrapper-page.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Menu, tabListBehavior, Dropdown, ShorthandCollection, MenuItemProps, MenuShorthandKinds, DropdownItemProps } from "@fluentui/react-northstar";
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import YourLearningResourcePage from "./your-learning-resource-page";
import YourLearningModulePage from "./your-learning-module-page";
import Resources from "../../constants/resources";

import "../../styles/discover-menu-wrapper-page.css";


interface IYourLearningTabMenuState {
    activeIndex: number;
    windowWidth: number;
}

class YourLearningTabMenu extends React.Component<WithTranslation, IYourLearningTabMenuState> {
    localize: TFunction;

    constructor(props: WithTranslation) {
        super(props);
        this.localize = this.props.t;
        this.state = {
            activeIndex: 0,
            windowWidth: window.innerWidth,
        }
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
    * Method gets invoked when user switch tab.
    * @param {Any} e component response data.
    * @param {Any} menuItemProps menu component response data.
    */
    private onMenuItemClick = (e: any, menuItemProps: any) => {
        this.setState({
            activeIndex: menuItemProps.activeIndex
        })
    }

    /**
    * Method gets invoked when user switch tab.
    * @param {Any} e component response data.
    * @param {Any} menuItemProps menu component response data.
    */
    private onDropDownClick = (e: any, menuItemProps: any) => {
        this.setState({
            activeIndex: menuItemProps.highlightedIndex
        })
    }

    /**
    * Renders the component
    */
    public render(): JSX.Element {

        const DiscoverMenuItems: ShorthandCollection<MenuItemProps, MenuShorthandKinds> = [
            {
                key: '0',
                content: this.localize('resourceLabel'),

            },
            {
                key: '1',
                content: this.state.windowWidth >= Resources.maxWidthForMobileView ? this.localize('learningModuleLabel') : this.localize('mobileLearningModuleLabel'),
            }
        ]

        const menuFilter: ShorthandCollection<DropdownItemProps> = [
            {
                header: this.localize('resourceLabel'),
                key: 0
            },
            {
                header: this.state.windowWidth >= Resources.maxWidthForMobileView ? this.localize('learningModuleLabel') : this.localize('mobileLearningModuleLabel'),
                key: 1
            },
        ];
        return (
            <>
                <div className="container-div-yourlearning">
                    {this.state.windowWidth > Resources.maxWidthForMobileView ?
                        <div className="container-subdiv-myprojects-yourlearning">
                            <Menu
                                defaultActiveIndex={0}
                                items={DiscoverMenuItems}
                                onActiveIndexChange={this.onMenuItemClick}
                                primary
                                accessibility={tabListBehavior}
                                className="Menu-item"
                            />
                        </div> :
                        <div className="container-subdiv-myprojects-yourlearning"> <Dropdown
                            inverted
                            activeSelectedIndex={0}
                            items={menuFilter}
                            defaultValue="Resource"
                            onChange={this.onDropDownClick}
                        /></div>}
                    <div className="tab-content">
                        {
                            this.state.activeIndex === 0
                                ? <YourLearningResourcePage />
                                : <YourLearningModulePage />
                        }
                    </div>
                </div>
            </>
        )
    }
}

export default withTranslation()(YourLearningTabMenu);