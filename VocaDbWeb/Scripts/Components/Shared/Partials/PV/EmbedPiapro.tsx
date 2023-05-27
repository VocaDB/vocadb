import { PVContract } from '@/DataContracts/PVs/PVContract';
import React from 'react';

interface EmbedPiaproProps {
	pv: PVContract;
	width?: number | string;
	height?: number | string;
}
export const EmbedPiapro = React.memo(
	({ pv, width, height }: EmbedPiaproProps): React.ReactElement => {
		return (
			// eslint-disable-next-line jsx-a11y/iframe-has-title
			<iframe
				width={width}
				height={height}
				style={{ border: 0 }}
				src={`//piapro.jp/html5_player_popup/?id=${pv.pvId}`}
			/>
		);
	},
);
