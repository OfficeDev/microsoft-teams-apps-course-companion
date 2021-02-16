// <copyright file="grade-tab.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import CommandBar from "./command-bar";
import GradeTable from "./grade-table";
import { getAllGrades, deleteGrades } from "../../api/grade-api";
import * as microsoftTeams from "@microsoft/teams-js";
import { withRouter, RouteComponentProps } from 'react-router-dom';
import ErrorMessage from "../error-message";

import "../../styles/admin-configure-wrapper-page.css";

interface IGradeData {
    userId: string,
    id: string,
    gradeName: string,
    userDisplayName: string,
    createdOn: string
}

interface IGradeState {
    gradeData: IGradeData[];
    userSelectedResponses: string[];
    filteredGradeResponses: IGradeData[];
    errorMessage: string;
}

/**
* Component for grade tab details.
*/
class GradeTabPage extends React.Component<RouteComponentProps, IGradeState> {
    history: any

    constructor(props: any) {
        super(props);
        this.history = props.history;

        this.state = {
            filteredGradeResponses: [],
            gradeData: [],
            userSelectedResponses: [],
            errorMessage: "",
        }
    }

    /**
    * Used to initialize Microsoft Teams sdk
    */
    public async componentDidMount() {
        microsoftTeams.initialize();
        microsoftTeams.getContext((context) => {
            this.getGradeList();
        });
    }

    /**
    * Deletes selected user grades
    */
    private handleDeleteButtonClick = async () => {
        let requestData: any[] = [];
        this.state.userSelectedResponses.forEach((value: string) => {

            requestData.push({
                id: value
            })
        })
        let deletionResult = await deleteGrades(requestData);
        if (deletionResult.status === 200) {
            let userResponses = this.state.gradeData.filter((userResponse) => {
                return !this.state.userSelectedResponses.includes(userResponse.id.toString());
            });

            this.setState({
                gradeData: userResponses,
                filteredGradeResponses: userResponses,
                userSelectedResponses: []
            })
        } else if (deletionResult.status === 547) { // if any grade entity is referred in other entity then API won't allow it to delete.
            this.setState({ errorMessage: "deleteConflictErrorMessage" });
        } else {
            this.setState({ errorMessage: "generalErrorMessage" });
        }
    }

    /**
    * Navigate to edit response page
    */
    private handleEditButtonClick = () => {
        this.history.push(`/edit-grade?id=${this.state.userSelectedResponses[0]}`);
    }

    /**
    * handle error occurred during authentication
    */
    private handleAuthenticationFailure = (error: string) => {
        // When the getAuthToken function returns a "resourceRequiresConsent" error, 
        // it means Azure AD needs the user's consent before issuing a token to the app. 
        // The following code redirects the user to the "Sign in" page where the user can grant the consent. 
        // Right now, the app redirects to the consent page for any error.
        console.error("Error from getAuthToken: ", error);
        this.history.push('/signin');
    }

    /**
    * Calls API to get grade details from server
    */
    private getGradeList = async () => {
        let allGradesResponse = await getAllGrades(this.handleAuthenticationFailure);
        if (allGradesResponse.status === 200 && allGradesResponse.data) {
            this.setState({
                gradeData: allGradesResponse.data,
                filteredGradeResponses: allGradesResponse.data
            });
        }
    }

    /**
    * Triggered when user select checkbox. It stored user selected checkbox ids
    * @param string gradeId Id of the grade checkbox on which has clicked
    * @param boolean isSelected Represents whether checkbox is selected or not
    */
    private onUserGradeSelected = (gradeId: string, isSelected: boolean) => {
        if (isSelected) {
            let userSelectedResponses = this.state.userSelectedResponses;
            userSelectedResponses.push(gradeId);
            this.setState({
                userSelectedResponses: userSelectedResponses
            })
        }
        else {
            let filteredGradeResponses = this.state.userSelectedResponses.filter((addedResponseId) => {
                return addedResponseId !== gradeId;
            });
            this.setState({
                userSelectedResponses: filteredGradeResponses
            })
        }
    }

    /**
    * Filters table as per search text entered by user
    * @param {String} searchText Search text entered by user
    */
    private handleSearch = (searchText: string) => {
        if (searchText) {
            var searchTextUpperCase = searchText.toLocaleUpperCase();
            var filteredResponses = this.state.gradeData.filter(function (userResponse) {
                return userResponse.gradeName?.toLocaleUpperCase().includes(searchTextUpperCase) ||
                    userResponse.userDisplayName?.toLocaleUpperCase().includes(searchTextUpperCase) ||
                    userResponse.createdOn?.toLocaleUpperCase().includes(searchTextUpperCase);
            });
            this.setState({ filteredGradeResponses: filteredResponses });
        }
        else {
            this.setState({ filteredGradeResponses: this.state.gradeData });
        }
    }

    /**
    * Navigate to add new response page.
    */
    private handleAddButtonClick = () => {
        this.history.push('/add-grade');
    }

    /**
    * Renders the component
    */
    public render(): JSX.Element {
        const isDeleteButtonEnabled = this.state.userSelectedResponses.length > 0;
        const isEditButtonEnabled = this.state.userSelectedResponses.length > 0 && this.state.userSelectedResponses.length < 2; // Enable delete button when only one response row is selected.

        return (
            <div className="admin-configure-page">
                <CommandBar
                    isDeleteEnable={isDeleteButtonEnabled}
                    isEditEnable={isEditButtonEnabled}
                    onAddButtonClick={this.handleAddButtonClick}
                    onDeleteButtonClick={this.handleDeleteButtonClick}
                    onEditButtonClick={this.handleEditButtonClick}
                    handleTableFilter={this.handleSearch}
                />
                <div className="table-cell-content">
                    <GradeTable showCheckbox={true} responsesData={this.state.filteredGradeResponses} onCheckBoxChecked={this.onUserGradeSelected} />
                </div>
                <ErrorMessage errorMessage={this.state.errorMessage} isGenericError={true} />
            </div>
        )
    }
}

export default withRouter(GradeTabPage)