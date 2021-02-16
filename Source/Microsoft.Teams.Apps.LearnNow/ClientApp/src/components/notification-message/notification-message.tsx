// <copyright file="notification-message.tsx" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

import * as React from "react";
import { Flex, Text } from "@fluentui/react-northstar";
import { CloseIcon } from '@fluentui/react-icons-northstar';
import { Icon } from '@fluentui/react/lib/Icon';
import { NotificationType } from "../../model/type";

import "../../styles/alert.css";


interface INotificationMessageProps {
    notificationType: number;
    content: string;
    showAlert: boolean;
    onClose: () => void;
}

/**
* Component for showing notification message.
*/
const NotificationMessage: React.FunctionComponent<INotificationMessageProps> = props => {

    let [showAlert, setShowAlert] = React.useState(props.showAlert ? props.showAlert : false);

    React.useEffect(() => {
        setShowAlert(props.showAlert)
    }, [props.showAlert])

    /**
    * Renders the component.
    */
    const renderNotification = () => {
        if (showAlert) {
            return (
                <div className="notification-container">
                    <div className={`notification-${props.notificationType === NotificationType.Success ? 'success' : 'error'}`}>
                        <Flex gap="gap.smaller" vAlign="center">
                            <Flex.Item>
                                {
                                    props.notificationType === NotificationType.Success ? <Icon iconName="CompletedSolid" className={"success-icon"} /> : <Icon iconName="CompletedSolid" className={"failure-icon"} />
                                }
                            </Flex.Item>
                            <Flex.Item>
                                <Text content={props.content} size="medium" />
                            </Flex.Item>
                            <Flex.Item push>
                                <CloseIcon className="close-button" onClick={props.onClose} />
                            </Flex.Item>
                        </Flex>
                    </div>
                </div>
            );
        }
        else {
            return (<></>);
        }
    }

    return renderNotification();
}

export default NotificationMessage;