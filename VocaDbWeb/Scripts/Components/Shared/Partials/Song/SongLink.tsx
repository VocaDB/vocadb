import { SongToolTip } from '@Components/KnockoutExtensions/EntryToolTip';
import SongApiContract from '@DataContracts/Song/SongApiContract';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import qs from 'qs';
import React from 'react';
import { Link } from 'react-router-dom';

interface SongLinkProps {
	song: SongApiContract;
	albumId?: number;
	tooltip?: boolean;
	toolTipDomain?: string;
}

const SongLink = React.memo(
	({
		song,
		albumId,
		tooltip = false,
		toolTipDomain,
	}: SongLinkProps): React.ReactElement => {
		return tooltip ? (
			<SongToolTip
				as={Link}
				to={`${EntryUrlMapper.details_song(song)}?${qs.stringify({
					albumId: albumId,
				})}`}
				title={song.additionalNames}
				id={song.id}
				toolTipDomain={toolTipDomain}
			>
				{song.name}
			</SongToolTip>
		) : (
			<Link
				to={`${EntryUrlMapper.details_song(song)}?${qs.stringify({
					albumId: albumId,
				})}`}
				title={song.additionalNames}
			>
				{song.name}
			</Link>
		);
	},
);

export default SongLink;
