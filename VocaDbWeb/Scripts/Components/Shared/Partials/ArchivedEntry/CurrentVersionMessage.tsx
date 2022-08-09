import { EntryStatus } from '@/Models/EntryStatus';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface CurrentVersionMessageProps {
	version?: number;
	status: EntryStatus;
}

export const CurrentVersionMessage = ({
	version,
	status,
}: CurrentVersionMessageProps): React.ReactElement => {
	const { t } = useTranslation(['Resources', 'ViewRes']);

	return (
		<p>
			{t('ViewRes:ArchivedObjectVersions.CurrentVersionIs')} {version} (
			{t(`Resources:EntryStatusNames.${EntryStatus[status]}`)}).
		</p>
	);
};
