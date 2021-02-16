// <copyright file="configure-admin-page.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Loader, Menu, tabListBehavior } from "@fluentui/react-northstar";
import GradeTabPage from "./grade-tab";
import SubjectTabPage from "./subject-tab";
import TagsTabPage from "./tag-tab";
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";
import ErrorMessage from "../error-message";

import "../../styles/admin-configure-wrapper-page.css";
import { validateIfUserIsModerator } from "../../api/member-validation-api";

interface IConfigAdminState {
    activeIndex: number;
    isModerator: boolean;
    errorMessage: string;
    loading: boolean;
}

/**
* Parent Component for admin pages.
*/
class ConfigureAdminPage extends React.Component<WithTranslation, IConfigAdminState> {
    localize: TFunction;
    history: any;

    constructor(props) {
        super(props);
        this.localize = this.props.t;
        this.state = {
            activeIndex: 0,
            isModerator: false,
            errorMessage: "",
            loading: true,
        }

        this.history = props.history;
        this.validateIfUserIsModerator();
    }

    /**
    * Validate whether user is part of a moderator security group.
    */
    private validateIfUserIsModerator = async () => {
        const validateIfUserIsModeratorResponse = await validateIfUserIsModerator();
        if (validateIfUserIsModeratorResponse.status === 200 && validateIfUserIsModeratorResponse.data) {
            this.setState({ isModerator: true, loading: false });
        } else {
            this.setState({ isModerator: false, errorMessage: "shouldBePartOfModeratorGroupError", loading: false });
        }
    }

    /**
    * Renders the component
    * @param {Any} event Object which describes event occurred
    * @param {Any} menuItemDetail menu property details
    */
    private onMenuItemClick = (event: any, menuItemDetail: any) => {
        this.setState({
            activeIndex: menuItemDetail.activeIndex
        })
    }

    /**
    * Renders the component
    */
    public render(): JSX.Element {
        const menuItems = [
            {
                key: 'Grade',
                content: this.localize("gradeLabel"),
            },
            {
                key: 'Subject',
                content: this.localize("subjectLabel"),
            },
            {
                key: 'Tag',
                content: this.localize("tagLabel"),
            }
        ]

        return (
            <div>
                <div>
                    {
                        this.state.loading ? <div className="admin-configure-page-centre"><Loader /></div> :
                            this.state.isModerator ?
                                <div className="container-ui">
                                    <Menu
                                        className="admin-menu"
                                        defaultActiveIndex={0}
                                        items={menuItems}
                                        onActiveIndexChange={(e: any, props: any) => this.onMenuItemClick(e, props)}
                                        underlined
                                        primary
                                        accessibility={tabListBehavior}
                                    />
                                    {
                                        this.state.activeIndex === 0 ? <GradeTabPage /> : this.state.activeIndex === 1 ? <SubjectTabPage /> : <TagsTabPage />
                                    }
                                </div>
                                :
                                <div className="admin-configure-page-centre">
                                    <ErrorMessage errorMessage={this.state.errorMessage} isGenericError={true} />
                                </div>
                    }
                </div>
            </div>
        )
    }
}

export default withTranslation()(ConfigureAdminPage);