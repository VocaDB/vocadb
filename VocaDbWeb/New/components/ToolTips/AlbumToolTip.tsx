import { DateTimeHelper } from '@/Helpers/DateTimeHelper';
import { AlbumForApiContract } from '@/types/DataContracts/Album/AlbumForApiContract';
import { Group, HoverCard, Text } from '@mantine/core';
import React from 'react';

interface AlbumToolTipProps {
	album: AlbumForApiContract;
	children?: React.ReactNode;
}

export function AlbumToolTip({ album, children }: AlbumToolTipProps) {
	const releaseDate = DateTimeHelper.formatComponentDate(
		album.releaseDate.year,
		album.releaseDate.month,
		album.releaseDate.day
	);

	return (
		<HoverCard>
			<HoverCard.Target>{children}</HoverCard.Target>
			<HoverCard.Dropdown>
				<Text>{album.name}</Text>
				<Text size="sm" color="dimmed">
					{album.artistString}
				</Text>
				{!album.releaseDate.isEmpty && (
					<Text size="sm" mt="md">
						{'Released '} {releaseDate}
					</Text>
				)}
			</HoverCard.Dropdown>
		</HoverCard>
	);
}

