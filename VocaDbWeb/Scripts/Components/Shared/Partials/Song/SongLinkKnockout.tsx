import { SongToolTip } from '@/Components/KnockoutExtensions/EntryToolTip';
import SongLink from '@/Components/Shared/Partials/Song/SongLink';
import SongApiContract from '@/DataContracts/Song/SongApiContract';
import React from 'react';

interface SongLinkKnockoutProps {
	song: SongApiContract;
	albumId?: number;
	extUrl?: string;
	tooltip?: boolean;
	toolTipDomain?: string;
}

const SongLinkKnockout = React.memo(
	({
		song,
		albumId,
		extUrl,
		tooltip = false,
		toolTipDomain,
	}: SongLinkKnockoutProps): React.ReactElement => {
		return extUrl ? (
			tooltip ? (
				<SongToolTip
					as="a"
					href={extUrl}
					title={song.additionalNames}
					id={song.id}
					toolTipDomain={toolTipDomain}
					className="extLink"
				>
					{song.name}
				</SongToolTip>
			) : (
				<a href={extUrl} title={song.additionalNames} className="extLink">
					{song.name}
				</a>
			)
		) : (
			<SongLink
				song={song}
				albumId={albumId}
				tooltip={tooltip}
				toolTipDomain={toolTipDomain}
			/>
		);
	},
);

export default SongLinkKnockout;
