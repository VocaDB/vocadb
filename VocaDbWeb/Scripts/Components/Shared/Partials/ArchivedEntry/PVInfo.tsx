import { ArchivedPVContract } from '@/DataContracts/PVs/ArchivedPVContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { DateTimeHelper } from '@/Helpers/DateTimeHelper';
import { VideoServiceHelper } from '@/Helpers/VideoServiceHelper';
import moment from 'moment';
import React from 'react';

interface PVInfoProps {
	pv: ArchivedPVContract;
}

export const PVInfo = React.memo(
	({ pv }: PVInfoProps): React.ReactElement => {
		return (
			<>
				{pv.service}:{' '}
				<a
					href={VideoServiceHelper.getUrlById(pv as PVContract)}
					/* TODO: target="_blank" */
				>
					{pv.pvId}
				</a>{' '}
				by {pv.author} ({DateTimeHelper.formatFromSeconds(pv.length)})
				{pv.publishDate && <> at {moment(pv.publishDate).format('l')}</>}
				{pv.disabled && <> (unavailable)</>}
			</>
		);
	},
);
