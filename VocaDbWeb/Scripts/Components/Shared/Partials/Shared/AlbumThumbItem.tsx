import { EmbedPVPreviewButtons } from '@/Components/Shared/Partials/PV/EmbedPVPreview';
import { ThumbItem } from '@/Components/Shared/Partials/Shared/ThumbItem';
import { useVdbPlayer } from '@/Components/VdbPlayer/VdbPlayerContext';
import { AlbumForApiContract } from '@/DataContracts/Album/AlbumForApiContract';
import { UrlHelper } from '@/Helpers/UrlHelper';
import { EntryType } from '@/Models/EntryType';
import { ImageSize } from '@/Models/Images/ImageSize';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { PlayMethod } from '@/Stores/VdbPlayer/PlayQueueStore';
import React from 'react';
import { Link } from 'react-router-dom';

interface AlbumThumbItemProps {
	album: AlbumForApiContract;
	tooltip?: boolean;
}

export const AlbumThumbItem = React.memo(
	({ album, tooltip }: AlbumThumbItemProps): React.ReactElement => {
		const { playQueue } = useVdbPlayer();

		const handlePlay = React.useCallback(
			async (method: PlayMethod) => {
				await playQueue.loadItemsAndPlay(method, {
					...album,
					entryType: EntryType.Album,
				});
			},
			[album, playQueue],
		);

		const thumbItemRef = React.useRef<HTMLAnchorElement>(undefined!);

		const [hover, setHover] = React.useState(false);
		const [isOpen, setIsOpen] = React.useState(false);

		// HACK: https://github.com/facebook/react/issues/6807#issuecomment-1240312500.
		React.useLayoutEffect(() => {
			const handleMouseEnter = (): void => setHover(true);
			const handleMouseLeave = (): void => setHover(false);

			const thumbItem = thumbItemRef.current;

			thumbItem.addEventListener('mouseenter', handleMouseEnter);
			thumbItem.addEventListener('mouseleave', handleMouseLeave);

			return (): void => {
				thumbItem.removeEventListener('mouseenter', handleMouseEnter);
				thumbItem.removeEventListener('mouseleave', handleMouseLeave);
			};
		}, []);

		return (
			<ThumbItem
				linkAs={Link}
				linkProps={{ to: EntryUrlMapper.details(EntryType.Album, album.id) }}
				thumbUrl={
					UrlHelper.imageThumb(album.mainPicture, ImageSize.SmallThumb) ??
					'/Content/unknown.png'
				}
				caption={album.name}
				entry={{ entryType: EntryType.Album, id: album.id }}
				tooltip={tooltip}
				ref={thumbItemRef}
			>
				{(hover || isOpen) && (
					<EmbedPVPreviewButtons
						onPlay={handlePlay}
						onToggle={(isOpen): void => setIsOpen(isOpen)}
					/>
				)}
			</ThumbItem>
		);
	},
);
