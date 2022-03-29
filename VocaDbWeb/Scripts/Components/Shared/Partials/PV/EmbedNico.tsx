import React from 'react';

interface EmbedNicoProps {
	pvId: string;
	width: number | string;
	height: number | string;
	id?: string;
	enableApi?: boolean;
}

const EmbedNico = React.memo(
	({
		pvId,
		width,
		height,
		id,
		enableApi = false,
	}: EmbedNicoProps): React.ReactElement => {
		return (
			<div id={id}>
				{/* eslint-disable-next-line jsx-a11y/iframe-has-title */}
				<iframe
					width={width}
					height={height}
					allowFullScreen
					style={{ border: 'none' }}
					src={`//embed.nicovideo.jp/watch/${pvId}?jsapi=${
						enableApi ? 1 : 0
					}&noRelatedVideo=0&autoplay=0&defaultNoComment=0&noLinkToNiconico=0&noController=0&noHeader=0&noTags=0&noShare=0`}
					allow="autoplay; fullscreen"
					key={pvId}
				/>
			</div>
		);
	},
);

export default EmbedNico;
