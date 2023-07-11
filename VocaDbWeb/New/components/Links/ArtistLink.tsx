import { ArtistApiContract } from '@/types/DataContracts/Artist/ArtistApiContract';
import { Anchor } from '@mantine/core';
import Link from 'next/link';
import { ArtistToolTip } from '../ToolTips/ArtistToolTip';

interface ArtistLinkProps {
	artist: ArtistApiContract;
}

export function ArtistLink({ artist }: ArtistLinkProps) {
	return (
		<ArtistToolTip artist={artist}>
			<Anchor component={Link} href={'/Ar/' + artist.id}>
				{artist.name}
			</Anchor>
		</ArtistToolTip>
	);
}

