import { PVContract } from '@/DataContracts/PVs/PVContract';
import React from 'react';

interface PiaproMetadata {
	Timestamp?: string;
}

export const getPiaproTimestamp = (pv: PVContract): string | undefined => {
	const meta = pv.extendedMetadata
		? (JSON.parse(pv.extendedMetadata.json) as PiaproMetadata)
		: undefined;

	return meta?.Timestamp;
};

export const getPiaproUrlWithTimestamp = (
	pv: PVContract,
): string | undefined => {
	const timestamp = getPiaproTimestamp(pv);

	if (timestamp === undefined) return undefined;

	return `https://cdn.piapro.jp/mp3_a/${pv.pvId.slice(0, 2)}/${
		pv.pvId
	}_${timestamp}_audition.mp3`;
};

interface EmbedPiaproProps {
	pv: PVContract;
	width?: number | string;
	height?: number | string;
}

export const EmbedPiapro = React.memo(
	({ pv, width, height }: EmbedPiaproProps): React.ReactElement => {
		return (
			// <object> embed instead of iframe because iframe doesn't work with flash disabled
			<object
				type="application/x-shockwave-flash"
				width={width ?? 340}
				height={height ?? 80}
				style={{ border: 0 }}
				data={`//piapro.jp/modpub/swf/player03_h.swf?030503&id=${pv.pvId}&c=1`}
			>
				<param
					name="movie"
					value={`//piapro.jp/modpub/swf/player03_h.swf?030503&id=${pv.pvId}&c=1`}
				/>
				<param name="quality" value="high" />
			</object>
		);
	},
);
