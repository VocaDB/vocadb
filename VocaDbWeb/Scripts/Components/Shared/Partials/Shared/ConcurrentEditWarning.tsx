import { NotificationPanel } from '@/Components/Shared/Partials/Shared/NotificationPanel';
import { EntryEditDataContract } from '@/DataContracts/User/EntryEditDataContract';
import dayjs from 'dayjs';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface ConcurrentEditWarningProps {
	conflictingEditor: EntryEditDataContract;
}

export const ConcurrentEditWarning = React.memo(
	({ conflictingEditor }: ConcurrentEditWarningProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		const ago = dayjs().diff(conflictingEditor.time, 'minutes');

		return (
			<NotificationPanel
				message={
					ago < 1
						? t('ViewRes:EntryEdit.ConcurrentEditWarningNow', {
								0: conflictingEditor.userName,
						  })
						: t('ViewRes:EntryEdit.ConcurrentEditWarning', {
								0: conflictingEditor.userName,
								1: ago,
						  })
				}
			/>
		);
	},
);
