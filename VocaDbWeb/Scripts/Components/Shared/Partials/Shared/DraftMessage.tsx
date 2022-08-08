import Alert from '@/Bootstrap/Alert';
import NotificationIcon from '@/Components/Shared/Partials/Shared/NotificationIcon';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface DraftMessageProps {
	section: string;
}

const DraftMessage = React.memo(
	({ section }: DraftMessageProps): React.ReactElement => {
		const { t } = useTranslation(['HelperRes']);

		return (
			<Alert>
				<NotificationIcon />
				<span>{t('HelperRes:Helper.DraftMessage')}</span>
				<span>
					{t('HelperRes:Helper.SeeGuidePre')}{' '}
					<Link to={`/Help/guidelines#${section}`}>
						{t('HelperRes:Helper.SeeGuide')}
					</Link>{' '}
					{t('HelperRes:Helper.SeeGuidePost')}
				</span>
			</Alert>
		);
	},
);

export default DraftMessage;
