import Button from '@/Bootstrap/Button';
import ButtonGroup from '@/Bootstrap/ButtonGroup';
import Container from '@/Bootstrap/Container';
import Dropdown from '@/Bootstrap/Dropdown';
import { EmbedPV } from '@/Components/VdbPlayer/EmbedPV';
import { VdbPlayerConsole } from '@/Components/VdbPlayer/VdbPlayerConsole';
import { useVdbPlayer } from '@/Components/VdbPlayer/VdbPlayerContext';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { RepeatMode } from '@/Stores/VdbPlayer/VdbPlayerStore';
import { css } from '@emotion/react';
import { PVPlayer, TimeEvent } from '@vocadb/nostalgic-diva';
import classNames from 'classnames';
import { reaction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { Link } from 'react-router-dom';

const repeatIcons: Record<RepeatMode, string> = {
	[RepeatMode.Off]: 'icon-ban-circle',
	[RepeatMode.All]: 'icon-refresh',
	[RepeatMode.One]: 'icon-repeat',
};

const VdbPlayerLeftControls = observer(
	(): React.ReactElement => {
		const { vdbPlayer, playQueue, playerRef } = useVdbPlayer();

		const handlePause = React.useCallback(async () => {
			const player = playerRef.current;

			if (!player) return;

			await player.pause();
		}, [playerRef]);

		const handlePlay = React.useCallback(async () => {
			const player = playerRef.current;

			if (!player) return;

			await player.play();
		}, [playerRef]);

		React.useEffect(() => {
			// Returns the disposer.
			return reaction(
				() => playQueue.currentItem,
				async (selectedItem, previousItem) => {
					// If the current PV is the same as the previous one, then seek it to 0 and play it again.
					if (selectedItem?.pv.id === previousItem?.pv.id) {
						const player = playerRef.current;

						if (!player) return;

						await player.setCurrentTime(0);
						await player.play();
					}
				},
			);
		}, [playQueue, playerRef]);

		return (
			<ButtonGroup>
				<Button
					variant="inverse"
					title={
						`Shuffle: ${vdbPlayer.shuffle ? 'On' : 'Off'}${
							vdbPlayer.canAutoplay
								? ''
								: ' (Unavailable for this video service)'
						}` /* TODO: localize */
					}
					onClick={vdbPlayer.toggleShuffle}
					disabled={!vdbPlayer.canAutoplay}
					className={classNames(vdbPlayer.shuffle && 'active')}
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
				>
					<i
						className={classNames(repeatIcons[vdbPlayer.repeat], 'icon-white')}
					/>
				</Button>
			</ButtonGroup>
		);
	},
);

const VdbPlayerEntryInfo = observer(
	(): React.ReactElement => {
		const { playQueue } = useVdbPlayer();

		return (
			<div css={{ display: 'flex', alignItems: 'center' }}>
				{playQueue.currentItem && (
					<Link
						to={EntryUrlMapper.details_entry(playQueue.currentItem.entry)}
						css={{ marginRight: 8 }}
					>
						<div
							css={{
								width: 64,
								height: 36,
								backgroundColor: 'rgb(28, 28, 28)',
								backgroundImage: `url(${playQueue.currentItem.entry.mainPicture?.urlThumb})`,
								backgroundSize: 'cover',
								backgroundPosition: 'center',
							}}
						/>
					</Link>
				)}

				<div
					css={{
						flexGrow: 1,
						display: 'flex',
						minWidth: 0,
						flexDirection: 'column',
					}}
				>
					{playQueue.currentItem && (
						<>
							<Link
								to={EntryUrlMapper.details_entry(playQueue.currentItem.entry)}
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
								{playQueue.currentItem.entry.name}
							</Link>
							<div css={{ display: 'flex' }}>
								<span
									css={{
										color: '#999999',
										overflow: 'hidden',
										textOverflow: 'ellipsis',
										whiteSpace: 'nowrap',
									}}
								>
									{playQueue.currentItem.entry.artistString}
								</span>
							</div>
						</>
					)}
				</div>
			</div>
		);
	},
);

const VdbPlayerRightControls = observer(
	(): React.ReactElement => {
		const { vdbPlayer, playQueue, playerRef } = useVdbPlayer();

		const handleClickSkipBack10Seconds = React.useCallback(async () => {
			const player = playerRef.current;

			if (!player) return;

			const currentTime = await player.getCurrentTime();

			if (currentTime === undefined) return;

			await player.setCurrentTime(currentTime - 10);
		}, [playerRef]);

		const handleClickSkipForward30Seconds = React.useCallback(async () => {
			const player = playerRef.current;

			if (!player) return;

			const currentTime = await player.getCurrentTime();

			if (currentTime === undefined) return;

			await player.setCurrentTime(currentTime + 30);
		}, [playerRef]);

		return (
			<Dropdown as={ButtonGroup} drop="up" css={{ marginLeft: 8 }}>
				<Dropdown.Toggle variant="inverse">⋯</Dropdown.Toggle>
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
						disabled={!vdbPlayer.canAutoplay}
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
					>
						{`Repeat: ${vdbPlayer.repeat}` /* TODO: localize */}
					</Dropdown.Item>
					<Dropdown.Item onClick={playQueue.clear} disabled={playQueue.isEmpty}>
						Clear play queue{/* TODO: localize */}
					</Dropdown.Item>
				</Dropdown.Menu>
			</Dropdown>
		);
	},
);

const controlsHeight = 48;

const VdbPlayerControls = observer(
	(): React.ReactElement => {
		return (
			<div
				css={{
					display: 'flex',
					height: controlsHeight,
					alignItems: 'center',
				}}
			>
				<VdbPlayerLeftControls />

				<div css={{ flexGrow: 1 }}></div>

				<div css={{ width: 220 }}>
					<VdbPlayerEntryInfo />
				</div>

				<VdbPlayerRightControls />
			</div>
		);
	},
);

interface PVPlayerProps {
	pv: PVContract;
}

const EmbedPVWrapper = observer(
	({ pv }: PVPlayerProps): React.ReactElement => {
		const { vdbPlayer, playQueue, playerRef } = useVdbPlayer();

		const handleError = React.useCallback((e: any) => {
			VdbPlayerConsole.error('error', e);
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

			const player = playerRef.current;

			if (!player) return;

			switch (vdbPlayer.repeat) {
				case RepeatMode.One:
					await player.setCurrentTime(0);
					await player.play();
					break;

				case RepeatMode.Off:
				case RepeatMode.All:
					if (playQueue.isLastItem) {
						switch (vdbPlayer.repeat) {
							case RepeatMode.Off:
								vdbPlayer.setPlaying(false);
								break;

							case RepeatMode.All:
								if (playQueue.hasMultipleItems) {
									// HACK: Prevent vdbPlayer.next from being called in the same context.
									// EmbedFile, EmbedNiconico, EmbedSoundCloud, EmbedYouTube and etc. must be rendered only after the `pv` prop has changed.
									// Otherwise, the same PV may be played twice when switching between video services (e.g. Niconico => YouTube).
									setTimeout(() => {
										playQueue.goToFirst();
									});
								} else {
									await player.setCurrentTime(0);
									await player.play();
								}
								break;
						}
					} else {
						// HACK: Prevent vdbPlayer.next from being called in the same context.
						// EmbedFile, EmbedNiconico, EmbedSoundCloud, EmbedYouTube and etc. must be rendered only after the `pv` prop has changed.
						// Otherwise, the same PV may be played twice when switching between video services (e.g. Niconico => YouTube).
						setTimeout(() => {
							playQueue.next();
						});
					}
					break;
			}
		}, [vdbPlayer, playQueue, playerRef]);

		const handleTimeUpdate = React.useCallback(
			({ percent }: TimeEvent) => {
				if (percent !== undefined) vdbPlayer.setPercent(percent);
			},
			[vdbPlayer],
		);

		const options = React.useMemo(
			() => ({
				onError: handleError,
				onPlay: handlePlay,
				onPause: handlePause,
				onEnded: handleEnded,
				onTimeUpdate: handleTimeUpdate,
			}),
			[handleError, handlePlay, handlePause, handleEnded, handleTimeUpdate],
		);

		const handlePlayerChange = React.useCallback(
			async (player?: PVPlayer) => {
				try {
					if (!player) return;

					await player.loadVideo(pv.pvId);
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
				enableApi={true}
				playerRef={playerRef}
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
								bottom: vdbPlayer.bottomBarVisible ? 50 : 0,
								width: 16 * 25,
								height: 9 * 25,
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

const seekBarHeight = 8;

const SeekBar = observer(
	(): React.ReactElement => {
		const { vdbPlayer, playerRef } = useVdbPlayer();

		const ref = React.useRef<HTMLDivElement>(undefined!);

		const handleClick = React.useCallback(
			async (e: React.MouseEvent): Promise<void> => {
				const player = playerRef.current;

				if (!player) return;

				const rect = ref.current.getBoundingClientRect();
				const fraction = (e.clientX - rect.left) / rect.width;

				const duration = await player.getDuration();

				if (duration === undefined) return;

				await player.setCurrentTime(duration * fraction);
				await player.play();
			},
			[playerRef],
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

export const bottomBarHeight = seekBarHeight + controlsHeight;

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

					<Container>
						<VdbPlayerControls />
					</Container>
				</div>
			</div>
		);
	},
);

export const VdbPlayer = observer(
	(): React.ReactElement => {
		VdbPlayerConsole.debug('VdbPlayer');

		const { vdbPlayer, playQueue } = useVdbPlayer();

		return (
			<>
				{!playQueue.isEmpty && <MiniPlayer />}

				{vdbPlayer.bottomBarVisible && <BottomBar />}
			</>
		);
	},
);
