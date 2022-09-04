import { EmbedPVPreviewButtons } from '@/Components/Shared/Partials/PV/EmbedPVPreview';
import { ThumbItem } from '@/Components/Shared/Partials/Shared/ThumbItem';
import { useVdbPlayer } from '@/Components/VdbPlayer/VdbPlayerContext';
import { AlbumForApiContract } from '@/DataContracts/Album/AlbumForApiContract';
import { AlbumHelper } from '@/Helpers/AlbumHelper';
import { UrlHelper } from '@/Helpers/UrlHelper';
import { EntryType } from '@/Models/EntryType';
import { ImageSize } from '@/Models/Images/ImageSize';
import { AlbumRepository } from '@/Repositories/AlbumRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { HttpClient } from '@/Shared/HttpClient';
import { PlayMethod } from '@/Stores/VdbPlayer/PlayQueueStore';
import React from 'react';
import { Link } from 'react-router-dom';

const httpClient = new HttpClient();

const albumRepo = new AlbumRepository(httpClient, vdb.values.baseAddress);

interface AlbumThumbItemProps {
	album: AlbumForApiContract;
	tooltip?: boolean;
}

export const AlbumThumbItem = React.memo(
	({ album, tooltip }: AlbumThumbItemProps): React.ReactElement => {
		const { playQueue } = useVdbPlayer();

		const handlePlay = React.useCallback(
			async (method: PlayMethod) => {
				const albumWithPVsAndTracks = await albumRepo.getOneWithPVsAndTracks({
					id: album.id,
					lang: vdb.values.languagePreference,
				});

				const items = AlbumHelper.createPlayQueueItems(albumWithPVsAndTracks);

				playQueue.play(method, ...items);
			},
			[album, playQueue],
		);

		const [hover, setHover] = React.useState(false);
		const [isOpen, setIsOpen] = React.useState(false);

		return (
			<ThumbItem
				linkAs={Link}
				linkProps={{ to: EntryUrlMapper.details(EntryType.Album, album.id) }}
				thumbUrl={
					UrlHelper.imageThumb(album.mainPicture, ImageSize.SmallThumb) ??
					'/Content/unknown.png'
				}
				caption={album.name}
				entry={{ entryType: EntryType[EntryType.Album], id: album.id }}
				tooltip={tooltip}
				onPointerEnter={(): void => setHover(true)}
				onPointerLeave={(): void => setHover(false)}
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
