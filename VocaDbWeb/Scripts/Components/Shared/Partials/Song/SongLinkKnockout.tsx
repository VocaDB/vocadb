import { SongToolTip } from '@/Components/KnockoutExtensions/EntryToolTip';
import { SongLink } from '@/Components/Shared/Partials/Song/SongLink';
import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import React from 'react';

interface SongLinkKnockoutBaseProps
	extends React.AnchorHTMLAttributes<HTMLAnchorElement> {
	song: SongApiContract;
	extUrl?: string;
}

const SongLinkKnockoutBase = ({
	song,
	extUrl,
	...props
}: SongLinkKnockoutBaseProps): React.ReactElement => {
	return (
		<a {...props} href={extUrl} className="extLink">
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
				<SongToolTip id={song.id} foreignDomain={toolTipDomain}>
					<SongLinkKnockoutBase
						song={song}
						extUrl={extUrl}
						target="_blank"
						referrerPolicy="same-origin"
					/>
				</SongToolTip>
			) : (
				<SongLinkKnockoutBase
					song={song}
					extUrl={extUrl}
					title={song.additionalNames}
				/>
			)
		) : (
			<SongLink song={song} albumId={albumId} tooltip={tooltip} />
		);
	},
);
