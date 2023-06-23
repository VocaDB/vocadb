import { GetServerSideProps, InferGetServerSidePropsType } from 'next';
import { FrontPageContract } from '@/types/DataContracts/FrontPageContract';
import { apiFetch } from '@/Helpers/FetchApiHelper';
import { HighlightedSongsCarousel } from '@/components/Frontpage/HighlightedSongsCarousel';
import { Alert, Anchor, Avatar, Button, Group, Loader, Progress, Title } from '@mantine/core';
import { AlbumCards } from '@/components/AlbumCard/AlbumCard';
import { IconAlertCircle } from '@tabler/icons-react';

export default function HomePage({
	frontPage,
}: InferGetServerSidePropsType<typeof getServerSideProps>) {
	return (
		<div style={{ display: 'flex', flexDirection: 'column' }}>
			<Group>
				<Button variant="light">Test Light</Button>
				<Button variant="outline">Test Outline</Button>
				<Button variant="filled">Test Filled</Button>
				<Anchor href="https://vocadb.net" target="_blank">
					This is a link
				</Anchor>
				<Loader />
				<Avatar color="default" radius="xl">
					MK
				</Avatar>
				<Progress w="300px" value={50} />
			</Group>
			<Alert
				icon={<IconAlertCircle size="1rem" />}
				title="Color Demo!"
				my="md"
				color="default"
			>
				Looooooooooooooooooong text
			</Alert>
			<Title order={2} mb="xs">
				Highlighted PVs
			</Title>
			<HighlightedSongsCarousel songs={frontPage.newSongs} />
			<Title mt="md" mb="xs" order={2}>
				Random popular albums
			</Title>
			<AlbumCards albums={frontPage.topAlbums} />
			<Title mt="md" mb="xs" order={2}>
				Recent or upcoming albums
			</Title>
			<AlbumCards albums={frontPage.newAlbums} />
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

