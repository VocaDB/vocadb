import { AlbumForApiContract } from '@/types/DataContracts/Album/AlbumForApiContract';
import { HoverCard } from '@mantine/core';
import React from 'react';

interface AlbumToolTipProps {
	album: AlbumForApiContract;
	children?: React.ReactNode;
}

// TODO: Check if this is worth it
const Content = React.lazy(() => import('./AlbumToolTipContent'));

export function AlbumToolTip({ album, children }: AlbumToolTipProps) {
	return (
		<HoverCard shadow="sm">
			<HoverCard.Target>{children}</HoverCard.Target>
			<HoverCard.Dropdown>
				<Content album={album} />
			</HoverCard.Dropdown>
		</HoverCard>
	);
}

