import { ColorSchemeToggle } from '../components/ColorSchemeToggle/ColorSchemeToggle';
import { GetServerSideProps, InferGetServerSidePropsType } from 'next';
import { FrontPageContract } from '@/types/DataContracts/FrontPageContract';
import { apiFetch } from '@/Helpers/FetchApiHelper';
import { HighlightedSongsCarousel } from '@/components/Frontpage/HighlightedSongsCarousel';
import { Grid, Stack, Title } from '@mantine/core';
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
			<Grid mt="md" justify="center" style={{ width: '100%' }}>
				<Grid.Col md={5} xs={12}>
					<Stack align="center">
						<Title align="center" order={2}>
							Recent or upcoming albums
						</Title>

						<AlbumCards albums={frontPage.topAlbums} />
					</Stack>
				</Grid.Col>
				<Grid.Col md={5} xs={12} offsetMd={1}>
					<Stack align="center">
						<Title order={2}>Random popular albums</Title>

						<AlbumCards albums={frontPage.newAlbums} />
					</Stack>
				</Grid.Col>
			</Grid>
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

