import PVContract from '@DataContracts/PVs/PVContract';
import PVService from '@Models/PVs/PVService';
import _ from 'lodash';
import React from 'react';

import EmbedBili from './EmbedBili';
import EmbedNico from './EmbedNico';
import EmbedPiapro from './EmbedPiapro';

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
	width?: number;
	height?: number;
	autoplay?: boolean;
	enableApi?: boolean;
	id?: string;
}

const EmbedPV = React.memo(
	({
		pv,
		width = 560,
		height = 315,
		autoplay = false,
		enableApi = false,
		id,
	}: EmbedPVProps): React.ReactElement => {
		switch (PVService[pv.service as keyof typeof PVService]) {
			case PVService.Bandcamp:
				return (
					// eslint-disable-next-line jsx-a11y/iframe-has-title
					<iframe
						style={{ border: 0, width: '100%', height: '120px' }}
						src={`https://bandcamp.com/EmbeddedPlayer/size=large/bgcol=ffffff/linkcol=0687f5/tracklist=false/artwork=small/track=${pv.pvId}/transparent=true/`}
						seamless
					/>
				);

			case PVService.Bilibili:
				return <EmbedBili pv={pv} width={width} height={height} />;

			case PVService.File:
			case PVService.LocalFile:
				return isAudio(pv.url) ? (
					<audio id={id} controls controlsList="nodownload" src={pv.url} />
				) : (
					<div style={{ width: `${width}px`, height: `${height}px` }}>
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
				return (
					<EmbedNico
						pvId={pv.pvId}
						width={width}
						height={height}
						id={id}
						enableApi={enableApi}
					/>
				);

			case PVService.Piapro:
				return <EmbedPiapro pv={pv} />;

			case PVService.SoundCloud:
				return (
					// eslint-disable-next-line jsx-a11y/iframe-has-title
					<iframe
						id={id}
						width={width}
						height={Math.min(height, 166)}
						scrolling="no"
						frameBorder="no"
						src={`https://w.soundcloud.com/player/?url=https%3A%2F%2Fapi.soundcloud.com%2Ftracks%2F${
							pv.pvId.split(' ')[0]
						}&amp;auto_play=false&amp;show_artwork=true&amp;color=ff7700`}
					/>
				);

			case PVService.Youtube:
				return (
					// eslint-disable-next-line jsx-a11y/iframe-has-title
					<iframe
						id={id}
						width={width}
						height={height}
						src={`https://www.youtube.com/embed/${pv.pvId}?autoplay=${
							autoplay ? 1 : 0
						}&enablejsapi=${enableApi ? 1 : 0}&origin=${
							enableApi && vdb.values.hostAddress
						}`}
						frameBorder="0"
						// @ts-ignore
						wmode="Opaque"
						allowFullScreen
					/>
				);

			case PVService.Vimeo:
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
					/>
				);

			case PVService.Creofuga:
				return (
					// eslint-disable-next-line jsx-a11y/iframe-has-title
					<iframe
						width={width}
						height={Math.min(height, 120)}
						scrolling="no"
						frameBorder="no"
						src={`https://creofuga.net/audios/player?color=black&id=${pv.pvId}`}
					/>
				);

			default:
				return <></>;
		}
	},
);

export default EmbedPV;
