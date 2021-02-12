// <copyright file="tag-tab.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import CommandBar from "./command-bar";
import TagTable from "./tag-table";
import { getAllTags, deleteTags } from "../../api/tag-api";
import * as microsoftTeams from "@microsoft/teams-js";
import { withRouter, RouteComponentProps } from 'react-router-dom';
import ErrorMessage from "../error-message";

import "../../styles/admin-configure-wrapper-page.css";

interface ITagData {
	userId: string,
	id: string,
	tagName: string,
	userDisplayName: string,
	updatedOn: string
}

interface ITagState {
	tagData: ITagData[];
	userSelectedResponses: string[];
	filteredTagResponses: ITagData[];
	errorMessage: string;
}

/**
* Component for tag tab details.
*/
class TagsTabPage extends React.Component<RouteComponentProps, ITagState> {
	history: any
	constructor(props: any) {
		super(props);

		this.state = {
			filteredTagResponses: [],
			tagData: [],
			userSelectedResponses: [],
			errorMessage: "",
		}
		this.history = props.history
	}

	/**
	* Used to initialize Microsoft Teams sdk
	*/
	public async componentDidMount() {
		microsoftTeams.initialize();
		microsoftTeams.getContext((context) => {
			this.getTagList();
		});
	}

	/**
	* Calls API to get tag details from server
	*/
	private async getTagList() {
		let response = await getAllTags(this.handleAuthenticationFailure);
		if (response.status === 200 && response.data) {
			this.setState({
				tagData: response.data,
				filteredTagResponses: response.data
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
	* @param string tagId Id of the tag checkbox on which has clicked
	* @param boolean isSelected Represents whether checkbox is selected or not
	*/
	private onUserTagSelected = (tagId: string, isSelected: boolean) => {
		if (isSelected) {
			let userSelectedResponses = this.state.userSelectedResponses;
			userSelectedResponses.push(tagId);
			this.setState({
				userSelectedResponses: userSelectedResponses
			})
		}
		else {
			let filteredGradeResponses = this.state.userSelectedResponses.filter((addedResponseId) => {
				return addedResponseId !== tagId;
			});
			this.setState({
				userSelectedResponses: filteredGradeResponses
			})
		}
	}

	/**
	*Filters table as per search text entered by user
	*@param {String} searchText Search text entered by user
	*/
	private handleSearch = (searchText: string) => {
		if (searchText) {
			var searchTextUpperCase = searchText.toLocaleUpperCase();
			var filteredResponses = this.state.tagData.filter(function (userResponse) {
				return userResponse.tagName.toLocaleUpperCase().includes(searchTextUpperCase) ||
					userResponse.userDisplayName.toLocaleUpperCase().includes(searchTextUpperCase) ||
					userResponse.updatedOn.toLocaleUpperCase().includes(searchTextUpperCase);
			});
			this.setState({ filteredTagResponses: filteredResponses });
		}
		else {
			this.setState({ filteredTagResponses: this.state.tagData });
		}
	}

	/**
	*Navigate to add new response page
	*/
	private handleAddButtonClick = () => {
		this.history.push('/add-tag');
	}

	/**
	*Navigate to edit subject page
	*/
	private handleEditButtonClick = () => {
		this.history.push(`/edit-tag?id=${this.state.userSelectedResponses[0]}`);
	}

	/**
	*Deletes selected user responses
	*/
	private handleDeleteButtonClick = async () => {
		let requestData: any[] = [];
		this.state.userSelectedResponses.forEach((value: string) => {

			requestData.push({
				id: value
			})
		})
		const deletionResult = await deleteTags(requestData);
		if (deletionResult.status === 200) {
			let userResponses = this.state.tagData.filter((userResponse) => {
				return !this.state.userSelectedResponses.includes(userResponse.id.toString());
			});

			this.setState({
				tagData: userResponses,
				filteredTagResponses: userResponses,
				userSelectedResponses: []
			})
		} else if (deletionResult.status === 547) { // if any tag entity is referred in other entity then API won't allow it to delete.
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
					<TagTable showCheckbox={true} responsesData={this.state.filteredTagResponses} onCheckBoxChecked={this.onUserTagSelected} />
				</div>
				<ErrorMessage errorMessage={this.state.errorMessage} isGenericError={true} />
			</div>
		)
	}
}
export default withRouter(TagsTabPage)