import { SongPopupContent } from '@/Components/Shared/SongPopupContent';
import { SongWithPVAndVoteContract } from '@/DataContracts/Song/SongWithPVAndVoteContract';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface SongWithVotePopupContentProps {
	song: SongWithPVAndVoteContract;
}

export const SongWithVotePopupContent = ({
	song,
}: SongWithVotePopupContentProps): React.ReactElement => {
	const { t } = useTranslation(['Resources']);

	return (
		<>
			<SongPopupContent song={song} />

			{song.vote !== 'Nothing' && (
				<p>{t(`Resources:SongVoteRatingNames.${song.vote}`)}</p>
			)}
		</>
	);
};
