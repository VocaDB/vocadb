import React from 'react';
import { AlbumToolTipProps } from './AlbumToolTipContent';
import { ArtistToolTipProps } from './ArtistToolTipContent';

const EntryToolTipCard = React.lazy(() => import('./EntryToolTipCard'));
const ArtistToolTipContent = React.lazy(() => import('./ArtistToolTipContent'));
const AlbumToolTipCard = React.lazy(() => import('./AlbumToolTipContent'));

type EntryToolTipProps = (AlbumToolTipProps | ArtistToolTipProps) & { children: JSX.Element };

export default function EntryToolTip(props: EntryToolTipProps) {
	const getToolTip = () => {
		if (props.entry === 'album') {
			return <AlbumToolTipCard {...props} />;
		} else if (props.entry === 'artist') {
			return <ArtistToolTipContent {...props} />;
		}
	};
	return (
		<React.Suspense fallback={props.children}>
			<EntryToolTipCard tooltip={getToolTip()}>{props.children}</EntryToolTipCard>
		</React.Suspense>
	);
}

