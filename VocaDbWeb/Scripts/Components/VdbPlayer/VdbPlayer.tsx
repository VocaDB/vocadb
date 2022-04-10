import Button from '@Bootstrap/Button';
import ButtonGroup from '@Bootstrap/ButtonGroup';
import Container from '@Bootstrap/Container';
import PVContract from '@DataContracts/PVs/PVContract';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import { RepeatMode } from '@Stores/VdbPlayer/VdbPlayerStore';
import { css } from '@emotion/react';
import classNames from 'classnames';
import { reaction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { Link } from 'react-router-dom';

import EmbedPV from './EmbedPV';
import IPVPlayer from './IPVPlayer';
import VdbPlayerConsole from './VdbPlayerConsole';
import { useVdbPlayer } from './VdbPlayerContext';

const repeatIcons: Record<RepeatMode, string> = {
	[RepeatMode.Off]: 'icon-ban-circle',
	[RepeatMode.All]: 'icon-refresh',
	[RepeatMode.One]: 'icon-repeat',
};

interface VdbPlayerLeftControlsProps {
	playerRef: React.MutableRefObject<IPVPlayer>;
}

const VdbPlayerLeftControls = observer(
	({ playerRef }: VdbPlayerLeftControlsProps): React.ReactElement => {
		const vdbPlayer = useVdbPlayer();

		const handlePause = React.useCallback(() => playerRef.current.pause(), [
			playerRef,
		]);

		const handlePlay = React.useCallback(() => playerRef.current.play(), [
			playerRef,
		]);

		React.useEffect(() => {
			// Returns the disposer.
			return reaction(
				() => vdbPlayer.playQueueStore.selectedEntry,
				(selectedEntry, previousEntry) => {
					// If the current PV is the same as the previous one, then seek it to 0 and play it again.
					if (selectedEntry?.pv.id === previousEntry?.pv.id) {
						playerRef.current.seekTo(0);
						playerRef.current.play();
					}
				},
			);
		}, [vdbPlayer, playerRef]);

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
					onClick={vdbPlayer.previous}
					disabled={!vdbPlayer.hasPreviousEntry}
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
					onClick={vdbPlayer.next}
					disabled={!vdbPlayer.hasNextEntry}
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
		const vdbPlayer = useVdbPlayer();

		const handleEntryLinkClick = React.useCallback(() => {
			vdbPlayer.collapse();
		}, [vdbPlayer]);

		return (
			<div css={{ display: 'flex', alignItems: 'center' }}>
				{vdbPlayer.selectedEntry && (
					<Link
						to={EntryUrlMapper.details_entry(vdbPlayer.selectedEntry.entry)}
						onClick={handleEntryLinkClick}
						css={{ marginRight: 8 }}
					>
						<div
							css={{
								width: 64,
								height: 36,
								backgroundColor: 'rgb(28, 28, 28)',
								backgroundImage: `url(${vdbPlayer.selectedEntry.entry.mainPicture?.urlThumb})`,
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
					{vdbPlayer.selectedEntry && (
						<>
							<Link
								to={EntryUrlMapper.details_entry(vdbPlayer.selectedEntry.entry)}
								onClick={handleEntryLinkClick}
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
								{vdbPlayer.selectedEntry.entry.name}
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
									{vdbPlayer.selectedEntry.entry.artistString}
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
		const vdbPlayer = useVdbPlayer();

		return (
			<ButtonGroup css={{ marginLeft: 8 }}>
				{vdbPlayer.expanded ? (
					<Button
						variant="inverse"
						title="Collapse" /* TODO: localize */
						onClick={vdbPlayer.collapse}
					>
						<i className="icon-resize-small icon-white" />
					</Button>
				) : (
					<Button
						variant="inverse"
						title="Expand" /* TODO: localize */
						onClick={vdbPlayer.expand}
						disabled={!vdbPlayer.selectedEntry}
					>
						<i className="icon-resize-full icon-white" />
					</Button>
				)}
			</ButtonGroup>
		);
	},
);

interface VdbPlayerControlsProps {
	playerRef: React.MutableRefObject<IPVPlayer>;
}

const VdbPlayerControls = observer(
	({ playerRef }: VdbPlayerControlsProps): React.ReactElement => {
		return (
			<div css={{ display: 'flex', height: 50, alignItems: 'center' }}>
				<VdbPlayerLeftControls playerRef={playerRef} />

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
	playerRef: React.MutableRefObject<IPVPlayer>;
	pv: PVContract;
}

const PVPlayer = observer(
	({ playerRef, pv }: PVPlayerProps): React.ReactElement => {
		const vdbPlayer = useVdbPlayer();

		React.useEffect(() => {
			const player = playerRef.current;

			if (!player) return;

			player
				.load(pv)
				.then(player.play)
				.catch((e) => {
					VdbPlayerConsole.error(
						'Failed to load PV',
						JSON.parse(JSON.stringify(pv)),
						e,
					);
				});
		}, [playerRef, pv]);

		const handleError = React.useCallback((e: any) => {
			VdbPlayerConsole.error('error', e);
		}, []);

		const handlePlay = React.useCallback(() => vdbPlayer.setPlaying(true), [
			vdbPlayer,
		]);

		const handlePause = React.useCallback(() => vdbPlayer.setPlaying(false), [
			vdbPlayer,
		]);

		const handleEnded = React.useCallback(() => {
			VdbPlayerConsole.debug(
				`Playback ended (repeat mode: ${vdbPlayer.repeat})`,
			);

			const player = playerRef.current;

			if (!player) return;

			switch (vdbPlayer.repeat) {
				case RepeatMode.One:
					player.seekTo(0);
					player.play();
					break;

				case RepeatMode.Off:
				case RepeatMode.All:
					if (vdbPlayer.playQueueStore.isLastEntry) {
						switch (vdbPlayer.repeat) {
							case RepeatMode.Off:
								vdbPlayer.setPlaying(false);
								break;

							case RepeatMode.All:
								if (vdbPlayer.playQueueStore.hasMultipleEntries) {
									// HACK: Prevent vdbPlayer.next from being called in the same context.
									// EmbedFile, EmbedNiconico, EmbedSoundCloud, EmbedYouTube and etc. must be rendered only after the `pv` prop has changed.
									// Otherwise, the same PV may be played twice when switching between video services (e.g. Niconico => YouTube).
									setTimeout(() => {
										vdbPlayer.playQueueStore.goToFirst();
									});
								} else {
									player.seekTo(0);
									player.play();
								}
								break;
						}
					} else {
						// HACK: Prevent vdbPlayer.next from being called in the same context.
						// EmbedFile, EmbedNiconico, EmbedSoundCloud, EmbedYouTube and etc. must be rendered only after the `pv` prop has changed.
						// Otherwise, the same PV may be played twice when switching between video services (e.g. Niconico => YouTube).
						setTimeout(() => {
							vdbPlayer.next();
						});
					}
					break;
			}
		}, [vdbPlayer, playerRef]);

		const options = React.useMemo(
			() => ({
				onError: handleError,
				onPlay: handlePlay,
				onPause: handlePause,
				onEnded: handleEnded,
			}),
			[handleError, handlePlay, handlePause, handleEnded],
		);

		return (
			<EmbedPV
				pv={pv}
				width="100%"
				height="100%"
				enableApi={true}
				playerRef={playerRef}
				options={options}
			/>
		);
	},
);

const VdbPlayer = observer(
	(): React.ReactElement => {
		VdbPlayerConsole.debug('VdbPlayer');

		const vdbPlayer = useVdbPlayer();

		const playerRef = React.useRef<IPVPlayer>(undefined!);

		return (
			<div
				css={{
					position: 'fixed',
					left: 0,
					right: 0,
					top: vdbPlayer.expanded ? 0 : undefined,
					bottom: 0,
					zIndex: 3939,
					backgroundColor: 'rgb(39, 39, 39)',
					display: 'flex',
					flexDirection: 'column',
				}}
			>
				<div
					css={{
						display: vdbPlayer.expanded ? undefined : 'none',
						flexGrow: 1,
						backgroundColor: 'black',
					}}
				>
					{vdbPlayer.selectedEntry && (
						<PVPlayer playerRef={playerRef} pv={vdbPlayer.selectedEntry.pv} />
					)}
				</div>

				<div>
					<Container>
						<VdbPlayerControls playerRef={playerRef} />
					</Container>
				</div>
			</div>
		);
	},
);

export default VdbPlayer;
