import WebLinkContract from '@DataContracts/WebLinkContract';
import UrlHelper from '@Helpers/UrlHelper';
import functions from '@Shared/GlobalFunctions';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface ExternalLinksListProps {
	webLinks: WebLinkContract[];
	showCategory?: boolean;
}

const ExternalLinksList = React.memo(
	({
		webLinks,
		showCategory = false,
	}: ExternalLinksListProps): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		return (
			<>
				{webLinks.map((webLink, index) => (
					<React.Fragment key={index}>
						{webLink.disabled ? (
							<a
								className="extLink"
								href={`https://web.archive.org/web/*/${webLink.url}`}
								onClick={(e): void =>
									functions.trackOutboundLink(e.nativeEvent)
								}
								title="See this page archived on the Wayback Machine" /* TODO: localize */
								style={{ textDecoration: 'line-through' }}
							>
								{webLink.description || webLink.url}
							</a>
						) : (
							<a
								className="extLink"
								href={UrlHelper.makePossileAffiliateLink(webLink.url)}
								onClick={(e): void =>
									functions.trackOutboundLink(e.nativeEvent)
								}
							>
								{webLink.description || webLink.url}
							</a>
						)}
						{showCategory && (
							<> ({t(`Resources:WebLinkCategoryNames.${webLink.category}`)})</>
						)}
						<br />
					</React.Fragment>
				))}
			</>
		);
	},
);

export default ExternalLinksList;
