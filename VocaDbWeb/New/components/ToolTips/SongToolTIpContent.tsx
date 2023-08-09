'use client';
import { SongApiContract } from '@/types/DataContracts/Song/SongApiContract';
import { Group, Text } from '@mantine/core';
import CustomImage from '../Image/Image';
import useSWR from 'swr';
import { apiGet } from '@/Helpers/FetchApiHelper';

export interface SongToolTipProps {
	entry: 'song';
	song: SongApiContract;
}

export default function SongToolTipContent({ song }: SongToolTipProps) {
	const { data } = useSWR(`/api/songs/${song.id}?fields=ThumbUrl`, apiGet<SongApiContract>);

	return (
		<Group>
			<CustomImage src={data?.thumbUrl} width={110} height={90} alt="" />
			<div style={{ flex: 1 }}>
				<Text>
					{song.name}
					<Text span>
						{' ('}
						{song.songType}
						{')'}
					</Text>
				</Text>
				<Text size="sm" color="dimmed">
					{song.artistString}
				</Text>
			</div>
		</Group>
	);
}

