import { functions } from '@/Shared/GlobalFunctions';
import { vdbConfig } from '@/vdbConfig';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface LanguageFlagProps {
	languageCode: string;
}

export const LanguageFlag = React.memo(
	({ languageCode }: LanguageFlagProps): React.ReactElement => {
		const { t } = useTranslation(['VocaDb.Web.Resources.Domain.Globalization']);

		return languageCode ? (
			<img
				src={functions.mergeUrls(
					vdbConfig.staticContentHost,
					`/img/languageFlags/${languageCode}.png`,
				)}
				alt={languageCode}
				title={languageCode}
			/>
		) : (
			<img
				src={functions.mergeUrls(
					vdbConfig.staticContentHost,
					'/img/languageFlags/unknown.png',
				)}
				alt={t(
					'VocaDb.Web.Resources.Domain.Globalization:InterfaceLanguage.Other',
				)}
				title={t(
					'VocaDb.Web.Resources.Domain.Globalization:InterfaceLanguage.Other',
				)}
			/>
		);
	},
);
