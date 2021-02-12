// <copyright file="checkbox-base.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Checkbox } from "@fluentui/react-northstar";

import "../../styles/admin-configure-wrapper-page.css";

interface ICheckboxState {
	isCheckboxChecked: boolean;
}
interface ICheckboxProps {
	value: string;
	onCheckboxChecked: (responseId: string, isChecked: boolean) => void;
}

/**
* Component for showing selection checkbox base component.
*/
export default class CheckboxBase extends React.Component<ICheckboxProps, ICheckboxState> {

	constructor(props: ICheckboxProps) {
		super(props);

		this.state = {
			isCheckboxChecked: false
		}
	}

	/**
	* Triggers when user checks/un-checks checkbox to set state.
	*/
	private onChange = (responseId: string, isChecked: boolean) => {
		this.setState({ isCheckboxChecked: isChecked });
		this.props.onCheckboxChecked(responseId, isChecked);
	}

	/**
	* Renders the component
	*/
	public render(): JSX.Element {
		return (
			<div>
				<Checkbox checked={this.state.isCheckboxChecked} onChange={() => this.onChange(this.props.value, !this.state.isCheckboxChecked)} className="checkbox-ui" />
			</div>)
	}
}