// <copyright file="subject-tab.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import CommandBar from "./command-bar";
import SubjectTable from "./subject-table";
import { getAllSubjects, deleteSubjects } from "../../api/subject-api";
import * as microsoftTeams from "@microsoft/teams-js";
import { withRouter, RouteComponentProps } from 'react-router-dom';
import ErrorMessage from "../error-message";

import "../../styles/admin-configure-wrapper-page.css";

interface ISubjectData {
    userId: string,
    id: string,
    subjectName: string,
    userDisplayName: string,
    createdOn: string
}

interface ISubjectState {
    subjectData: ISubjectData[];
    userSelectedResponses: string[];
    filteredSubjectResponses: ISubjectData[];
    errorMessage: string;
}

/**
* Component for subject tab details.
*/
class SubjectTabPage extends React.Component<RouteComponentProps, ISubjectState> {
    history: any

    constructor(props: any) {
        super(props);

        this.state = {
            filteredSubjectResponses: [],
            subjectData: [],
            userSelectedResponses: [],
            errorMessage: "",
        }
        this.history = props.history;
    }

    /**
    * Used to initialize Microsoft Teams sdk
    */
    public async componentDidMount() {
        microsoftTeams.initialize();
        microsoftTeams.getContext((context) => {
            this.getSubjectList();
        });
    }

    /**
    * Calls API to get subject details from server
    */
    private getSubjectList = async () => {
        let allSubjectsResponse = await getAllSubjects(this.handleAuthenticationFailure);
        if (allSubjectsResponse.status === 200 && allSubjectsResponse.data) {
            this.setState({
                subjectData: allSubjectsResponse.data,
                filteredSubjectResponses: allSubjectsResponse.data
            });
        }
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
    * Triggered when user select checkbox. It stored user selected checkbox ids
    * @param {String} subjectId Id of the subject checkbox on which has clicked
    * @param {Boolean} isSelected Represents whether checkbox is selected or not
    */
    private onUserSubjectSelected = (subjectId: string, isSelected: boolean) => {
        if (isSelected) {
            let userSelectedResponses = this.state.userSelectedResponses;
            userSelectedResponses.push(subjectId);
            this.setState({
                userSelectedResponses: userSelectedResponses
            })
        }
        else {
            let filteredGradeResponses = this.state.userSelectedResponses.filter((addedResponseId) => {
                return addedResponseId !== subjectId;
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
            var filteredResponses = this.state.subjectData.filter((userResponse: ISubjectData) => {
                return userResponse.subjectName?.toLocaleUpperCase().includes(searchTextUpperCase) ||
                    userResponse.userDisplayName?.toLocaleUpperCase().includes(searchTextUpperCase) ||
                    userResponse.createdOn?.toLocaleUpperCase().includes(searchTextUpperCase);
            });
            this.setState({ filteredSubjectResponses: filteredResponses });
        } else {
            this.setState({ filteredSubjectResponses: this.state.subjectData });
        }
    }

    /**
    * Navigate to add new response page
    */
    private handleAddButtonClick = () => {
        this.history.push('/add-subject');
    }

    /**
    * Navigate to edit subject page
    */
    private handleEditButtonClick = () => {
        this.history.push(`/edit-subject?id=${this.state.userSelectedResponses[0]}`);
    }

    /**
    * Deletes selected user subjects
    */
    private handleDeleteButtonClick = async () => {
        let requestData: any[] = [];
        this.state.userSelectedResponses.forEach((value: string) => {

            requestData.push({
                id: value
            })
        })
        let deletionResult = await deleteSubjects(requestData);
        if (deletionResult.status === 200) {
            let userResponses = this.state.subjectData.filter((userResponse) => {
                return !this.state.userSelectedResponses.includes(userResponse.id.toString());
            });

            this.setState({
                subjectData: userResponses,
                filteredSubjectResponses: userResponses,
                userSelectedResponses: []
            })
        } else if (deletionResult.status === 547) { // if any subject entity is referred in other entity then API won't allow it to delete.
            this.setState({ errorMessage: "deleteConflictErrorMessage" });
        } else {
            this.setState({ errorMessage: "generalErrorMessage" });
        }
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
                    <SubjectTable showCheckbox={true} responsesData={this.state.filteredSubjectResponses} onCheckBoxChecked={this.onUserSubjectSelected} />
                </div>
                <ErrorMessage errorMessage={this.state.errorMessage} isGenericError={true} />
            </div>
        )
    }
}
export default withRouter(SubjectTabPage)