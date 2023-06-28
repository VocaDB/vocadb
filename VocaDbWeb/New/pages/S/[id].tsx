import { apiFetch } from '@/Helpers/FetchApiHelper';
import EmbedPVPreview from '@/nostalgic-darling/EmbedPVPreview';
import { PVContract } from '@/types/DataContracts/PVs/PVContract';
import { SongDetailsContract } from '@/types/DataContracts/Song/SongDetailsContract';
import { Button } from '@mantine/core';
import { GetServerSideProps, InferGetServerSidePropsType } from 'next';
import { useRouter } from 'next/router';
import { useState } from 'react';

export default function Page({ song }: InferGetServerSidePropsType<typeof getServerSideProps>) {
	const router = useRouter();
	const [pv, setPv] = useState<PVContract | undefined>(undefined);

	return (
		<>
			{pv !== undefined && <EmbedPVPreview song={{ ...song.song, pvs: song.pvs }} pv={pv} />}
			{song.pvs.map((pv) => (
				<Button onClick={() => setPv(pv)} key={pv.url}>
					{pv.service}
				</Button>
			))}
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

