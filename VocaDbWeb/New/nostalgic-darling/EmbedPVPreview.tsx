import React from 'react';
import { usePlayerStore } from './stores/usePlayerStore';
import { SongApiContract } from '@/types/DataContracts/Song/SongApiContract';
import CustomImage from '@/components/Image/Image';
import { PVContract } from '@/types/DataContracts/PVs/PVContract';

interface EmbedPVPreviewProps {
	song: SongApiContract;
	pv: PVContract;
}

export default function EmbedPVPreview({ song, pv }: EmbedPVPreviewProps) {
	const [setPlayerBounds, setSong, currSong, currPV] = usePlayerStore((set) => [
		set.setPlayerBounds,
		set.loadSong,
		set.song,
		set.pv,
	]);
	const embedPVPreviewRef = React.useRef<HTMLDivElement>(undefined!);

	const updatePlayerBounds = () => {
		const rect = embedPVPreviewRef.current.getBoundingClientRect();
		setPlayerBounds({
			x: rect.x + window.scrollX,
			y: rect.y + window.scrollY,
			width: rect.width,
			height: rect.height,
		});
	};

	const handleResize = React.useCallback(() => {
		if (currSong?.id !== song.id) {
			return;
		}
		updatePlayerBounds();
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
		if (currPV?.url !== pv.url) {
			setPlayerBounds(undefined);
		}
	}, [pv]);

	return (
		<div
			onClick={() => {
				setSong(song, pv);
				updatePlayerBounds();
			}}
			ref={embedPVPreviewRef}
			style={{ width: '30vw', aspectRatio: '16/9' }}
		>
			<CustomImage
				width={500}
				height={315}
				style={{ width: '100%', height: '100%' }}
				src={song.mainPicture?.urlOriginal ?? 'todo: fallback'}
				alt="Start the song"
			/>
		</div>
	);
}

