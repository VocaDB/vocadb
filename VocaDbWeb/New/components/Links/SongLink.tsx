import EntryToolTip from '../ToolTips/EntryToolTip';
import { Anchor } from '@mantine/core';
import Link from 'next/link';
import { SongApiContract } from '@/types/DataContracts/Song/SongApiContract';

interface SongLinkProps {
	song: SongApiContract;
}

export default function SongLink({ song }: SongLinkProps) {
	return (
		<EntryToolTip entry="song" song={song}>
			<Anchor component={Link} href={'/S/' + song.id}>
				{song.name}
			</Anchor>
		</EntryToolTip>
	);
}

