import React from 'react';
import { usePlayerStore } from './stores/usePlayerStore';
import { SongApiContract } from '@/types/DataContracts/Song/SongApiContract';
import CustomImage from '@/components/Image/Image';

interface EmbedPVPreviewProps {
	song: SongApiContract;
}

export default function EmbedPVPreview({ song }: EmbedPVPreviewProps) {
	const [setPlayerBounds, setSong, currSong] = usePlayerStore((set) => [
		set.setPlayerBounds,
		set.loadSong,
		set.song,
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

	return (
		<div
			onClick={() => {
				setSong(song);
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

