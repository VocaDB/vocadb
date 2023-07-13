import { Paper, Tooltip } from '@mantine/core';
import React from 'react';
import { ArtistApiContract } from '@/types/DataContracts/Artist/ArtistApiContract';
import ArtistToolTipContent from './ArtistToolTipContent';

export interface ArtistToolTipProps {
	artist: ArtistApiContract;
	children?: React.ReactNode;
}

export default function ArtistToolTip({ artist, children }: ArtistToolTipProps) {
	return (
		<Tooltip
			styles={{
				tooltip: {
					padding: 0,
				},
			}}
			label={
				<Paper radius="xs" p="md">
					<ArtistToolTipContent artist={artist} />
				</Paper>
			}
		>
			{children}
		</Tooltip>
	);
}

