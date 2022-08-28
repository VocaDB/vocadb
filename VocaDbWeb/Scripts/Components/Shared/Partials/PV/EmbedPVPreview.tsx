import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { useVdbPlayer } from '@/Components/VdbPlayer/VdbPlayerContext';
import { useContextMenu } from '@/Components/useContextMenu';
import { EntryContract } from '@/DataContracts/EntryContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { PlayQueueItem } from '@/Stores/VdbPlayer/PlayQueueStore';
import { reaction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';

interface EmbedPVPreviewProps {
	entry: EntryContract;
	pv: PVContract;
	width?: number;
	height?: number;
	allowInline?: boolean;
}

export const EmbedPVPreview = observer(
	({
		entry,
		pv,
		width = 560,
		height = 315,
		allowInline,
	}: EmbedPVPreviewProps): React.ReactElement => {
		const embedPVPreviewRef = React.useRef<HTMLDivElement>(undefined!);
		const { vdbPlayer, playQueue } = useVdbPlayer();

		const handleResize = React.useCallback(() => {
			if (!allowInline) return;

			if (pv.id === playQueue.currentItem?.pv.id) {
				const rect = embedPVPreviewRef.current.getBoundingClientRect();

				vdbPlayer.setPlayerBounds({
					x: rect.x + window.scrollX,
					y: rect.y + window.scrollY,
					width: rect.width,
					height: rect.height,
				});
			} else {
				vdbPlayer.setPlayerBounds(undefined);
			}
		}, [allowInline, pv, vdbPlayer, playQueue]);

		const handleClick = React.useCallback(() => {
			playQueue.clearAndPlay(new PlayQueueItem(entry, pv));

			handleResize();
		}, [entry, pv, playQueue, handleResize]);

		React.useLayoutEffect(() => {
			window.addEventListener('resize', handleResize);

			handleResize();

			return (): void => {
				window.removeEventListener('resize', handleResize);
			};
		}, [handleResize]);

		React.useLayoutEffect(() => {
			return (): void => {
				vdbPlayer.setPlayerBounds(undefined);
			};
		}, [vdbPlayer]);

		React.useLayoutEffect(() => {
			return reaction(() => playQueue.currentItem?.pv.id, handleResize);
		}, [playQueue, handleResize]);

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
					ref={embedPVPreviewRef}
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
