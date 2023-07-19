import { TagBaseContract } from '@/types/DataContracts/Tag/TagBaseContract';
import EntryToolTip from '../ToolTips/EntryToolTip';
import { Anchor } from '@mantine/core';
import Link from 'next/link';

interface TagLinkProps {
	tag: TagBaseContract;
}

export default function TagLink({ tag }: TagLinkProps) {
	return (
		<EntryToolTip entry="tag" tag={tag}>
			<Anchor component={Link} href={'/T/' + tag.id}>
				{tag.name}
			</Anchor>
		</EntryToolTip>
	);
}
