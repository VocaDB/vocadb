import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { useVdbPlayer } from '@/Components/VdbPlayer/VdbPlayerContext';
import { useContextMenu } from '@/Components/useContextMenu';
import { EntryContract } from '@/DataContracts/EntryContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { PlayQueueItem } from '@/Stores/VdbPlayer/PlayQueueStore';
import React from 'react';

interface EmbedPVPreviewProps {
	entry: EntryContract;
	pv: PVContract;
	width?: number;
	height?: number;
}

export const EmbedPVPreview = React.memo(
	({
		entry,
		pv,
		width = 560,
		height = 315,
	}: EmbedPVPreviewProps): React.ReactElement => {
		const { playQueue } = useVdbPlayer();

		const handleClick = React.useCallback(() => {
			playQueue.clearAndPlay(new PlayQueueItem(entry, pv));
		}, [entry, pv, playQueue]);

		const contextMenuRef = React.useRef<HTMLUListElement>(undefined!);
		const contextMenu = useContextMenu(contextMenuRef);

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
					onClick={handleClick}
					onContextMenu={contextMenu.handleContextMenu}
				/>

				{contextMenu.show && (
					<ul
						ref={contextMenuRef}
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
									playQueue.clearAndPlay(new PlayQueueItem(entry, pv));
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
									playQueue.playNext(new PlayQueueItem(entry, pv));
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
									playQueue.addToQueue(new PlayQueueItem(entry, pv));
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
