import { apiGet } from '@/Helpers/FetchApiHelper';
import { unmark } from '@/Helpers/markdown';
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
		<Stack>
			{data === undefined ? (
				<div style={{ width: 70 }} />
			) : (
				<>
					<div>
						<Text weight={500}>{data.name}</Text>
						<Text color="dimmed">
							{data.additionalNames
								?.split(',')
								.filter((_tag, index) => index < 3)
								.join(',')}
						</Text>
					</div>
					<Text>{data.categoryName}</Text>
					<Text lineClamp={2}>{unmark(data.description)}</Text>
				</>
			)}
		</Stack>
	);
}

