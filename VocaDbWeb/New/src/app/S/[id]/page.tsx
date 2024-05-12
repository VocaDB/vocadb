import { SongDetails } from "@/api/types/song";
import ExamplePage from "@/components/forms/page";
import Image from "next/image";

async function getSongData(id: number) {
	const res = await fetch(
		`${process.env.NEXT_PUBLIC_API_URL}/api/songs/${id}/details`,
	);
	return (await res.json()) as SongDetails;
}

interface PageParams {
	params: {
		id: number;
	};
}

export const generateMetadata = async ({ params }: PageParams) => {
	const songData = await getSongData(params.id);

	return {
		title: songData.song.name,
		description:
			"Original song by Twinfield feat. 初音ミク, published 3/23/2024", // TODO: Make this non-static
		openGraph: {
			images: [
				{
					url: songData.song.mainPicture?.urlOriginal,
				},
			],
		},
	};
};

export default async function Page({ params: { id } }: PageParams) {
	const data = await getSongData(id);

	return (
		<>
			<h1>{data.song.name}</h1>
			{data.song.mainPicture?.urlOriginal && (
				<Image
					width={480}
					height={360}
					// @ts-ignore
					src={`https://vocadb.net/api/pvs/thumbnail?pvUrl=${data.pvs[0].url}`}
					/// src={data.song.mainPicture.urlOriginal}
					alt={""}
				/>
			)}
			<ExamplePage />
		</>
	);
}

