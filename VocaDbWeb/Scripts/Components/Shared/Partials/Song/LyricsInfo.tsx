import { useCultureCodes } from '@/CultureCodesContext';
import { LyricsForSongContract } from '@/DataContracts/Song/LyricsForSongContract';
import React from 'react';

interface LyricsInfoProps {
	lyrics: LyricsForSongContract;
}

export const LyricsInfo = React.memo(
	({ lyrics }: LyricsInfoProps): React.ReactElement => {
		const { getCodeDescription } = useCultureCodes();
		return (
			<>
				{lyrics.translationType}
				{lyrics.cultureCodes && (
					<>
						{' '}
						(
						{getCodeDescription(lyrics.cultureCodes[0])?.englishName ?? 'Other'}
						)
					</>
				)}
				{lyrics.source && <> ({lyrics.source})</>}
			</>
		);
	},
);
