import SongApiContract from '@DataContracts/Song/SongApiContract';
import UrlHelper from '@Helpers/UrlHelper';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import qs from 'qs';
import React from 'react';
import { Link } from 'react-router-dom';

interface SongIconLinkProps {
	song: SongApiContract;
	albumId?: number;
}

const SongIconLink = React.memo(
	({ song, albumId }: SongIconLinkProps): React.ReactElement => {
		return (
			<Link
				to={`${EntryUrlMapper.details_song(song)}?${qs.stringify({
					albumId: albumId,
				})}`}
			>
				<img
					src={UrlHelper.upgradeToHttps(song.mainPicture?.urlThumb)}
					alt="Cover" /* TODO: localize */
					className="coverPicThumb"
					referrerPolicy="same-origin"
				/>
			</Link>
		);
	},
);

export default SongIconLink;
