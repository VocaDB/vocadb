import { SongToolTip } from '@/Components/KnockoutExtensions/EntryToolTip';
import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import qs from 'qs';
import React from 'react';
import { Link, LinkProps } from 'react-router-dom';

interface SongLinkBaseProps extends Omit<LinkProps, 'to'> {
	song: SongApiContract;
	albumId?: number;
}

const SongLinkBase = ({
	song,
	albumId,
	...props
}: SongLinkBaseProps): React.ReactElement => {
	return (
		<Link
			{...props}
			to={`${EntryUrlMapper.details_song(song)}?${qs.stringify({
				albumId: albumId,
			})}`}
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
			<SongToolTip id={song.id}>
				<SongLinkBase song={song} albumId={albumId} target={target} />
			</SongToolTip>
		) : (
			<SongLinkBase
				song={song}
				albumId={albumId}
				target={target}
				title={song.additionalNames}
			/>
		);
	},
);
