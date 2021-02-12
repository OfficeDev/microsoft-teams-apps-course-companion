// <copyright file="discover-menu-wreapper-page.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Menu, tabListBehavior, Dropdown, ShorthandCollection, MenuItemProps, MenuShorthandKinds } from "@fluentui/react-northstar";
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import DiscoverPage from "./discover-wrapper-page";
import LearningModulePage from "../learning-module-tab/learning-module-wrapper-page";
import Resources from '../../constants/resources';

import "../../styles/discover-menu-wrapper-page.css";

interface IDiscoverTabMenuState {
    activeIndex: number;
    windowWidth: number;
}

/**
* Component for discover menu wrapper page.
*/
class DiscoverTabMenu extends React.Component<WithTranslation, IDiscoverTabMenuState> {
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
                key: 0,
                content: this.localize('resourceLabel'),

            },
            {
                key: 1,
                content: this.state.windowWidth >= Resources.maxWidthForMobileView ? this.localize('learningModuleLabel') : this.localize('mobileLearningModuleLabel'),
            }
        ]

        const menuFilter = [
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
                <div className="container-div-discover">
                    {this.state.windowWidth > Resources.maxWidthForMobileView ?
                        <div className="container-subdiv-myprojects-discover">
                            <Menu
                                defaultActiveIndex={0}
                                items={DiscoverMenuItems}
                                onActiveIndexChange={this.onMenuItemClick}
                                primary
                                accessibility={tabListBehavior}
                                className="Menu-item"
                            />
                        </div> :
                        <div className="container-subdiv-myprojects-discover">
                            <Dropdown
                                inverted
                                items={menuFilter}
                                defaultValue={menuFilter[0].header}
                                defaultHighlightedIndex={0}
                                onChange={this.onDropDownClick}
                            />
                        </div>
                    }
                    <div className="tab-content">
                        {
                            this.state.activeIndex === 0
                                ? <DiscoverPage />
                                : < LearningModulePage />
                        }
                    </div>
                </div>
            </>
        )
    }
}

export default withTranslation()(DiscoverTabMenu);