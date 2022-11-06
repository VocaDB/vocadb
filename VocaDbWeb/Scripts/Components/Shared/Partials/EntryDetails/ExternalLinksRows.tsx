import { ExternalLinksList } from '@/Components/Shared/Partials/EntryDetails/ExternalLinksList';
import { WebLinkContract } from '@/DataContracts/WebLinkContract';
import { WebLinkCategory } from '@/Models/WebLinkCategory';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface ExternalLinksRowsProps {
	webLinks: WebLinkContract[];
}

export const ExternalLinksRows = React.memo(
	({ webLinks }: ExternalLinksRowsProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		const official = React.useMemo(
			() =>
				webLinks.filter(
					(webLink) =>
						webLink.category === WebLinkCategory.Official ||
						webLink.category === WebLinkCategory.Commercial,
				),
			[webLinks],
		);

		const other = React.useMemo(
			() => webLinks.filter((webLink) => !official.includes(webLink)),
			[webLinks, official],
		);

		return (
			<>
				{official.length > 0 && (
					<tr>
						<td>{t('ViewRes:EntryDetails.OfficialLinks')}</td>
						<td>
							<ExternalLinksList webLinks={official} showCategory={false} />
						</td>
					</tr>
				)}

				{other.length > 0 && (
					<tr>
						<td>{t('ViewRes:EntryDetails.OtherLinks')}</td>
						<td>
							<ExternalLinksList webLinks={other} showCategory={true} />
						</td>
					</tr>
				)}
			</>
		);
	},
);
