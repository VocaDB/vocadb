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
			.map(({ song, pv }) => ({ song, pv: pv! }))
			.map(
				({ song, pv }) =>
					new PlayQueueItem(
						{
							entryType: 'Song' /* TODO: enum */,
							id: song.id,
							name: song.name,
							status: song.status,
							additionalNames: song.additionalNames,
							artistString: song.artistString,
							mainPicture: { urlThumb: song.mainPicture?.urlThumb! },
							songType: song.songType,
						},
						{
							id: pv.id!,
							service: pv.service,
							pvId: pv.pvId,
							pvType: pv.pvType,
						},
					),
			);
	};

	public static createItemsFromAlbum = (
		album: AlbumWithPVsAndTracksContract,
	): PlayQueueItem[] => {
		const primaryPV = PVHelper.primaryPV(album.pvs, true);

		const albumItem = primaryPV
			? new PlayQueueItem(
					{
						entryType: 'Album' /* TODO: enum */,
						id: album.id,
						name: album.name,
						status: album.status!,
						additionalNames: album.additionalNames,
						artistString: album.artistString,
						mainPicture: { urlThumb: album.mainPicture?.urlThumb! },
					},
					{
						id: primaryPV.id!,
						service: primaryPV.service,
						pvId: primaryPV.pvId,
						pvType: primaryPV.pvType,
					},
			  )
			: undefined;

		const songs = album.tracks.map((track) => track.song);
		const songItems = PlayQueueHelper.createItemsFromSongs(songs);

		return (albumItem ? [albumItem] : []).concat(songItems);
	};
}
