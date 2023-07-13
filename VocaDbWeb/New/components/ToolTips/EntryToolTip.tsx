import React from 'react';
import { AlbumToolTipProps } from './AlbumToolTipCard';
import { ArtistToolTipProps } from './ArtistToolTipCard';

const ArtistToolTipCard = React.lazy(() => import('./ArtistToolTipCard'));
const AlbumToolTipCard = React.lazy(() => import('./AlbumToolTipCard'));

type EntryToolTipProps = AlbumToolTipProps | ArtistToolTipProps;

export default function EntryToolTip(props: EntryToolTipProps) {
	if (props.entry === 'album') {
		return <AlbumToolTipCard {...props} />;
	} else if (props.entry === 'artist') {
		return <ArtistToolTipCard {...props} />;
	}
	return <></>;
}

