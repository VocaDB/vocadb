import { getBackgroundCredit } from '@/Components/Shared/Partials/BackgroundCredits';
import { useVdb } from '@/VdbContext';
import React from 'react';
import { useTranslation } from 'react-i18next';

export const AboutDisclaimer = React.memo((): React.ReactElement => {
	const { t } = useTranslation(['ViewRes']);
	const vdb = useVdb();
	const backgroundCredit = getBackgroundCredit(vdb.values);

	return (
		<span className="about-disclaimer">
			<a href={backgroundCredit.artistUrl}>
				{t('ViewRes:Layout.BackgroundCredit', { 0: backgroundCredit.artistName })}
			</a>
			{' | '}
			<a href="https://github.com/VocaDB/vocadb/">
				{t('ViewRes:Layout.OtherCredit')}
			</a>
			{' | '}
			<a href="https://wiki.vocadb.net/wiki/29/license">
				{t('ViewRes:Layout.License')}
			</a>
			{' | '}
			<a href="https://wiki.vocadb.net/wiki/50/privacy-and-cookie-policy">
				{t('ViewRes:Layout.PrivacyPolicy')}
			</a>
		</span>
	);
});
