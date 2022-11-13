import { DateTimeHelper } from '@/Helpers/DateTimeHelper';
import React from 'react';

interface UniversalTimeLabelProps {
	dateTime: string;
}

export const UniversalTimeLabel = React.memo(
	({ dateTime }: UniversalTimeLabelProps): React.ReactElement => {
		return (
			<span title="UTC" /* LOC */>
				{DateTimeHelper.DateTime_utc_format_lll(dateTime)}
			</span>
		);
	},
);
