import { apiFetch, apiPost } from '@/Helpers/FetchApiHelper';
import EmbedPVPreview from '@/nostalgic-darling/EmbedPVPreview';
import { SongApiContract } from '@/types/DataContracts/Song/SongApiContract';
import { SongDetailsContract } from '@/types/DataContracts/Song/SongDetailsContract';
import { SongVoteRating, parseSongVoteRating } from '@/types/Models/SongVoteRating';
import { ActionIcon, Group } from '@mantine/core';
import { IconHeart, IconThumbUp } from '@tabler/icons-react';
import { GetServerSideProps, InferGetServerSidePropsType } from 'next';
import { useState } from 'react';

interface SongActionsProps {
	details: SongDetailsContract;
}

const SongActions = ({ details }: SongActionsProps) => {
	const [userRating, setUserRating] = useState<SongVoteRating>(
		parseSongVoteRating(details.userRating)
	);

	const setRating = (newRating: SongVoteRating): void => {
		apiPost(`/api/songs/${details.song.id}/ratings`, { rating: SongVoteRating[newRating] });
	};

	return (
		<Group>
			<ActionIcon
				variant={userRating === SongVoteRating.Like ? 'filled' : undefined}
				radius="xl"
				size="lg"
			>
				<IconThumbUp />
			</ActionIcon>
			<ActionIcon
				variant={userRating === SongVoteRating.Favorite ? 'filled' : undefined}
				radius="xl"
				size="lg"
			>
				<IconHeart />
			</ActionIcon>
		</Group>
	);
};

export default function Page({ song }: InferGetServerSidePropsType<typeof getServerSideProps>) {
	return (
		<>
			<div style={{ marginRight: 'auto', marginLeft: 'auto', maxWidth: 'max-content' }}>
				{song.pvs.length > 0 && (
					<EmbedPVPreview song={{ ...song.song, pvs: song.pvs }} pv={song.pvs[0]} />
				)}
				<SongActions details={song} />
			</div>
		</>
	);
}

export const getServerSideProps: GetServerSideProps<{
	song: SongDetailsContract;
}> = async ({ query }) => {
	const res = await apiFetch(`/api/songs/${query.id}/details`);
	const song = await res.json();
	return { props: { song } };
};

