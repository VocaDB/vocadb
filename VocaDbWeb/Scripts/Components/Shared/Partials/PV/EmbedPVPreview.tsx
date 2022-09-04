import Button from '@/Bootstrap/Button';
import ButtonGroup from '@/Bootstrap/ButtonGroup';
import Dropdown from '@/Bootstrap/Dropdown';
import { useVdbPlayer } from '@/Components/VdbPlayer/VdbPlayerContext';
import { EntryContract } from '@/DataContracts/EntryContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { AlbumHelper } from '@/Helpers/AlbumHelper';
import { EntryType } from '@/Models/EntryType';
import { AlbumRepository } from '@/Repositories/AlbumRepository';
import { HttpClient } from '@/Shared/HttpClient';
import { PlayMethod, PlayQueueItem } from '@/Stores/VdbPlayer/PlayQueueStore';
import { MoreHorizontal20Filled, Play20Filled } from '@fluentui/react-icons';
import { reaction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';

const httpClient = new HttpClient();

const albumRepo = new AlbumRepository(httpClient, vdb.values.baseAddress);

interface EmbedPVPreviewButtonsProps {
	onPlay: (method: PlayMethod) => void;
	onToggle?: (
		isOpen: boolean,
		event: React.SyntheticEvent,
		metadata: { source: 'select' | 'click' | 'rootClose' | 'keydown' },
	) => void;
}

export const EmbedPVPreviewButtons = React.memo(
	({ onPlay, onToggle }: EmbedPVPreviewButtonsProps): React.ReactElement => {
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
						<Dropdown.Item onClick={(): void => onPlay(PlayMethod.PlayNext)}>
							Play next{/* TODO: localize */}
						</Dropdown.Item>
						<Dropdown.Item
							onClick={(): void => onPlay(PlayMethod.AddToPlayQueue)}
						>
							Add to play queue{/* TODO: localize */}
						</Dropdown.Item>
					</Dropdown.Menu>
				</Dropdown>
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

		const handlePlay = React.useCallback(
			async (method: PlayMethod) => {
				if (entry.entryType === EntryType[EntryType.Album]) {
					const albumWithPVsAndTracks = await albumRepo.getOneWithPVsAndTracks({
						id: entry.id,
						lang: vdb.values.languagePreference,
					});

					const items = AlbumHelper.createPlayQueueItems(albumWithPVsAndTracks);

					playQueue.play(method, ...items);
				} else {
					const item = new PlayQueueItem(entry, pv);

					playQueue.play(method, item);
				}

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
					width: width,
					height: height,
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
					<EmbedPVPreviewButtons onPlay={handlePlay} />
				)}
			</div>
		);
	},
);
