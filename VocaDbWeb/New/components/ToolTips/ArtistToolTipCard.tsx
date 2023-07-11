import { HoverCard } from '@mantine/core';
import React from 'react';
import { ArtistApiContract } from '@/types/DataContracts/Artist/ArtistApiContract';
import ArtistToolTipContent from './ArtistToolTipContent';

export interface ArtistToolTipProps {
	artist: ArtistApiContract;
	children?: React.ReactNode;
}

// TODO: Make the whole Tooltip lazy to prevent loading the HoverCard js
export default function AlbumToolTip({ artist, children }: ArtistToolTipProps) {
	return (
		<HoverCard shadow="sm">
			<HoverCard.Target>{children}</HoverCard.Target>
			<HoverCard.Dropdown>
				<ArtistToolTipContent artist={artist} />
			</HoverCard.Dropdown>
		</HoverCard>
	);
}

