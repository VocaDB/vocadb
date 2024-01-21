import { httpClient } from '@/Shared/HttpClient';
import { urlMapper } from '@/Shared/UrlMapper';
import React, { useEffect, useRef, useState } from 'react';

export default function RewindPage(): React.ReactElement {
	const iframeRef = useRef<HTMLIFrameElement | null>(null);
	const [rewindData, setRewindData] = useState<null | object>(null);

	const postMessage = (): void => {
		console.log('Sending rewind data');
		const iframe = iframeRef.current;
		if (iframe) {
			iframe.contentWindow?.postMessage(rewindData, '*');
		} else {
			console.log('iframeRef is null');
		}
	};

	useEffect(() => {
		httpClient
			.get<object>(urlMapper.mapRelative('/api/users/rewind'))
			.then((resp) => setRewindData(resp));
	}, []);

	useEffect(() => {
		postMessage();
	}, [rewindData]);

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
			src="https://vocadb-rewind-page.vercel.app/"
		/>
	);
}
