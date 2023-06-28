import { apiFetch } from '@/Helpers/FetchApiHelper';
import EmbedPVPreview from '@/nostalgic-darling/EmbedPVPreview';
import { PVContract } from '@/types/DataContracts/PVs/PVContract';
import { SongDetailsContract } from '@/types/DataContracts/Song/SongDetailsContract';
import { PVService } from '@/types/Models/PVs/PVService';
import { Button, Group } from '@mantine/core';
import { GetServerSideProps, InferGetServerSidePropsType } from 'next';
import { useRouter } from 'next/router';
import { useState } from 'react';

export default function Page({ song }: InferGetServerSidePropsType<typeof getServerSideProps>) {
	const router = useRouter();
	const [pv, setPv] = useState<PVContract | undefined>(undefined);

	return (
		<>
			{pv !== undefined && <EmbedPVPreview song={{ ...song.song, pvs: song.pvs }} pv={pv} />}
			<Group mt="md">
				{song.pvs.map((pv) => (
					<Button
						disabled={
							![
								PVService.Bilibili,
								PVService.Youtube,
								PVService.NicoNicoDouga,
							].includes(pv.service)
						}
						onClick={() => setPv(pv)}
						key={pv.url}
					>
						{pv.service}
					</Button>
				))}
			</Group>
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

