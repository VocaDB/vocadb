import { ArtistDetailsContract } from '@/DataContracts/Artist/ArtistDetailsContract';
import { MarkdownHelper } from '@/Helpers/MarkdownHelper';
import { EntryType } from '@/Models/EntryType';
import { ArtistDetailsTabs } from '@/Pages/Artist/ArtistDetailsRoutes';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { functions } from '@/Shared/GlobalFunctions';
import { ArtistDetailsStore } from '@/Stores/Artist/ArtistDetailsStore';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface ArtistShareProps {
	artist: ArtistDetailsContract;
	artistDetailsStore: ArtistDetailsStore;
}

const ArtistShare = ({
	artist,
	artistDetailsStore,
}: ArtistShareProps): React.ReactElement => {
	const { t } = useTranslation(['ViewRes']);

	const url = functions.mergeUrls(
		vdb.values.hostAddress,
		EntryUrlMapper.details(EntryType.Artist, artist.id),
	);

	return (
		<ArtistDetailsTabs
			artist={artist}
			artistDetailsStore={artistDetailsStore}
			tab="share"
		>
			<div>
				{t('ViewRes:EntryDetails.Link')}
				<br />
				<input
					type="text"
					defaultValue={url}
					className="input-large"
					onFocus={(e): void => e.target.select()}
				/>
			</div>
			<div>
				<a href="http://daringfireball.net/projects/markdown/">
					Markdown{/* TODO: localize */}
				</a>
				<br />
				<input
					type="text"
					defaultValue={MarkdownHelper.createMarkdownLink(url, artist.name)}
					className="input-xxlarge"
					onFocus={(e): void => e.target.select()}
				/>
			</div>
		</ArtistDetailsTabs>
	);
};

export default ArtistShare;
