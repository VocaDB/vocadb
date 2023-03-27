import Button from '@/Bootstrap/Button';
import ButtonGroup from '@/Bootstrap/ButtonGroup';
import Dropdown from '@/Bootstrap/Dropdown';
import {
	usePlayQueue,
	useVdbPlayer,
} from '@/Components/VdbPlayer/VdbPlayerContext';
import { EntryContract } from '@/DataContracts/EntryContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { PlayMethod } from '@/Stores/VdbPlayer/PlayQueueStore';
import { MoreHorizontal20Filled, Play20Filled } from '@fluentui/react-icons';
import { reaction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';

interface EmbedPVPreviewDropdownProps {
	pv: PVContract | undefined;
	onPlay: (method: PlayMethod) => void;
	onToggle?: (
		isOpen: boolean,
		event: React.SyntheticEvent,
		metadata: { source: 'select' | 'click' | 'rootClose' | 'keydown' },
	) => void;
}

const EmbedPVPreviewDropdown = React.memo(
	({
		pv,
		onPlay,
		onToggle,
	}: EmbedPVPreviewDropdownProps): React.ReactElement => {
		return (
			<Dropdown
				as={ButtonGroup}
				drop="up"
				css={{ position: 'absolute', right: 8, bottom: 8 }}
				onToggle={onToggle}
			>
				<Dropdown.Toggle
					style={{
						padding: 0,
						width: 40,
						height: 40,
						borderRadius: '50%',
					}}
				>
					<span
						css={{
							display: 'flex',
							justifyContent: 'center',
							alignItems: 'center',
						}}
					>
						<MoreHorizontal20Filled />
					</span>
				</Dropdown.Toggle>
				<Dropdown.Menu>
					<Dropdown.Item onClick={(): void => onPlay(PlayMethod.PlayFirst)}>
						Play first{/* LOC */}
					</Dropdown.Item>
					<Dropdown.Item onClick={(): void => onPlay(PlayMethod.PlayNext)}>
						Play next{/* LOC */}
					</Dropdown.Item>
					<Dropdown.Item
						onClick={(): void => onPlay(PlayMethod.AddToPlayQueue)}
					>
						Add to play queue{/* LOC */}
					</Dropdown.Item>
				</Dropdown.Menu>
			</Dropdown>
		);
	},
);

interface EmbedPVPreviewButtonsProps {
	pv: PVContract | undefined;
	onPlay: (method: PlayMethod) => void;
	onToggle?: (
		isOpen: boolean,
		event: React.SyntheticEvent,
		metadata: { source: 'select' | 'click' | 'rootClose' | 'keydown' },
	) => void;
}

export const EmbedPVPreviewButtons = React.memo(
	({
		pv,
		onPlay,
		onToggle,
	}: EmbedPVPreviewButtonsProps): React.ReactElement => {
		return (
			<>
				<Button
					onClick={(): void => onPlay(PlayMethod.ClearAndPlay)}
					css={{ position: 'absolute', left: 8, bottom: 8 }}
					style={{
						padding: 0,
						width: 40,
						height: 40,
						borderRadius: '50%',
					}}
				>
					<span
						css={{
							display: 'flex',
							justifyContent: 'center',
							alignItems: 'center',
						}}
					>
						<Play20Filled />
					</span>
				</Button>

				<EmbedPVPreviewDropdown pv={pv} onPlay={onPlay} onToggle={onToggle} />
			</>
		);
	},
);

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
		const vdbPlayer = useVdbPlayer();
		const playQueue = usePlayQueue();

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

		const handlePlay = React.useCallback(
			async (method: PlayMethod) => {
				await playQueue.loadItemsAndPlay(method, entry, pv);

				handleResize();
			},
			[entry, pv, playQueue, handleResize],
		);

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

		return (
			<div
				className="pv-embed-preview"
				css={{
					display: 'inline-block',
					position: 'relative',
					maxWidth: width,
					maxHeight: height,
					width: '100%',
					aspectRatio: '16 / 9',
				}}
				ref={embedPVPreviewRef}
			>
				<div
					css={{
						width: '100%',
						height: '100%',
						backgroundColor: 'rgb(28, 28, 28)',
						backgroundSize: 'cover',
						backgroundPosition: 'center',
					}}
					style={{
						backgroundImage: `url(${entry.mainPicture?.urlOriginal})`,
					}}
				/>

				{(vdbPlayer.playerBounds === undefined ||
					pv.id !== playQueue.currentItem?.pv.id) && (
					<EmbedPVPreviewButtons pv={pv} onPlay={handlePlay} />
				)}
			</div>
		);
	},
);
