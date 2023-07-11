import { AlbumForApiContract } from '@/types/DataContracts/Album/AlbumForApiContract';
import { HoverCard } from '@mantine/core';
import React from 'react';
import AlbumToolTipContent from './AlbumToolTipContent';

export interface AlbumToolTipProps {
	album: AlbumForApiContract;
	children?: React.ReactNode;
}

// TODO: Make the whole Tooltip lazy to prevent loading the HoverCard js
export default function AlbumToolTip({ album, children }: AlbumToolTipProps) {
	return (
		<HoverCard shadow="sm">
			<HoverCard.Target>{children}</HoverCard.Target>
			<HoverCard.Dropdown>
				<AlbumToolTipContent album={album} />
			</HoverCard.Dropdown>
		</HoverCard>
	);
}

