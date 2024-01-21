import { RewindVideo } from '@/vocadb-rewind/src/rewind/Rewind';
import { Player } from '@remotion/player';
import React, { useEffect, useRef } from 'react';

import data from './shiro.json';

export default function RewindPage(): React.ReactElement {
	const iframeRef = useRef<HTMLIFrameElement | null>(null);

	const postMessage = (): void => {
		console.log('Sending rewind data');
		const iframe = iframeRef.current;
		if (iframe) {
			iframe.contentWindow?.postMessage(data, '*');
		} else {
			console.log('iframeRef is null');
		}
	};

	useEffect(() => {}, []);

	return (
		<iframe
			allowFullScreen
			style={{
				height: 'calc(100vh - 150px)',
				width: '100%',
			}}
			ref={iframeRef}
			onLoad={(): any => postMessage()}
			title="VocaDB Rewind"
			id="rewind_iframe"
			scrolling="no"
			src="http://vocadb-rewind-page.vercel.app"
		/>
	);
}
