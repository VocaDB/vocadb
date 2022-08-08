import Alert from '@/Bootstrap/Alert';
import ArtistContract from '@/DataContracts/Artist/ArtistContract';
import React from 'react';
import { useTranslation } from 'react-i18next';

import ArtistLink from '../Partials/Artist/ArtistLink';
import NotificationIcon from '../Partials/Shared/NotificationIcon';

interface CoverArtistMessageProps {
	coverArtists: ArtistContract[];
}

// Shows a message for cover artists in an original song.
const CoverArtistMessage = React.memo(
	({ coverArtists }: CoverArtistMessageProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		return (
			<Alert>
				<NotificationIcon /> {t('ViewRes:EntryCreate.FoundCoverArtist')}
				<ul>
					{coverArtists.map((coverArtist, index) => (
						<li key={index}>
							<ArtistLink artist={coverArtist} /* TODO: target="_blank" */ />
							{coverArtist.additionalNames && (
								<div>
									<span>{coverArtist.additionalNames}</span>
								</div>
							)}
						</li>
					))}
				</ul>
			</Alert>
		);
	},
);

export default CoverArtistMessage;
