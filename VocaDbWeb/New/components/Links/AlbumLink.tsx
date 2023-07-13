import EntryToolTip from '../ToolTips/EntryToolTip';
import { Anchor } from '@mantine/core';
import Link from 'next/link';
import { AlbumForApiContract } from '@/types/DataContracts/Album/AlbumForApiContract';

interface AlbumLinkProps {
	album: AlbumForApiContract;
}

export default function AlbumLink({ album }: AlbumLinkProps) {
	return (
		<EntryToolTip entry="album" album={album}>
			<Anchor component={Link} href={'/Al/' + album.id}>
				{album.name}
			</Anchor>
		</EntryToolTip>
	);
}

