import { userLanguageCultures } from '@/Components/userLanguageCultures';
import { LyricsForSongContract } from '@/DataContracts/Song/LyricsForSongContract';
import React from 'react';

interface LyricsInfoProps {
	lyrics: LyricsForSongContract;
}

export const LyricsInfo = React.memo(
	({ lyrics }: LyricsInfoProps): React.ReactElement => {
		return (
			<>
				{lyrics.translationType}
				{lyrics.cultureCodes && (
					<>
						{' '}
						(
						{userLanguageCultures[lyrics.cultureCodes[0]]?.englishName ??
							'Other'}
						)
					</>
				)}
				{lyrics.source && <> ({lyrics.source})</>}
			</>
		);
	},
);
