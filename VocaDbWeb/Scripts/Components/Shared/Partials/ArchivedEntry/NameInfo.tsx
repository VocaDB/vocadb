import { LocalizedStringContract } from '@/DataContracts/Globalization/LocalizedStringContract';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface NameInfoProps {
	name: LocalizedStringContract;
}

export const NameInfo = React.memo(
	({ name }: NameInfoProps): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		return (
			<>
				{t(`Resources:ContentLanguageSelectionNames.${name.language}`)}:{' '}
				{name.value}
			</>
		);
	},
);
