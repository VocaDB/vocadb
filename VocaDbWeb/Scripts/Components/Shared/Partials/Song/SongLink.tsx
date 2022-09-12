import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import qs from 'qs';
import React from 'react';
import { Link } from 'react-router-dom';

interface SongLinkBaseProps {
	song: SongApiContract;
	albumId?: number;
	target?: string;
}

const SongLinkBase = ({
	song,
	albumId,
	target,
}: SongLinkBaseProps): React.ReactElement => {
	return (
		<Link
			to={`${EntryUrlMapper.details_song(song)}?${qs.stringify({
				albumId: albumId,
			})}`}
			title={song.additionalNames}
			target={target}
		>
			{song.name}
		</Link>
	);
};

interface SongLinkProps {
	song: SongApiContract;
	albumId?: number;
	tooltip?: boolean;
	toolTipDomain?: string;
	target?: string;
}

export const SongLink = React.memo(
	({
		song,
		albumId,
		tooltip = false,
		toolTipDomain,
		target,
	}: SongLinkProps): React.ReactElement => {
		return tooltip ? (
			/*<SongToolTip
				as={Link}
				to={`${EntryUrlMapper.details_song(song)}?${qs.stringify({
					albumId: albumId,
				})}`}
				title={song.additionalNames}
				id={song.id}
				toolTipDomain={toolTipDomain}
				target={target}
			>
				{song.name}
			</SongToolTip>*/
			<SongLinkBase song={song} albumId={albumId} target={target} />
		) : (
			<SongLinkBase song={song} albumId={albumId} target={target} />
		);
	},
);
