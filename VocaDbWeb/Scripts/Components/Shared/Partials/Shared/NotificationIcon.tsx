import React from 'react';

export const NotificationIcon = React.memo(
	(): React.ReactElement => {
		return (
			<span
				className="ui-icon ui-icon-info"
				style={{ float: 'left', marginRight: '.3em' }}
			></span>
		);
	},
);
