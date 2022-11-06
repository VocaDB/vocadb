import { EntryStatus } from '@/Models/EntryStatus';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface EntryStatusMessageProps {
	status: EntryStatus;
}

export const EntryStatusMessage = React.memo(
	({ status }: EntryStatusMessageProps): React.ReactElement => {
		const { t } = useTranslation(['HelperRes', 'Resources']);

		switch (status) {
			case EntryStatus.Draft:
				return (
					<span title={t('HelperRes:Helper.StatusDescriptionDraft')}>
						<span className="icon draftIcon" />{' '}
						{t(`Resources:EntryStatusNames.${EntryStatus.Draft}`)}
					</span>
				);

			case EntryStatus.Finished:
				return (
					<span title={t('HelperRes:Helper.StatusDescriptionFinished')}>
						<span className="icon asteriskIcon" />{' '}
						{t(`Resources:EntryStatusNames.${EntryStatus.Finished}`)}
					</span>
				);

			case EntryStatus.Approved:
				return (
					<span title={t('HelperRes:Helper.StatusDescriptionApproved')}>
						<span className="icon tickIcon" />{' '}
						{t(`Resources:EntryStatusNames.${EntryStatus.Approved}`)}
					</span>
				);

			case EntryStatus.Locked:
				return (
					<span title={t('HelperRes:Helper.StatusDescriptionLocked')}>
						<span className="icon lockIcon" />{' '}
						{t(`Resources:EntryStatusNames.${EntryStatus.Locked}`)}
					</span>
				);

			default:
				return <></>;
		}
	},
);
