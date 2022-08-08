import Alert from '@/Bootstrap/Alert';
import NotificationIcon from '@/Components/Shared/Partials/Shared/NotificationIcon';
import React from 'react';

interface NotificationPanelProps {
	message: string;
	messageId?: string;
}

const NotificationPanel = React.memo(
	({ message, messageId }: NotificationPanelProps): React.ReactElement => {
		return (
			<Alert>
				<NotificationIcon />
				<span id={messageId}>{message}</span>
			</Alert>
		);
	},
);

export default NotificationPanel;
