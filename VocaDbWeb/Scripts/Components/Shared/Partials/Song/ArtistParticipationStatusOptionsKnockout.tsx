import Button from '@/Bootstrap/Button';
import React from 'react';

enum ArtistAlbumParticipationStatus {
	Everything = 'Everything',
	OnlyMainAlbums = 'OnlyMainAlbums',
	OnlyCollaborations = 'OnlyCollaborations',
}

const options = {
	[ArtistAlbumParticipationStatus.Everything]: 'Everything' /* LOC */,
	[ArtistAlbumParticipationStatus.OnlyMainAlbums]: 'Only main songs' /* LOC */,
	[ArtistAlbumParticipationStatus.OnlyCollaborations]:
		'Only collaborations' /* LOC */,
};

interface ArtistParticipationStatusOptionsKnockoutProps {
	activeKey: string;
	onSelect: (eventKey: string) => void;
}

export const ArtistParticipationStatusOptionsKnockout = React.memo(
	({
		activeKey,
		onSelect,
	}: ArtistParticipationStatusOptionsKnockoutProps): React.ReactElement => {
		return (
			<div style={{ display: 'inline-block' }}>
				{Object.entries(options).map(([key, value], index) => (
					<React.Fragment key={key}>
						{index > 0 && ' '}
						<Button
							onClick={(): void => onSelect(key)}
							disabled={key === activeKey}
						>
							{value}
						</Button>
					</React.Fragment>
				))}
			</div>
		);
	},
);
