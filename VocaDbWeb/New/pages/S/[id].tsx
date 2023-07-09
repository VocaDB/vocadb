import { apiFetch, apiGet, apiPost, authApiGet } from '@/Helpers/FetchApiHelper';
import { useVdb } from '@/components/Context/VdbContext';
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
	const { values } = useVdb();
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

	const SongVoteButton = (buttonRating: SongVoteRating, icon: JSX.Element) => (
		<ActionIcon
			variant={rating === buttonRating ? 'filled' : undefined}
			onClick={() => setRating(buttonRating)}
			color="default"
			radius="xl"
			size="lg"
			disabled={!values.isLoggedIn}
		>
			{icon}
		</ActionIcon>
	);

	return (
		<Group mt="sm">
			{SongVoteButton(SongVoteRating.Like, <IconThumbUp />)}
			{SongVoteButton(SongVoteRating.Favorite, <IconHeart />)}
		</Group>
	);
};

export default function Page({ song }: InferGetServerSidePropsType<typeof getServerSideProps>) {
	return (
		<>
			<div
				style={{
					marginRight: 'auto',
					marginLeft: 'auto',
					maxWidth: 'max-content',
				}}
			>
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
	const song = await apiGet<SongDetailsContract>(`/api/songs/${query.id}/details`);
	return { props: { song } };
};

