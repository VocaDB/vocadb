import dayjs from '@/dayjs';
import React from 'react';

interface UniversalTimeLabelProps {
	dateTime: string;
}

export const UniversalTimeLabel = React.memo(
	({ dateTime }: UniversalTimeLabelProps): React.ReactElement => {
		return (
			<span title="UTC" /* LOC */>{dayjs(dateTime).utc().format('lll')}</span>
		);
	},
);
