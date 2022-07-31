import { ArchivedTranslatedStringContract } from '@/DataContracts/ArchivedTranslatedStringContract';
import { ComparedVersionsContract } from '@/DataContracts/Versioning/ComparedVersionsContract';
import { ContentLanguageSelection } from '@/Models/Globalization/ContentLanguageSelection';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface TranslatedNameRowProps {
	name: ArchivedTranslatedStringContract;
	compareToName?: ArchivedTranslatedStringContract;
}

export const TranslatedNameRow = React.memo(
	({ name, compareToName }: TranslatedNameRowProps): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		return (
			<>
				<tr>
					<td>Original language{/* TODO: localize */}</td>
					<td colSpan={compareToName !== undefined ? 1 : 2}>
						{!!name &&
							t(
								`Resources:ContentLanguageSelectionNames.${name.defaultLanguage}`,
							)}
					</td>
					{compareToName !== undefined && (
						<td>
							{t(
								`Resources:ContentLanguageSelectionNames.${compareToName.defaultLanguage}`,
							)}
						</td>
					)}
				</tr>
				<tr>
					<td>Primary names{/* TODO: localize */}</td>
					<td colSpan={compareToName !== undefined ? 1 : 2}>
						{name && (
							<>
								{t(
									`Resources:ContentLanguageSelectionNames.${ContentLanguageSelection.Japanese}`,
								)}
								: {name.japanese}
								<br />
								{t(
									`Resources:ContentLanguageSelectionNames.${ContentLanguageSelection.Romaji}`,
								)}
								: {name.romaji}
								<br />
								{t(
									`Resources:ContentLanguageSelectionNames.${ContentLanguageSelection.English}`,
								)}
								: {name.english}
								<br />
							</>
						)}
					</td>
					{compareToName !== undefined && (
						<td>
							{t(
								`Resources:ContentLanguageSelectionNames.${ContentLanguageSelection.Japanese}`,
							)}
							: {compareToName.japanese}
							<br />
							{t(
								`Resources:ContentLanguageSelectionNames.${ContentLanguageSelection.Romaji}`,
							)}
							: {compareToName.romaji}
							<br />
							{t(
								`Resources:ContentLanguageSelectionNames.${ContentLanguageSelection.English}`,
							)}
							: {compareToName.english}
							<br />
						</td>
					)}
				</tr>
			</>
		);
	},
);

interface TranslatedNameRow_ComparedVersionsContractProps<T> {
	comparedVersions: ComparedVersionsContract<T>;
	valGetter: (data: T) => ArchivedTranslatedStringContract;
}

export const TranslatedNameRow_ComparedVersionsContract = <T,>({
	comparedVersions,
	valGetter,
}: TranslatedNameRow_ComparedVersionsContractProps<T>): React.ReactElement => {
	const name = valGetter(comparedVersions.firstData);
	const compareToName =
		comparedVersions.secondData !== undefined
			? valGetter(comparedVersions.secondData)
			: undefined;

	return <TranslatedNameRow name={name} compareToName={compareToName} />;
};
