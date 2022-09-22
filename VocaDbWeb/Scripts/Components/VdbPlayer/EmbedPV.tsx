import { EmbedBili } from '@/Components/Shared/Partials/PV/EmbedBili';
import { EmbedPiapro } from '@/Components/Shared/Partials/PV/EmbedPiapro';
import { VdbPlayerConsole } from '@/Components/VdbPlayer/VdbPlayerConsole';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { VideoServiceHelper } from '@/Helpers/VideoServiceHelper';
import { PVService } from '@/Models/PVs/PVService';
import { NostalgicDiva } from '@vocadb/nostalgic-diva';
import { PlayerApi, PlayerOptions, PlayerType } from '@vocadb/nostalgic-diva';
import _ from 'lodash';
import React from 'react';

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

const playerTypes: Record<
	Exclude<
		PVService,
		PVService.Bilibili | PVService.Creofuga | PVService.Bandcamp
	>,
	PlayerType
> = {
	[PVService.File]: 'Audio',
	[PVService.LocalFile]: 'Audio',
	[PVService.NicoNicoDouga]: 'Niconico',
	[PVService.Piapro]: 'Audio',
	[PVService.SoundCloud]: 'SoundCloud',
	[PVService.Vimeo]: 'Vimeo',
	[PVService.Youtube]: 'YouTube',
};

interface EmbedPVProps {
	pv: PVContract;
	width?: number | string;
	height?: number | string;
	options: PlayerOptions;
	onPlayerChange?: (player?: PlayerApi) => void;
}

export const EmbedPV = React.memo(
	({
		pv,
		width = 560,
		height = 315,
		options,
		onPlayerChange,
	}: EmbedPVProps): React.ReactElement => {
		VdbPlayerConsole.debug('EmbedPV');

		const service = pv.service;

		switch (service) {
			case PVService.File:
			case PVService.LocalFile:
				if (!isAudio(pv.url)) {
					return (
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
				}

				break;

			case PVService.Piapro:
				if (VideoServiceHelper.getPiaproTimestamp(pv) === undefined) {
					return <EmbedPiapro pv={pv} width={width} height={height} />;
				}

				break;
		}

		switch (service) {
			case PVService.File:
			case PVService.LocalFile:
			case PVService.NicoNicoDouga:
			case PVService.Piapro:
			case PVService.SoundCloud:
			case PVService.Vimeo:
			case PVService.Youtube:
				return (
					<NostalgicDiva
						type={playerTypes[service]}
						options={options}
						onPlayerChange={onPlayerChange}
					/>
				);

			case PVService.Bandcamp:
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
				return <EmbedBili pv={pv} width={width} height={height} />;

			case PVService.Creofuga:
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
				return <></>;
		}
	},
);
