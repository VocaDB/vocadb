import { PVContract } from '@/DataContracts/PVs/PVContract';
import React from 'react';

interface PiaproMetadata {
	Timestamp?: string;
}

interface EmbedPiaproProps {
	pv: PVContract;
	width?: number | string;
	height?: number | string;
}

export const EmbedPiapro = React.memo(
	({ pv, width, height }: EmbedPiaproProps): React.ReactElement => {
		const meta = pv.extendedMetadata
			? (JSON.parse(pv.extendedMetadata.json) as PiaproMetadata)
			: undefined;

		return meta && meta.Timestamp ? (
			<audio
				controls
				controlsList="nodownload"
				src={`https://cdn.piapro.jp/mp3_a/${pv.pvId.slice(0, 2)}/${pv.pvId}_${
					meta.Timestamp
				}_audition.mp3`}
				css={{ width: width, height: height }}
			/>
		) : (
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
