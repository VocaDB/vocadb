import PVContract from '@DataContracts/PVs/PVContract';
import React from 'react';

interface PiaproMetadata {
	timestamp?: string;
}

interface EmbedPiaproProps {
	pv: PVContract;
}

const EmbedPiapro = React.memo(
	({ pv }: EmbedPiaproProps): React.ReactElement => {
		const meta = pv.extendedMetadata
			? (pv.extendedMetadata as PiaproMetadata)
			: undefined;

		return meta && meta.timestamp ? (
			<audio
				controls
				controlsList="nodownload"
				src={`https://cdn.piapro.jp/mp3_a/${pv.pvId.slice(0, 2)}/${pv.pvId}_${
					meta.timestamp
				}_audition.mp3`}
			/>
		) : (
			// <object> embed instead of iframe because iframe doesn't work with flash disabled
			<object
				type="application/x-shockwave-flash"
				width="340"
				height="80"
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

export default EmbedPiapro;
