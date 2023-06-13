import { ColorSchemeToggle } from '../components/ColorSchemeToggle/ColorSchemeToggle';
import { GetServerSideProps, InferGetServerSidePropsType } from 'next';
import { FrontPageContract } from '@/types/DataContracts/FrontPageContract';
import { apiFetch } from '@/Helpers/FetchApiHelper';
import { HighlightedSongsCarousel } from '@/components/Frontpage/HighlightedSongsCarousel';
import { Title } from '@mantine/core';

export default function HomePage({
	frontPage,
}: InferGetServerSidePropsType<typeof getServerSideProps>) {
	return (
		<div style={{ display: 'flex', flexDirection: 'column' }}>
			<Title order={2}>Highlighted PVs</Title>
			<HighlightedSongsCarousel songs={frontPage.newSongs} />
			<ColorSchemeToggle />
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

