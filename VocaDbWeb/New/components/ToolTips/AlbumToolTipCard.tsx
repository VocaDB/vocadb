import { AlbumForApiContract } from '@/types/DataContracts/Album/AlbumForApiContract';
import { Paper, Tooltip } from '@mantine/core';
import React from 'react';
import AlbumToolTipContent from './AlbumToolTipContent';

export interface AlbumToolTipProps {
	album: AlbumForApiContract;
	children?: React.ReactNode;
}

export default function AlbumToolTip({ album, children }: AlbumToolTipProps) {
	return (
		<Tooltip
			styles={{
				tooltip: {
					padding: 0,
				},
			}}
			label={
				<Paper radius="xs" p="md">
					<AlbumToolTipContent album={album} />
				</Paper>
			}
		>
			{children}
		</Tooltip>
	);
}

