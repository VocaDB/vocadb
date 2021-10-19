import Alert from '@Bootstrap/Alert';
import EntryBaseContract from '@DataContracts/EntryBaseContract';
import React from 'react';
import { useTranslation } from 'react-i18next';

import EntryLink from '../Shared/EntryLink';

interface DeletedBannerProps {
	mergedTo?: EntryBaseContract;
}

const DeletedBanner = React.memo(
	({ mergedTo }: DeletedBannerProps): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		return (
			<Alert>
				{t('Resources:CommonMessages.EntryDeleted')}
				{mergedTo && (
					<>
						{' '}
						{t('Resources:CommonMessages.EntryMergedTo')}{' '}
						<EntryLink entry={mergedTo} />.
					</>
				)}
			</Alert>
		);
	},
);

export default DeletedBanner;
