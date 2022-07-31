import { ArchivedWebLinkContract } from '@/DataContracts/ArchivedWebLinkContract';
import { UrlHelper } from '@/Helpers/UrlHelper';
import React from 'react';

interface WebLinkInfoProps {
	link: ArchivedWebLinkContract;
}

export const WebLinkInfo = React.memo(
	({ link }: WebLinkInfoProps): React.ReactElement => {
		return (
			<>
				{link.description && <>{link.description}: </>}
				<a href={UrlHelper.makeLink(link.url)} /* TODO: target="_blank" */>
					{link.url}
				</a>{' '}
				({link.category}){link.disabled && <> (unavailable)</>}
			</>
		);
	},
);
