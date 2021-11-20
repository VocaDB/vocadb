import SongApiContract from '@DataContracts/Song/SongApiContract';
import UrlHelper from '@Helpers/UrlHelper';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import qs from 'qs';
import React from 'react';

interface SongIconLinkProps {
	song: SongApiContract;
	albumId?: number;
}

const SongIconLink = React.memo(
	({ song, albumId }: SongIconLinkProps): React.ReactElement => {
		return (
			<a
				href={`${EntryUrlMapper.details_song(song)}?${qs.stringify({
					albumId: albumId,
				})}`}
			>
				<img
					src={UrlHelper.upgradeToHttps(song.thumbUrl)}
					alt="Cover" /* TODO: localize */
					className="coverPicThumb"
					referrerPolicy="same-origin"
				/>
			</a>
		);
	},
);

export default SongIconLink;
