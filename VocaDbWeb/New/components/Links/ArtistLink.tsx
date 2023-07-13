import { ArtistApiContract } from '@/types/DataContracts/Artist/ArtistApiContract';
import { Anchor } from '@mantine/core';
import Link from 'next/link';
import EntryToolTip from '../ToolTips/EntryToolTip';

interface ArtistLinkProps {
	artist: ArtistApiContract;
}

export default function ArtistLink({ artist }: ArtistLinkProps) {
	return (
		<EntryToolTip entry="artist" artist={artist}>
			<Anchor component={Link} href={'/Ar/' + artist.id}>
				{artist.name}
			</Anchor>
		</EntryToolTip>
	);
}

