import { SongToolTip } from '@Components/KnockoutExtensions/EntryToolTip';
import SongApiContract from '@DataContracts/Song/SongApiContract';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import qs from 'qs';
import React from 'react';

interface SongLinkProps {
	song: SongApiContract;
	albumId?: number;
	tooltip?: boolean;
}

const SongLink = React.memo(
	({ song, albumId, tooltip = false }: SongLinkProps): React.ReactElement => {
		return tooltip ? (
			<SongToolTip
				as="a"
				href={`${EntryUrlMapper.details_song(song)}?${qs.stringify({
					albumId: albumId,
				})}`}
				title={song.additionalNames}
				id={song.id}
			>
				{song.name}
			</SongToolTip>
		) : (
			<a
				href={`${EntryUrlMapper.details_song(song)}?${qs.stringify({
					albumId: albumId,
				})}`}
				title={song.additionalNames}
			>
				{song.name}
			</a>
		);
	},
);

export default SongLink;
