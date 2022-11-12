import moment from 'moment';
import React from 'react';

interface UniversalTimeLabelProps {
	dateTime: string;
}

export const UniversalTimeLabel = React.memo(
	({ dateTime }: UniversalTimeLabelProps): React.ReactElement => {
		return (
			<span title="UTC" /* LOC */>{moment(dateTime).utc().format('lll')}</span>
		);
	},
);
