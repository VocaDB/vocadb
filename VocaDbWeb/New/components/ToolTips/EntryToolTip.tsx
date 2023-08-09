import React from 'react';
import AlbumToolTipContent, { AlbumToolTipProps } from './AlbumToolTipContent';
import ArtistToolTipContent, { ArtistToolTipProps } from './ArtistToolTipContent';
import TagToolTipContent, { TagToolTipProps } from './TagToolTipContent';
import SongToolTipContent, { SongToolTipProps } from './SongToolTIpContent';
import EntryToolTipCard from './EntryToolTipCard';

// const EntryToolTipCard = React.lazy(() => import('./EntryToolTipCard'));
// const ArtistToolTipContent = React.lazy(() => import('./ArtistToolTipContent'));
// const AlbumToolTipContent = React.lazy(() => import('./AlbumToolTipContent'));
// const TagToolTipContent = React.lazy(() => import('./TagToolTipContent'));
// const SongToolTipContent = React.lazy(() => import('./SongToolTIpContent'));

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

	return (
		<React.Suspense fallback={props.children}>
			<EntryToolTipCard tooltip={getToolTip()}>{props.children}</EntryToolTipCard>
		</React.Suspense>
	);
}

