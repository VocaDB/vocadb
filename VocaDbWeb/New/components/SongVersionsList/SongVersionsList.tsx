import { SongApiContract } from '@/types/DataContracts/Song/SongApiContract';
import { Anchor, Space, Text } from '@mantine/core';
import SongLink from '../Links/SongLink';
import { formatNumberToTime } from '@/Helpers/DateTimeHelper';
import { useState } from 'react';
import React from 'react';

interface SongVersionsList {
	songs: SongApiContract[];
}

export default function SongVersionsList({ songs }: SongVersionsList) {
	const [expanded, setExpanded] = useState(false);
	// TODO: Song type badge
	return (
		<>
			{songs
				.filter((_s, index) => expanded || index < 3)
				.map((s) => (
					<React.Fragment key={s.id}>
						<Text>
							<SongLink song={s} />
							{` (${formatNumberToTime(s.lengthSeconds)})`}
						</Text>
						<Text color="dimmed" size="sm">
							{s.artistString}
						</Text>
					</React.Fragment>
				))}
			{!expanded && songs.length >= 3 && (
				<>
					<Space h="xs" />
					<Anchor onClick={() => setExpanded(true)}>Show all ({songs.length})</Anchor>
				</>
			)}
		</>
	);
}

