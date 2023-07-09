import { apiFetch, apiPost, authApiGet } from '@/Helpers/FetchApiHelper';
import EmbedPVPreview from '@/nostalgic-darling/EmbedPVPreview';
import { SongDetailsContract } from '@/types/DataContracts/Song/SongDetailsContract';
import { SongVoteRating, parseSongVoteRating } from '@/types/Models/SongVoteRating';
import { ActionIcon, Group } from '@mantine/core';
import { IconHeart, IconThumbUp } from '@tabler/icons-react';
import { GetServerSideProps, InferGetServerSidePropsType } from 'next';
import useSWR from 'swr';

interface SongActionsProps {
	details: SongDetailsContract;
}

const SongActions = ({ details }: SongActionsProps) => {
	const { data, mutate } = useSWR(
		'/api/users/current/ratedSongs/' + details.song.id,
		authApiGet<string>
	);

	const postNewRating = async (newRating: SongVoteRating) => {
		await apiPost(`/api/songs/${details.song.id}/ratings`, {
			rating: SongVoteRating[newRating],
		});
	};

	const setRating = async (clickedRating: SongVoteRating) => {
		const newRating =
			SongVoteRating[clickedRating] === data ? SongVoteRating.Nothing : clickedRating;
		await postNewRating(newRating);
		mutate(SongVoteRating[newRating]);
	};

	const rating = parseSongVoteRating(data ?? SongVoteRating[SongVoteRating.Nothing]);

	return (
		<Group>
			<ActionIcon
				variant={rating === SongVoteRating.Like ? 'filled' : undefined}
				onClick={() => setRating(SongVoteRating.Like)}
				color="default"
				radius="xl"
				size="lg"
			>
				<IconThumbUp />
			</ActionIcon>
			<ActionIcon
				variant={rating === SongVoteRating.Favorite ? 'filled' : undefined}
				onClick={() => setRating(SongVoteRating.Favorite)}
				color="default"
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

