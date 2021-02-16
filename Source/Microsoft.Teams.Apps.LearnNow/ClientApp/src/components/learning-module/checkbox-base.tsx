// <copyright file="checkbox-base-learning-module.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Checkbox } from "@fluentui/react-northstar";
import "../../styles/admin-configure-wrapper-page.css";

interface ICheckboxProps {
	value: string;
	checked: boolean;
	onCheckboxChecked: (responseId: string, isChecked: boolean) => void;
}

/**
* Component for rendering checkbox for resource in learning module.
*/
export default class CheckboxBase extends React.Component<ICheckboxProps> {

	constructor(props: ICheckboxProps) {
		super(props);
	}

	/**
	*Triggers when user checks/unchecks checkbox to set state
	*/
	private onChange = (responseId: string, isChecked: boolean) => {
		this.props.onCheckboxChecked(responseId, isChecked);
	}

	/**
	* Renders the component
	*/
	public render(): JSX.Element {
		return (
			<div>
				<Checkbox checked={this.props.checked} onChange={() => this.onChange(this.props.value, !this.props.checked)} className="checkbox-ui" />
			</div>)
	}
}
