import Button from '@/Bootstrap/Button';
import ButtonGroup from '@/Bootstrap/ButtonGroup';
import Container from '@/Bootstrap/Container';
import Dropdown from '@/Bootstrap/Dropdown';
import { PVServiceIcon } from '@/Components/Shared/Partials/Shared/PVServiceIcon';
import { EmbedPV } from '@/Components/VdbPlayer/EmbedPV';
import {
	SongleIcon,
	SongleWidget,
	songleWidgetHeight,
} from '@/Components/VdbPlayer/SongleWidget';
import { VdbPlayerConsole } from '@/Components/VdbPlayer/VdbPlayerConsole';
import {
	usePlayQueue,
	useVdbPlayer,
} from '@/Components/VdbPlayer/VdbPlayerContext';
import { VdbPlayerEntryLink } from '@/Components/VdbPlayer/VdbPlayerEntryLink';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { PlayQueueEntryContract } from '@/Stores/VdbPlayer/PlayQueueRepository';
import { PlayQueueItem, RepeatMode } from '@/Stores/VdbPlayer/PlayQueueStore';
import { css } from '@emotion/react';
import { MoreHorizontal20Filled } from '@fluentui/react-icons';
import {
	PlayerOptions,
	TimeEvent,
	useNostalgicDiva,
} from '@vocadb/nostalgic-diva';
import { useLocalStorageStateStore } from '@vocadb/route-sphere';
import classNames from 'classnames';
import { reaction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
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
		const vdbPlayer = useVdbPlayer();
		const playQueue = usePlayQueue();

		const handlePrevious = React.useCallback(async () => {
			if (playQueue.hasPreviousItem) {
				const currentTime = await diva.getCurrentTime();

				if (currentTime === undefined || currentTime < 5) {
					playQueue.previous();
				} else {
					await diva.setCurrentTime(0);
				}
			} else {
				await diva.setCurrentTime(0);
			}
		}, [diva, playQueue]);

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
					title={`Coming soon!` /* TODO: Remove. */}
					onClick={playQueue.toggleShuffle}
					disabled={true /* TODO: Remove. */}
					className={classNames('hidden-phone', playQueue.shuffle && 'active')}
				>
					<i className="icon-random icon-white" />
				</Button>
				<Button
					variant="inverse"
					title="Previous" /* LOC */
					onClick={handlePrevious}
					disabled={playQueue.isEmpty}
				>
					<i className="icon-step-backward icon-white" />
				</Button>
				{vdbPlayer.playing ? (
					<Button
						variant="inverse"
						title="Pause" /* LOC */
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
						}`} /* LOC */
						onClick={handlePlay}
						disabled={!vdbPlayer.canAutoplay}
					>
						<i className="icon-play icon-white" />
					</Button>
				)}
				<Button
					variant="inverse"
					title="Next" /* LOC */
					onClick={playQueue.next}
					disabled={!playQueue.hasNextItem}
				>
					<i className="icon-step-forward icon-white" />
				</Button>
				<Button
					variant="inverse"
					title={`Repeat: ${playQueue.repeat}` /* LOC */}
					onClick={playQueue.toggleRepeat}
					className="hidden-phone"
				>
					<i
						className={classNames(repeatIcons[playQueue.repeat], 'icon-white')}
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
				<VdbPlayerEntryLink entry={entry} css={{ marginRight: 8 }}>
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
				</VdbPlayerEntryLink>

				<div
					css={{
						flexGrow: 1,
						display: 'flex',
						minWidth: 0,
						flexDirection: 'column',
					}}
				>
					<VdbPlayerEntryLink
						entry={entry}
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
					</VdbPlayerEntryLink>
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
		const { t } = useTranslation(['ViewRes.Search']);

		const diva = useNostalgicDiva();
		const vdbPlayer = useVdbPlayer();
		const playQueue = usePlayQueue();

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

		const handleClickRemoveFromPlayQueue = React.useCallback(async () => {
			const { currentItem } = playQueue;
			if (currentItem !== undefined) {
				playQueue.removeFromPlayQueue([currentItem]);
			}
		}, [playQueue]);

		return (
			<>
				{playQueue.currentItem && (
					<PVServiceDropdown item={playQueue.currentItem} />
				)}{' '}
				<ButtonGroup>
					<Button
						variant="inverse"
						title="Songle"
						onClick={vdbPlayer.toggleSongleWidget}
						className={classNames(vdbPlayer.songleWidgetEnabled && 'active')}
					>
						<SongleIcon />
					</Button>
				</ButtonGroup>{' '}
				<ButtonGroup className="hidden-phone">
					<Button
						variant="inverse"
						as={Link}
						to="/playlist"
						title={t('ViewRes.Search:Index.Playlist')}
					>
						<i className="icon-list icon-white" />
					</Button>
				</ButtonGroup>{' '}
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
						<Dropdown.Item as={Link} to="/playlist" className="visible-phone">
							Show play queue{/* LOC */}
						</Dropdown.Item>
						<Dropdown.Item
							onClick={handleClickSkipBack10Seconds}
							disabled={!vdbPlayer.canAutoplay}
						>
							Skip back 10 seconds{/* LOC */}
						</Dropdown.Item>
						<Dropdown.Item
							onClick={handleClickSkipForward30Seconds}
							disabled={!vdbPlayer.canAutoplay}
						>
							Skip forward 30 seconds{/* LOC */}
						</Dropdown.Item>
						<Dropdown.Item
							onClick={playQueue.toggleShuffle}
							disabled={true /* TODO: Remove. */}
							className="visible-phone"
							title="Coming soon!" /* TODO: Remove. */
						>
							{`Shuffle: ${playQueue.shuffle ? 'On' : 'Off'}` /* LOC */}
						</Dropdown.Item>
						<Dropdown.Item
							onClick={playQueue.toggleRepeat}
							className="visible-phone"
						>
							{`Repeat: ${playQueue.repeat}` /* LOC */}
						</Dropdown.Item>
						<Dropdown.Divider />
						<Dropdown.Item
							onClick={handleClickRemoveFromPlayQueue}
							disabled={playQueue.isEmpty}
						>
							Remove from play queue{/* LOC */}
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
						{playQueue.currentItem && (
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
		const vdbPlayer = useVdbPlayer();
		const playQueue = usePlayQueue();

		const handleError = React.useCallback((event: any) => {
			VdbPlayerConsole.error('error', event);
		}, []);

		const handleLoaded = React.useCallback(() => {
			if (!playQueue.interacted) {
				return;
			}

			diva.play();
		}, [playQueue, diva]);

		const handlePlay = React.useCallback(() => vdbPlayer.setPlaying(true), [
			vdbPlayer,
		]);

		const handlePause = React.useCallback(() => vdbPlayer.setPlaying(false), [
			vdbPlayer,
		]);

		const handleEnded = React.useCallback(async () => {
			VdbPlayerConsole.debug(
				`Playback ended (repeat mode: ${playQueue.repeat})`,
			);

			switch (playQueue.repeat) {
				case RepeatMode.One:
					await diva.setCurrentTime(0);
					break;

				case RepeatMode.Off:
				case RepeatMode.All:
					if (playQueue.isLastItem && !playQueue.hasMoreItems) {
						switch (playQueue.repeat) {
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
				onLoaded: handleLoaded,
				onPlay: handlePlay,
				onPause: handlePause,
				onEnded: handleEnded,
				onTimeUpdate: handleTimeUpdate,
			}),
			[
				handleError,
				handleLoaded,
				handlePlay,
				handlePause,
				handleEnded,
				handleTimeUpdate,
			],
		);

		return <EmbedPV pv={pv} width="100%" height="100%" options={options} />;
	},
);

const MiniPlayer = observer(
	(): React.ReactElement => {
		const vdbPlayer = useVdbPlayer();
		const playQueue = usePlayQueue();

		return (
			<div
				css={{
					...(vdbPlayer.playerBounds === undefined
						? {
								position: 'fixed',
								right: 0,
								bottom: vdbPlayer.bottomBarEnabled
									? (vdbPlayer.songleWidgetEnabled ? songleWidgetHeight : 0) +
									  bottomBarHeight
									: 0,
								width: miniPlayerWidth,
								height: miniPlayerHeight,
								zIndex: 998,
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
		const vdbPlayer = useVdbPlayer();

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

const BottomBar = observer(
	(): React.ReactElement => {
		const vdbPlayer = useVdbPlayer();

		// Code from: https://github.com/elastic/eui/blob/e07ee756120607b338d522ee8bcedd4228d02673/src/components/bottom_bar/bottom_bar.tsx#L137.
		React.useEffect(() => {
			document.body.style.paddingBottom = `${
				(vdbPlayer.songleWidgetEnabled ? songleWidgetHeight : 0) +
				bottomBarHeight
			}px`;

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
					zIndex: 998,
					backgroundColor: 'rgb(39, 39, 39)',
					display: 'flex',
					flexDirection: 'column',
				}}
			>
				<div css={{ display: 'flex', flexDirection: 'column' }}>
					{vdbPlayer.songleWidgetEnabled && <SongleWidget />}

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
		const vdbPlayer = useVdbPlayer();
		const playQueue = usePlayQueue();

		useLocalStorageStateStore('VdbPlayerStore', vdbPlayer);
		useLocalStorageStateStore('PlayQueueStore', playQueue);
		useLocalStorageStateStore('SkipListStore', playQueue.skipList);

		React.useEffect(() => {
			// Returns the disposer.
			return reaction(
				() => playQueue.currentItem,
				async (selectedItem, previousItem) => {
					if (!selectedItem || !previousItem) return;

					if (selectedItem.pvId === previousItem.pvId) {
						// If the current PV is the same as the previous one, then seek it to 0 and play it again.
						await diva.setCurrentTime(0);
					}
				},
			);
		}, [diva, playQueue]);

		return vdbPlayer.bottomBarEnabled ? (
			<>
				{!playQueue.isEmpty && <MiniPlayer />}
				<BottomBar />
			</>
		) : (
			<></>
		);
	},
);
