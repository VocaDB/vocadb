import { SongLink } from '@/Components/Shared/Partials/Song/SongLink';
import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import React from 'react';

interface SongLinkKnockoutBaseProps {
	song: SongApiContract;
	extUrl?: string;
}

const SongLinkKnockoutBase = ({
	song,
	extUrl,
}: SongLinkKnockoutBaseProps): React.ReactElement => {
	return (
		<a href={extUrl} title={song.additionalNames} className="extLink">
			{song.name}
		</a>
	);
};

interface SongLinkKnockoutProps {
	song: SongApiContract;
	albumId?: number;
	extUrl?: string;
	tooltip?: boolean;
	toolTipDomain?: string;
}

export const SongLinkKnockout = React.memo(
	({
		song,
		albumId,
		extUrl,
		tooltip = false,
		toolTipDomain,
	}: SongLinkKnockoutProps): React.ReactElement => {
		return extUrl ? (
			tooltip ? (
				/*<SongToolTip
					as="a"
					href={extUrl}
					title={song.additionalNames}
					id={song.id}
					toolTipDomain={toolTipDomain}
					className="extLink"
				>
					{song.name}
				</SongToolTip>*/
				<SongLinkKnockoutBase song={song} extUrl={extUrl} />
			) : (
				<SongLinkKnockoutBase song={song} extUrl={extUrl} />
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
