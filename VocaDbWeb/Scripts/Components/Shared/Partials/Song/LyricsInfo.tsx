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
				{lyrics.cultureCode && (
					<> ({userLanguageCultures[lyrics.cultureCode].englishName})</>
				)}
				{lyrics.source && <> ({lyrics.source})</>}
			</>
		);
	},
);
