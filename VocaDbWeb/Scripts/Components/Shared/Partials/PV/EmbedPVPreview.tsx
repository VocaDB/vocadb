import EntryContract from '@DataContracts/EntryContract';
import PVContract from '@DataContracts/PVs/PVContract';
import React from 'react';

import { useVdbPlayer } from '../../../VdbPlayer/VdbPlayerContext';

interface EmbedPVPreviewProps {
	entry: EntryContract;
	pv: PVContract;
	width?: number;
	height?: number;
	autoplay?: boolean;
	enableApi?: boolean;
	id?: string;
}

const EmbedPVPreview = ({
	entry,
	pv,
	width = 560,
	height = 315,
}: EmbedPVPreviewProps): React.ReactElement => {
	const vdbPlayer = useVdbPlayer();

	return (
		<div
			className="pv-embed-preview"
			css={{
				display: 'inline-block',
				width: width,
				height: height,
				backgroundColor: 'rgb(39, 39, 39)',
				backgroundImage: `url(${entry.mainPicture?.urlOriginal})`,
				backgroundSize: 'cover',
				backgroundPosition: 'center',
				cursor: 'pointer',
			}}
			onClick={(): void =>
				vdbPlayer.selectEntry({
					entry: entry,
					pv: pv,
				})
			}
		/>
	);
};

export default EmbedPVPreview;
