import React from 'react';
import { usePlayerStore } from './stores/usePlayerStore';
import { SongContract } from '@/types/DataContracts/Song/SongContract';

interface EmbedPVPreviewProps {
	song: SongContract;
}

export default function EmbedPVPreview({ song }: EmbedPVPreviewProps) {
	const [setPlayerBounds, setSong] = usePlayerStore((set) => [set.setPlayerBounds, set.loadSong]);
	const embedPVPreviewRef = React.useRef<HTMLDivElement>(undefined!);

	const handleResize = React.useCallback(() => {
		const rect = embedPVPreviewRef.current.getBoundingClientRect();
		setPlayerBounds({
			x: rect.x + window.scrollX,
			y: rect.y + window.scrollY,
			width: rect.width,
			height: rect.height,
		});
	}, [song]);

	React.useLayoutEffect(() => {
		window.addEventListener('resize', handleResize);
		handleResize();

		return (): void => {
			window.removeEventListener('resize', handleResize);
		};
	}, [handleResize]);

	React.useLayoutEffect(() => {
		return (): void => {
			setPlayerBounds(undefined);
		};
	}, []);

	React.useEffect(() => {
		setSong(song);
	}, []);

	return <div ref={embedPVPreviewRef} style={{ width: 560, height: 315 }} />;
}

