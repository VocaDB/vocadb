import moment from 'moment';
import React from 'react';

interface UniversalTimeLabelProps {
	dateTime: Date;
}

const UniversalTimeLabel = React.memo(
	({ dateTime }: UniversalTimeLabelProps): React.ReactElement => {
		return (
			<span title="UTC" /* TODO: localize */>
				{moment(dateTime).utc().format('lll')}
			</span>
		);
	},
);

export default UniversalTimeLabel;
