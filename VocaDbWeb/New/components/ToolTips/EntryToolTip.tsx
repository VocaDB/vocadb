import React, { useEffect, useState } from 'react';
import { AlbumToolTipProps } from './AlbumToolTipContent';
import { ArtistToolTipProps } from './ArtistToolTipContent';
import { TagToolTipProps } from './TagToolTipContent';
import { SongToolTipProps } from './SongToolTIpContent';

const EntryToolTipCard = React.lazy(() => import('./EntryToolTipCard'));
const ArtistToolTipContent = React.lazy(() => import('./ArtistToolTipContent'));
const AlbumToolTipContent = React.lazy(() => import('./AlbumToolTipContent'));
const TagToolTipContent = React.lazy(() => import('./TagToolTipContent'));
const SongToolTipContent = React.lazy(() => import('./SongToolTIpContent'));

type EntryToolTipProps = (
	| AlbumToolTipProps
	| ArtistToolTipProps
	| TagToolTipProps
	| SongToolTipProps
) & {
	children: JSX.Element;
};

// TODO: Investigate hydration suspenbse issues
export default function EntryToolTip(props: EntryToolTipProps) {
	const [tooltip, setTooltip] = useState<JSX.Element | undefined>(undefined);
	const getToolTip = () => {
		if (props.entry === 'album') {
			return <AlbumToolTipContent {...props} />;
		} else if (props.entry === 'artist') {
			return <ArtistToolTipContent {...props} />;
		} else if (props.entry === 'tag') {
			return <TagToolTipContent {...props} />;
		} else if (props.entry === 'song') {
			return <SongToolTipContent {...props} />;
		}
	};

	useEffect(() => {
		setTooltip(getToolTip());
	}, [props]);

	return (
		<React.Suspense fallback={props.children}>
			<EntryToolTipCard tooltip={tooltip}>{props.children}</EntryToolTipCard>
		</React.Suspense>
	);
}

