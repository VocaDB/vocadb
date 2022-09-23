import Button from '@/Bootstrap/Button';
import ButtonGroup from '@/Bootstrap/ButtonGroup';
import Container from '@/Bootstrap/Container';
import Dropdown from '@/Bootstrap/Dropdown';
import { PVServiceIcon } from '@/Components/Shared/Partials/Shared/PVServiceIcon';
import { EmbedPV } from '@/Components/VdbPlayer/EmbedPV';
import { VdbPlayerConsole } from '@/Components/VdbPlayer/VdbPlayerConsole';
import { useVdbPlayer } from '@/Components/VdbPlayer/VdbPlayerContext';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { VideoServiceHelper } from '@/Helpers/VideoServiceHelper';
import { PVService } from '@/Models/PVs/PVService';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { PlayQueueEntryContract } from '@/Stores/VdbPlayer/PlayQueueRepository';
import { PlayQueueItem } from '@/Stores/VdbPlayer/PlayQueueStore';
import { RepeatMode } from '@/Stores/VdbPlayer/VdbPlayerStore';
import { css } from '@emotion/react';
import { MoreHorizontal20Filled } from '@fluentui/react-icons';
import {
	IPlayerApi,
	PlayerOptions,
	TimeEvent,
	useNostalgicDiva,
} from '@vocadb/nostalgic-diva';
import { useLocalStorageStateStore } from '@vocadb/route-sphere';
import classNames from 'classnames';
import { reaction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { Link } from 'react-router-dom';

export const miniPlayerWidth = 16 * 25;
export const miniPlayerHeight = 9 * 25;

const seekBarHeight = 8;
const controlsHeight = 48;
export const bottomBarHeight = seekBarHeight + controlsHeight;

const repeatIcons: Record<RepeatMode, string> = {
	[RepeatMode.Off]: 'icon-ban-circle',
	[RepeatMode.All]: 'icon-refresh',
	[RepeatMode.One]: 'icon-repeat',
};

const PlayerCenterControls = observer(
	(): React.ReactElement => {
		const diva = useNostalgicDiva();
		const { vdbPlayer, playQueue } = useVdbPlayer();

		const handlePause = React.useCallback(async () => {
			await diva.pause();
		}, [diva]);

		const handlePlay = React.useCallback(async () => {
			await diva.play();
		}, [diva]);

		return (
			<ButtonGroup>
				<Button
					variant="inverse"
					title={
						`Coming soon!` /* TODO: Remove. */ /* TODO: `Shuffle: ${vdbPlayer.shuffle ? 'On' : 'Off'}${
							vdbPlayer.canAutoplay
								? ''
								: ' (Unavailable for this video service)'
						}`*/ /* TODO: localize */
					}
					onClick={vdbPlayer.toggleShuffle}
					disabled={true /* TODO: !vdbPlayer.canAutoplay */}
					className={classNames('hidden-phone', vdbPlayer.shuffle && 'active')}
				>
					<i className="icon-random icon-white" />
				</Button>
				<Button
					variant="inverse"
					title="Previous" /* TODO: localize */
					onClick={playQueue.previous}
					disabled={!playQueue.hasPreviousItem}
				>
					<i className="icon-step-backward icon-white" />
				</Button>
				{vdbPlayer.playing ? (
					<Button
						variant="inverse"
						title="Pause" /* TODO: localize */
						onClick={handlePause}
						disabled={!vdbPlayer.canAutoplay}
					>
						<i className="icon-pause icon-white" />
					</Button>
				) : (
					<Button
						variant="inverse"
						title={`Play${
							vdbPlayer.canAutoplay
								? ''
								: ' (Unavailable for this video service)'
						}`} /* TODO: localize */
						onClick={handlePlay}
						disabled={!vdbPlayer.canAutoplay}
					>
						<i className="icon-play icon-white" />
					</Button>
				)}
				<Button
					variant="inverse"
					title="Next" /* TODO: localize */
					onClick={playQueue.next}
					disabled={!playQueue.hasNextItem}
				>
					<i className="icon-step-forward icon-white" />
				</Button>
				<Button
					variant="inverse"
					title={
						`Repeat: ${vdbPlayer.repeat}${
							vdbPlayer.canAutoplay
								? ''
								: ' (Unavailable for this video service)'
						}` /* TODO: localize */
					}
					onClick={vdbPlayer.toggleRepeat}
					disabled={!vdbPlayer.canAutoplay}
					className="hidden-phone"
				>
					<i
						className={classNames(repeatIcons[vdbPlayer.repeat], 'icon-white')}
					/>
				</Button>
			</ButtonGroup>
		);
	},
);

interface EntryInfoProps {
	entry: PlayQueueEntryContract;
}

const EntryInfo = observer(
	({ entry }: EntryInfoProps): React.ReactElement => {
		return (
			<div css={{ display: 'flex', alignItems: 'center', width: '100%' }}>
				<Link to={EntryUrlMapper.details_entry(entry)} css={{ marginRight: 8 }}>
					<div
						css={{
							width: 64,
							height: 36,
							backgroundColor: 'rgb(28, 28, 28)',
							backgroundSize: 'cover',
							backgroundPosition: 'center',
						}}
						style={{
							backgroundImage: `url(${entry.urlThumb})`,
						}}
					/>
				</Link>

				<div
					css={{
						flexGrow: 1,
						display: 'flex',
						minWidth: 0,
						flexDirection: 'column',
					}}
				>
					<Link
						to={EntryUrlMapper.details_entry(entry)}
						css={css`
							color: white;
							&:hover {
								color: white;
							}
							&:visited {
								color: white;
							}
							font-weight: bold;
							overflow: hidden;
							text-overflow: ellipsis;
							white-space: nowrap;
						`}
					>
						{entry.name}
					</Link>
					{(entry.entryType === 'Album' || entry.entryType === 'Song') && (
						<div css={{ display: 'flex' }}>
							<span
								css={{
									color: '#999999',
									overflow: 'hidden',
									textOverflow: 'ellipsis',
									whiteSpace: 'nowrap',
								}}
							>
								{entry.artistString}
							</span>
						</div>
					)}
				</div>
			</div>
		);
	},
);

interface PVServiceDropdownProps {
	item: PlayQueueItem;
}

const PVServiceDropdown = observer(
	({ item }: PVServiceDropdownProps): React.ReactElement => {
		const { playQueue } = useVdbPlayer();

		return (
			<Dropdown as={ButtonGroup} drop="up" css={{ marginLeft: 8 }}>
				<Dropdown.Toggle variant="inverse">
					<PVServiceIcon service={item.pv.service} />
				</Dropdown.Toggle>
				<Dropdown.Menu>
					{item.entry.pvs
						.filter((pv) => !pv.disabled)
						.map((pv) => (
							<Dropdown.Item
								onClick={async (): Promise<void> => {
									if (pv.id === item.pv.id) return;

									playQueue.switchPV(pv);
								}}
								key={pv.id}
							>
								{pv.id === item.pv.id ? (
									<i className="menuIcon icon-ok" />
								) : (
									<i className="menuIcon icon-" />
								)}{' '}
								<PVServiceIcon service={pv.service} /> {pv.service}
							</Dropdown.Item>
						))}
				</Dropdown.Menu>
			</Dropdown>
		);
	},
);

const PlayerRightControls = observer(
	(): React.ReactElement => {
		const diva = useNostalgicDiva();
		const { vdbPlayer, playQueue } = useVdbPlayer();

		const handleClickSkipIntro = React.useCallback(async () => {
			// TODO: Implement "Skip intro".
		}, []);

		const handleClickSkipBack10Seconds = React.useCallback(async () => {
			const { currentTime } = playQueue;
			if (currentTime !== undefined) {
				await diva.setCurrentTime(currentTime - 10);
			}
		}, [diva, playQueue]);

		const handleClickSkipForward30Seconds = React.useCallback(async () => {
			const { currentTime } = playQueue;
			if (currentTime !== undefined) {
				await diva.setCurrentTime(currentTime + 30);
			}
		}, [diva, playQueue]);

		return (
			<>
				{playQueue.currentItem && (
					<>
						<ButtonGroup>
							<Button
								onClick={handleClickSkipIntro}
								disabled={!vdbPlayer.canAutoplay}
								variant="inverse"
								title="Skip intro" /* TODO: localize */
							>
								<img
									src="" /* TODO: Implement "Skip intro". */
									alt="Skip intro" /* TODO: localize */
									width={16}
									height={16}
								/>
							</Button>
						</ButtonGroup>
						<PVServiceDropdown item={playQueue.currentItem} />
					</>
				)}
				<Dropdown as={ButtonGroup} drop="up" css={{ marginLeft: 8 }}>
					<Dropdown.Toggle variant="inverse">
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
						<Dropdown.Item as={Link} to="/playlist">
							Show play queue{/* TODO: localize */}
						</Dropdown.Item>
						<Dropdown.Item
							onClick={handleClickSkipBack10Seconds}
							disabled={!vdbPlayer.canAutoplay}
						>
							Skip back 10 seconds{/* TODO: localize */}
						</Dropdown.Item>
						<Dropdown.Item
							onClick={handleClickSkipForward30Seconds}
							disabled={!vdbPlayer.canAutoplay}
						>
							Skip forward 30 seconds{/* TODO: localize */}
						</Dropdown.Item>
						<Dropdown.Item
							onClick={vdbPlayer.toggleShuffle}
							disabled={true /* TODO: !vdbPlayer.canAutoplay */}
							className="visible-phone"
							title="Coming soon!" /* TODO: Remove. */
						>
							{
								`Shuffle: ${
									vdbPlayer.shuffle ? 'On' : 'Off'
								}` /* TODO: localize */
							}
						</Dropdown.Item>
						<Dropdown.Item
							onClick={vdbPlayer.toggleRepeat}
							disabled={!vdbPlayer.canAutoplay}
							className="visible-phone"
						>
							{`Repeat: ${vdbPlayer.repeat}` /* TODO: localize */}
						</Dropdown.Item>
						<Dropdown.Item
							onClick={playQueue.clear}
							disabled={playQueue.isEmpty}
						>
							Clear play queue{/* TODO: localize */}
						</Dropdown.Item>
					</Dropdown.Menu>
				</Dropdown>
			</>
		);
	},
);

const PlayerControls = observer(
	(): React.ReactElement => {
		const { playQueue } = useVdbPlayer();

		return (
			<div
				css={{
					display: 'flex',
					height: controlsHeight,
					alignItems: 'center',
				}}
			>
				<div
					css={{
						display: 'flex',
						justifyContent: 'flex-start',
						alignItems: 'center',
						width: 'calc(100% / 3)',
						height: '100%',
					}}
				>
					<div css={{ maxWidth: '100%' }}>
						{playQueue.currentItem && playQueue.currentItem.entry && (
							<EntryInfo entry={playQueue.currentItem.entry} />
						)}
					</div>
				</div>
				<div
					css={{
						display: 'flex',
						justifyContent: 'center',
						alignItems: 'center',
						width: 'calc(100% / 3)',
						height: '100%',
					}}
				>
					<PlayerCenterControls />
				</div>
				<div
					css={{
						display: 'flex',
						justifyContent: 'flex-end',
						alignItems: 'center',
						width: 'calc(100% / 3)',
						height: '100%',
					}}
				>
					<PlayerRightControls />
				</div>
			</div>
		);
	},
);

interface PVPlayerProps {
	pv: PVContract;
}

const EmbedPVWrapper = observer(
	({ pv }: PVPlayerProps): React.ReactElement => {
		const diva = useNostalgicDiva();
		const { vdbPlayer, playQueue } = useVdbPlayer();

		const handleError = React.useCallback((event: any) => {
			VdbPlayerConsole.error('error', event);
		}, []);

		const handlePlay = React.useCallback(() => vdbPlayer.setPlaying(true), [
			vdbPlayer,
		]);

		const handlePause = React.useCallback(() => vdbPlayer.setPlaying(false), [
			vdbPlayer,
		]);

		const handleEnded = React.useCallback(async () => {
			VdbPlayerConsole.debug(
				`Playback ended (repeat mode: ${vdbPlayer.repeat})`,
			);

			switch (vdbPlayer.repeat) {
				case RepeatMode.One:
					await diva.setCurrentTime(0);
					break;

				case RepeatMode.Off:
				case RepeatMode.All:
					if (playQueue.isLastItem && !playQueue.hasMoreItems) {
						switch (vdbPlayer.repeat) {
							case RepeatMode.Off:
								vdbPlayer.setPlaying(false);
								break;

							case RepeatMode.All:
								if (playQueue.hasMultipleItems) {
									playQueue.goToFirst();
								} else {
									await diva.setCurrentTime(0);
								}
								break;
						}
					} else {
						await playQueue.next();
					}
					break;
			}
		}, [diva, vdbPlayer, playQueue]);

		const handleTimeUpdate = React.useCallback(
			async ({ percent, seconds }: TimeEvent) => {
				if (percent !== undefined) {
					vdbPlayer.setPercent(percent);
				}

				if (seconds !== undefined) {
					if (seconds > 0) {
						const startTime = playQueue.currentItem?.getAndClearStartTime();
						if (startTime !== undefined) {
							await diva.setCurrentTime(startTime);
						}
					}

					playQueue.currentTime = seconds;
				}
			},
			[diva, vdbPlayer, playQueue],
		);

		const options = React.useMemo(
			(): PlayerOptions => ({
				onError: handleError,
				onPlay: handlePlay,
				onPause: handlePause,
				onEnded: handleEnded,
				onTimeUpdate: handleTimeUpdate,
			}),
			[handleError, handlePlay, handlePause, handleEnded, handleTimeUpdate],
		);

		const handlePlayerChange = React.useCallback(
			async (player?: IPlayerApi) => {
				try {
					if (!player) return;

					const pvId =
						pv.service === PVService.Piapro
							? VideoServiceHelper.getPiaproUrlWithTimestamp(pv)!
							: pv.pvId;

					await player.loadVideo(pvId);
					await player.play();
				} catch (error) {
					VdbPlayerConsole.error(
						'Failed to load PV',
						JSON.parse(JSON.stringify(pv)),
						error,
					);
				}
			},
			[pv],
		);

		return (
			<EmbedPV
				pv={pv}
				width="100%"
				height="100%"
				options={options}
				onPlayerChange={handlePlayerChange}
			/>
		);
	},
);

const MiniPlayer = observer(
	(): React.ReactElement => {
		const { vdbPlayer, playQueue } = useVdbPlayer();

		return (
			<div
				css={{
					...(vdbPlayer.playerBounds === undefined
						? {
								position: 'fixed',
								right: 0,
								bottom: vdbPlayer.bottomBarEnabled ? bottomBarHeight : 0,
								width: miniPlayerWidth,
								height: miniPlayerHeight,
								zIndex: 3939,
						  }
						: {
								position: 'absolute',
								left: vdbPlayer.playerBounds.x,
								top: vdbPlayer.playerBounds.y,
								width: vdbPlayer.playerBounds.width,
								height: vdbPlayer.playerBounds.height,
						  }),
					backgroundColor: 'rgb(39, 39, 39)',
					display: 'flex',
					flexDirection: 'column',
				}}
			>
				<div
					css={{
						flexGrow: 1,
						backgroundColor: 'black',
					}}
				>
					{playQueue.currentItem && (
						<EmbedPVWrapper pv={playQueue.currentItem.pv} />
					)}
				</div>
			</div>
		);
	},
);

const SeekBar = observer(
	(): React.ReactElement => {
		const diva = useNostalgicDiva();
		const { vdbPlayer } = useVdbPlayer();

		const ref = React.useRef<HTMLDivElement>(undefined!);

		const handleClick = React.useCallback(
			async (e: React.MouseEvent): Promise<void> => {
				const duration = await diva.getDuration();
				if (duration !== undefined) {
					const rect = ref.current.getBoundingClientRect();
					const fraction = (e.clientX - rect.left) / rect.width;
					await diva.setCurrentTime(duration * fraction);
				}
			},
			[diva],
		);

		return (
			<div
				css={{
					display: 'flex',
					backgroundColor: 'rgb(157, 157, 157)',
					width: '100%',
					height: seekBarHeight,
					cursor: 'pointer',
					userSelect: 'none',
				}}
				onClick={handleClick}
				ref={ref}
			>
				<div
					css={{
						backgroundColor: 'rgb(76, 194, 255)',
					}}
					style={{
						width: `${vdbPlayer.percent * 100}%`,
					}}
				/>
			</div>
		);
	},
);

const BottomBar = React.memo(
	(): React.ReactElement => {
		// Code from: https://github.com/elastic/eui/blob/e07ee756120607b338d522ee8bcedd4228d02673/src/components/bottom_bar/bottom_bar.tsx#L137.
		React.useEffect(() => {
			document.body.style.paddingBottom = `${bottomBarHeight}px`;

			return (): void => {
				document.body.style.paddingBottom = '';
			};
		});

		return (
			<div
				css={{
					position: 'fixed',
					left: 0,
					right: 0,
					bottom: 0,
					zIndex: 3939,
					backgroundColor: 'rgb(39, 39, 39)',
					display: 'flex',
					flexDirection: 'column',
				}}
			>
				<div css={{ display: 'flex', flexDirection: 'column' }}>
					<SeekBar />

					<Container fluid>
						<PlayerControls />
					</Container>
				</div>
			</div>
		);
	},
);

export const VdbPlayer = observer(
	(): React.ReactElement => {
		VdbPlayerConsole.debug('VdbPlayer');

		const diva = useNostalgicDiva();
		const { vdbPlayer, playQueue } = useVdbPlayer();

		useLocalStorageStateStore('PlayQueueStore', playQueue);

		React.useEffect(() => {
			// Returns the disposer.
			return reaction(
				() => playQueue.currentItem,
				async (selectedItem, previousItem) => {
					if (!selectedItem || !previousItem) return;

					// If the current PV is the same as the previous one, then seek it to 0 and play it again.
					if (selectedItem.pv.id === previousItem.pv.id) {
						await diva.setCurrentTime(0);
					}
				},
			);
		}, [diva, playQueue]);

		return (
			<>
				{!playQueue.isEmpty && <MiniPlayer />}

				{vdbPlayer.bottomBarEnabled && <BottomBar />}
			</>
		);
	},
);
