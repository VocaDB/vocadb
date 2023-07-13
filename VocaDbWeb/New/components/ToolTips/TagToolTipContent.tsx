import { apiGet } from '@/Helpers/FetchApiHelper';
import { TagApiContract } from '@/types/DataContracts/Tag/TagApiContract';
import { TagBaseContract } from '@/types/DataContracts/Tag/TagBaseContract';
import { Stack, Text } from '@mantine/core';
import useSWR from 'swr';

export interface TagToolTipProps {
	entry: 'tag';
	tag: TagBaseContract;
}

export default function TagToolTipContent({ tag }: TagToolTipProps) {
	const { data } = useSWR(
		`/api/tags/${tag.id}?fields=MainPicture,AdditionalNames,Description`,
		apiGet<TagApiContract>
	);

	return (
		<Stack maw="20vw">
			{data === undefined ? (
				<div style={{ width: 70 }} />
			) : (
				<>
					<div>
						<Text weight={500}>{data.name}</Text>
						<Text color="dimmed">{data.additionalNames}</Text>
					</div>
					<Text>{data.categoryName}</Text>
					<Text lineClamp={2}>{data.description}</Text>
				</>
			)}
		</Stack>
	);
}

