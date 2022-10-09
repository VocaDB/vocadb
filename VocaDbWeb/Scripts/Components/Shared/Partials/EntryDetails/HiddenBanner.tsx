import Alert from '@/Bootstrap/Alert';
import React from 'react';
import { useTranslation } from 'react-i18next';

export const HiddenBanner = React.memo(
	(): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		return <Alert>{t('Resources:CommonMessages.RevisionHidden')}</Alert>;
	},
);
