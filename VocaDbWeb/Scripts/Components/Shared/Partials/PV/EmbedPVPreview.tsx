import SafeAnchor from '@Bootstrap/SafeAnchor';
import EntryContract from '@DataContracts/EntryContract';
import PVContract from '@DataContracts/PVs/PVContract';
import React from 'react';

import { useVdbPlayer } from '../../../VdbPlayer/VdbPlayerContext';
import useContextMenu from '../../../useContextMenu';

interface EmbedPVPreviewProps {
	entry: EntryContract;
	pv: PVContract;
	width?: number;
	height?: number;
	autoplay?: boolean;
	enableApi?: boolean;
	id?: string;
}

const EmbedPVPreview = React.memo(
	({
		entry,
		pv,
		width = 560,
		height = 315,
	}: EmbedPVPreviewProps): React.ReactElement => {
		const vdbPlayer = useVdbPlayer();

		const ref = React.useRef<HTMLUListElement>(undefined!);
		const contextMenu = useContextMenu(ref);

		return (
			<>
				<div
					className="pv-embed-preview"
					css={{
						display: 'inline-block',
						width: width,
						height: height,
						backgroundColor: 'rgb(28, 28, 28)',
						backgroundImage: `url(${entry.mainPicture?.urlOriginal})`,
						backgroundSize: 'cover',
						backgroundPosition: 'center',
						cursor: 'pointer',
					}}
					onClick={(): void => vdbPlayer.selectEntry({ entry: entry, pv: pv })}
					onContextMenu={contextMenu.handleContextMenu}
				/>

				{contextMenu.show && (
					<ul
						ref={ref}
						className="dropdown-menu"
						role="menu"
						css={{ display: 'block', position: 'fixed' }}
						style={{
							left: contextMenu.position.x,
							top: contextMenu.position.y,
						}}
					>
						<li>
							<SafeAnchor
								href="#"
								style={{ color: 'inherit', margin: 'inherit' }}
								onClick={(): void => {
									contextMenu.setShow(false);
									vdbPlayer.selectEntry({ entry: entry, pv: pv });
								}}
							>
								Play{/* TODO: localize */}
							</SafeAnchor>
						</li>
						<li>
							<SafeAnchor
								href="#"
								style={{ color: 'inherit', margin: 'inherit' }}
								onClick={(): void => {
									contextMenu.setShow(false);
								}}
							>
								Play next{/* TODO: localize */}
							</SafeAnchor>
						</li>
						<li>
							<SafeAnchor
								href="#"
								style={{ color: 'inherit', margin: 'inherit' }}
								onClick={(): void => {
									contextMenu.setShow(false);
								}}
							>
								Add to play queue{/* TODO: localize */}
							</SafeAnchor>
						</li>
					</ul>
				)}
			</>
		);
	},
);

export default EmbedPVPreview;
