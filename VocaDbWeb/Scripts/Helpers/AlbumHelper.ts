import { AlbumForApiContract } from '@/DataContracts/Album/AlbumForApiContract';
import { PVHelper } from '@/Helpers/PVHelper';
import { AlbumType } from '@/Models/Albums/AlbumType';
import { ContentFocus } from '@/Models/ContentFocus';
import { EntryType } from '@/Models/EntryType';
import { PlayQueueItem } from '@/Stores/VdbPlayer/PlayQueueStore';

export class AlbumHelper {
	public static getContentFocus = (albumType: AlbumType): ContentFocus => {
		switch (albumType) {
			case AlbumType.Artbook:
				return ContentFocus.Illustration;

			case AlbumType.Video:
				return ContentFocus.Video;

			default:
				return ContentFocus.Music;
		}
	};

	public static createPlayQueueItems = (
		albumWithPVsAndTracks: AlbumForApiContract,
	): PlayQueueItem[] => {
		const primaryPV = PVHelper.primaryPV(albumWithPVsAndTracks.pvs ?? [], true);

		const albumItem = primaryPV
			? new PlayQueueItem(
					{
						...albumWithPVsAndTracks,
						entryType: EntryType[EntryType.Album],
					},
					primaryPV,
			  )
			: undefined;

		const songs = (albumWithPVsAndTracks.tracks ?? [])
			.map((track) => track.song)
			.filter((song) => !!song && !!song.pvs);

		const songItems = songs
			.map((song) => ({
				entry: { ...song, entryType: EntryType[EntryType.Song] },
				pv: PVHelper.primaryPV(song.pvs!, true),
			}))
			.filter(({ pv }) => !!pv)
			.map(({ entry, pv }) => new PlayQueueItem(entry, pv!));

		const items = (albumItem ? [albumItem] : []).concat(songItems);

		return items;
	};
}
