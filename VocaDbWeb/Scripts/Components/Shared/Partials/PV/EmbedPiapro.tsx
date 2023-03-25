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
			<a href={pv.url} target="_blank" rel="noreferrer">
				<div
					css={{
						width: '100%',
						height: '100%',
						backgroundColor: 'rgb(28, 28, 28)',
						backgroundSize: 'cover',
						backgroundPosition: 'center',
					}}
					style={{
						backgroundImage: `url(/Content/banners/bnr_piapro468x120.jpg)`,
					}}
				/>
			</a>
		);
	},
);
