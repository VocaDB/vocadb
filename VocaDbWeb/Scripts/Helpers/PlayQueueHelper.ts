import { AlbumWithPVsAndTracksContract } from '@/DataContracts/Album/AlbumWithPVsAndTracksContract';
import { SongWithPVsContract } from '@/DataContracts/Song/SongWithPVsContract';
import { PVHelper } from '@/Helpers/PVHelper';
import { PlayQueueItem } from '@/Stores/VdbPlayer/PlayQueueStore';

export class PlayQueueHelper {
	public static createItemsFromSongs = (
		songs: SongWithPVsContract[],
	): PlayQueueItem[] => {
		return songs
			.map((song) => ({
				song: song,
				pv: PVHelper.primaryPV(song.pvs, true),
			}))
			.filter(({ pv }) => !!pv)
			.map(({ song, pv }) => new PlayQueueItem(song, pv!));
	};

	public static createItemsFromAlbum = (
		album: AlbumWithPVsAndTracksContract,
	): PlayQueueItem[] => {
		const primaryPV = PVHelper.primaryPV(album.pvs, true);

		const albumItem = primaryPV
			? new PlayQueueItem(album, primaryPV)
			: undefined;

		const songs = album.tracks.map((track) => track.song);
		const songItems = PlayQueueHelper.createItemsFromSongs(songs);

		return (albumItem ? [albumItem] : []).concat(songItems);
	};
}
