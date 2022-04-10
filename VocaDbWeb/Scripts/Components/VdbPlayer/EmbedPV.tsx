import PVContract from '@DataContracts/PVs/PVContract';
import PVService from '@Models/PVs/PVService';
import { RepeatMode } from '@Stores/VdbPlayer/VdbPlayerStore';
import _ from 'lodash';
import React from 'react';

// TODO: Implement lazy loading.
import EmbedBili from '../Shared/Partials/PV/EmbedBili';
import EmbedPiapro from '../Shared/Partials/PV/EmbedPiapro';
import EmbedFile from './EmbedFile';
import EmbedNiconico from './EmbedNiconico';
import EmbedSoundCloud from './EmbedSoundCloud';
import EmbedYouTube from './EmbedYouTube';
import IPVPlayer from './IPVPlayer';
import VdbPlayerConsole from './VdbPlayerConsole';
import { useVdbPlayer } from './VdbPlayerContext';

// Code from: https://github.com/dotnet/runtime/blob/09c1a1f7b0c477890b04912d8dd4f742f80faffc/src/libraries/System.Private.CoreLib/src/System/IO/Path.cs#L152
// TODO: Test.
const getExtension = (path?: string): string | undefined => {
	if (!path) return undefined;

	const length = path.length;

	for (let i = length - 1; i >= 0; i--) {
		const ch = path[i];
		if (ch === '.') {
			if (i !== length - 1) return path.slice(i, length);
			else return '';
		}
		if (ch === '/') break;
	}

	return '';
};

const isImage = (filename?: string): boolean => {
	const imageExtensions = ['.jpg', '.png'];
	const ext = getExtension(filename);
	return _.includes(imageExtensions, ext);
};

const isAudio = (filename?: string): boolean => !isImage(filename);

interface EmbedPVProps {
	pv: PVContract;
	width?: number | string;
	height?: number | string;
	autoplay?: boolean;
	enableApi?: boolean;
	id?: string;
	playerRef: React.MutableRefObject<IPVPlayer | undefined>;
}

const EmbedPV = React.memo(
	({
		pv,
		width = 560,
		height = 315,
		autoplay = false,
		enableApi = false,
		id,
		playerRef,
	}: EmbedPVProps): React.ReactElement => {
		VdbPlayerConsole.debug('EmbedPV');

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
				case RepeatMode.Off:
					if (vdbPlayer.playQueueStore.isEmpty) {
						vdbPlayer.setPlaying(false);
					} else {
						vdbPlayer.next();
					}
					break;

				case RepeatMode.One:
					player.seekTo(0);
					player.play();
					break;

				case RepeatMode.All:
					// TODO: Implement.
					break;
			}
		}, [vdbPlayer, playerRef]);

		const playerOptions = React.useMemo(
			() => ({
				onError: handleError,
				onPlay: handlePlay,
				onPause: handlePause,
				onEnded: handleEnded,
			}),
			[handleError, handlePlay, handlePause, handleEnded],
		);

		switch (PVService[pv.service as keyof typeof PVService]) {
			case PVService.Bandcamp:
				// TODO: Remove.
				playerRef.current = undefined;

				return (
					// eslint-disable-next-line jsx-a11y/iframe-has-title
					<iframe
						style={{ border: 0, width: '100%', height: '120px' }}
						src={`https://bandcamp.com/EmbeddedPlayer/size=large/bgcol=ffffff/linkcol=0687f5/tracklist=false/artwork=small/track=${pv.pvId}/transparent=true/`}
						seamless
						key={pv.pvId}
					/>
				);

			case PVService.Bilibili:
				// TODO: Remove.
				playerRef.current = undefined;

				return <EmbedBili pv={pv} width={width} height={height} />;

			case PVService.File:
			case PVService.LocalFile:
				return isAudio(pv.url) ? (
					<EmbedFile playerRef={playerRef} {...playerOptions} />
				) : (
					<div css={{ width: width, height: height }}>
						<a href={pv.url}>
							<img
								style={{ maxWidth: '100%', maxHeight: '100%' }}
								src={pv.thumbUrl}
								alt={pv.name}
							/>
						</a>
					</div>
				);

			case PVService.NicoNicoDouga:
				return <EmbedNiconico playerRef={playerRef} {...playerOptions} />;

			case PVService.Piapro:
				// TODO: Remove.
				playerRef.current = undefined;

				return <EmbedPiapro pv={pv} width={width} height={height} />;

			case PVService.SoundCloud:
				return <EmbedSoundCloud playerRef={playerRef} {...playerOptions} />;

			case PVService.Youtube:
				return <EmbedYouTube playerRef={playerRef} {...playerOptions} />;

			case PVService.Vimeo:
				// TODO: Remove.
				playerRef.current = undefined;

				return (
					// eslint-disable-next-line jsx-a11y/iframe-has-title
					<iframe
						src={`https://player.vimeo.com/video/${pv.pvId}`}
						width={width}
						height={height}
						frameBorder="0"
						// @ts-ignore
						webkitAllowFullScreen
						mozallowfullscreen
						allowFullScreen
						key={pv.pvId}
					/>
				);

			case PVService.Creofuga:
				// TODO: Remove.
				playerRef.current = undefined;

				return (
					// eslint-disable-next-line jsx-a11y/iframe-has-title
					<iframe
						width={width}
						height={typeof height === 'number' ? Math.min(height, 120) : height}
						scrolling="no"
						frameBorder="no"
						src={`https://creofuga.net/audios/player?color=black&id=${pv.pvId}`}
						key={pv.pvId}
					/>
				);

			default:
				// TODO: Remove.
				playerRef.current = undefined;

				return <></>;
		}
	},
);

export default EmbedPV;
