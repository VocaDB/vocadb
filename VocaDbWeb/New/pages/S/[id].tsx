import { apiFetch } from '@/Helpers/FetchApiHelper';
import EmbedPVPreview from '@/nostalgic-darling/EmbedPVPreview';
import { SongDetailsContract } from '@/types/DataContracts/Song/SongDetailsContract';
import { GetServerSideProps, InferGetServerSidePropsType } from 'next';
import { useRouter } from 'next/router';

export default function Page({ song }: InferGetServerSidePropsType<typeof getServerSideProps>) {
	const router = useRouter();

	return <EmbedPVPreview song={{ ...song.song, pvs: song.pvs }} />;
}

export const getServerSideProps: GetServerSideProps<{
	song: SongDetailsContract;
}> = async ({ query }) => {
	const res = await apiFetch(`/api/songs/${query.id}/details`);
	const song = await res.json();
	return { props: { song } };
};

