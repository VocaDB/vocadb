import { ColorSchemeToggle } from '../components/ColorSchemeToggle/ColorSchemeToggle';
import { GetServerSideProps, InferGetServerSidePropsType } from 'next';
import { FrontPageContract } from '@/types/DataContracts/FrontPageContract';
import { apiFetch } from '@/Helpers/FetchApiHelper';
import { HighlightedSongsCarousel } from '@/components/Frontpage/HighlightedSongsCarousel';
import { Title } from '@mantine/core';
import { AlbumCards } from '@/components/AlbumCard/AlbumCard';

export default function HomePage({
	frontPage,
}: InferGetServerSidePropsType<typeof getServerSideProps>) {
	return (
		<div style={{ display: 'flex', flexDirection: 'column' }}>
			<Title order={2} mb="xs">
				Highlighted PVs
			</Title>
			<HighlightedSongsCarousel songs={frontPage.newSongs} />
			<Title order={2} mt="md" mb="xs">
				Recent or upcoming albums
			</Title>
			<AlbumCards albums={frontPage.newAlbums} />
			<Title order={2} mt="md" mb="xs">
				Random popular albums
			</Title>
			<AlbumCards albums={frontPage.topAlbums} />
		</div>
	);
}

export const getServerSideProps: GetServerSideProps<{
	frontPage: FrontPageContract;
}> = async ({ res }) => {
	res.setHeader('Cache-Control', 'public, s-maxage=30, stale-while-revalidate=300');

	const apiResp = await apiFetch('/api/frontpage');
	const frontPage = await apiResp.json();

	return { props: { frontPage } };
};

