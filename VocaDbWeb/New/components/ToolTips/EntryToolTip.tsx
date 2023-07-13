import React from 'react';
import { AlbumToolTipProps } from './AlbumToolTipContent';
import { ArtistToolTipProps } from './ArtistToolTipContent';
import { TagToolTipProps } from './TagToolTipContent';

const EntryToolTipCard = React.lazy(() => import('./EntryToolTipCard'));
const ArtistToolTipContent = React.lazy(() => import('./ArtistToolTipContent'));
const AlbumToolTipContent = React.lazy(() => import('./AlbumToolTipContent'));
const TagToolTipContent = React.lazy(() => import('./TagToolTipContent'));

type EntryToolTipProps = (AlbumToolTipProps | ArtistToolTipProps | TagToolTipProps) & {
	children: JSX.Element;
};

export default function EntryToolTip(props: EntryToolTipProps) {
	const getToolTip = () => {
		if (props.entry === 'album') {
			return <AlbumToolTipContent {...props} />;
		} else if (props.entry === 'artist') {
			return <ArtistToolTipContent {...props} />;
		} else if (props.entry === 'tag') {
			return <TagToolTipContent {...props} />;
		}
	};
	return (
		<React.Suspense fallback={props.children}>
			<EntryToolTipCard tooltip={getToolTip()}>{props.children}</EntryToolTipCard>
		</React.Suspense>
	);
}

