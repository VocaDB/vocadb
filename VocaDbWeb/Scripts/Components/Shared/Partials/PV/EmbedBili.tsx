import PVContract from '@DataContracts/PVs/PVContract';
import React from 'react';

interface EmbedBiliProps {
	pv: PVContract;
	width: number;
	height: number;
}

const EmbedBili = React.memo(
	({ pv, width, height }: EmbedBiliProps): React.ReactElement => {
		return height >= 274 && width >= 480 ? (
			<div
				style={{
					overflow: 'hidden',
					width: 'max-content',
					height: 'max-content',
				}}
			>
				{/* eslint-disable-next-line jsx-a11y/iframe-has-title */}
				<iframe
					src={`https://player.bilibili.com/player.html?aid=${pv.pvId}&page=1`}
					scrolling="no"
					// @ts-ignore
					border="0"
					frameBorder="no"
					framespacing="0"
					allowFullScreen
					width={width}
					height={height + 67}
					style={{
						marginBottom: '-43px',
						width: `${width}px`,
						height: `${height + 67}px`,
					}}
				/>
			</div>
		) : (
			// eslint-disable-next-line jsx-a11y/iframe-has-title
			<iframe
				src={`https://player.bilibili.com/player.html?aid=${pv.pvId}&page=1`}
				scrolling="no"
				// @ts-ignore
				border="0"
				frameBorder="no"
				framespacing="0"
				allowFullScreen
				width={width}
				height={height}
			/>
		);
	},
);

export default EmbedBili;
