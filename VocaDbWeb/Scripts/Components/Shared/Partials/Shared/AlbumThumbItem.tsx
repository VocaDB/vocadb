import { EmbedPVPreviewButtons } from '@/Components/Shared/Partials/PV/EmbedPVPreview';
import { ThumbItem } from '@/Components/Shared/Partials/Shared/ThumbItem';
import { useVdbPlayer } from '@/Components/VdbPlayer/VdbPlayerContext';
import { AlbumForApiContract } from '@/DataContracts/Album/AlbumForApiContract';
import { PVHelper } from '@/Helpers/PVHelper';
import { UrlHelper } from '@/Helpers/UrlHelper';
import { EntryType } from '@/Models/EntryType';
import { ImageSize } from '@/Models/Images/ImageSize';
import {
	AlbumOptionalField,
	AlbumRepository,
	SongOptionalField,
} from '@/Repositories/AlbumRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { HttpClient } from '@/Shared/HttpClient';
import { PlayMethod, PlayQueueItem } from '@/Stores/VdbPlayer/PlayQueueStore';
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
				const albumWithPVsAndTracks = await albumRepo.getOneWithComponents({
					id: album.id,
					lang: vdb.values.languagePreference,
					fields: [AlbumOptionalField.PVs, AlbumOptionalField.Tracks],
					songFields: [SongOptionalField.PVs],
				});

				const primaryPV = PVHelper.primaryPV(albumWithPVsAndTracks.pvs ?? []);
				const primaryPVItem = primaryPV
					? new PlayQueueItem(
							{
								...albumWithPVsAndTracks,
								entryType: EntryType[EntryType.Album],
							},
							primaryPV,
					  )
					: undefined;

				const tracks = albumWithPVsAndTracks.tracks ?? [];
				const trackItems = tracks
					.map((track) => track.song)
					.filter((song) => !!song && !!song.pvs)
					.map((song) => ({
						entry: { ...song, entryType: EntryType[EntryType.Song] },
						pv: PVHelper.primaryPV(song.pvs!),
					}))
					.filter(({ pv }) => !!pv)
					.map(({ entry, pv }) => new PlayQueueItem(entry, pv!));

				const items = (primaryPVItem ? [primaryPVItem] : []).concat(trackItems);

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
				onMouseEnter={(): void => setHover(true)}
				onMouseLeave={(): void => setHover(false)}
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
