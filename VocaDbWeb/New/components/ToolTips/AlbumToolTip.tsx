import { AlbumToolTipProps } from './AlbumToolTipCard';
import { Suspense } from 'react';
import React from 'react';

const AlbumToolTipCard = React.lazy(() => import('./AlbumToolTipCard'));

export const AlbumToolTip = ({ album, children }: AlbumToolTipProps) => {
	return (
		<Suspense fallback={children}>
			<AlbumToolTipCard album={album}>{children}</AlbumToolTipCard>
		</Suspense>
	);
};

