import { Suspense } from 'react';
import React from 'react';
import { ArtistToolTipProps } from './ArtistToolTipCard';

const ArtistToolTipCard = React.lazy(() => import('./ArtistToolTipCard'));

export const ArtistToolTip = ({ artist, children }: ArtistToolTipProps) => {
	return (
		<Suspense fallback={children}>
			<ArtistToolTipCard artist={artist}>{children}</ArtistToolTipCard>
		</Suspense>
	);
};

