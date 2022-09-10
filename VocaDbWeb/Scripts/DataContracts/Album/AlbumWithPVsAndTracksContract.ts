import { AlbumForApiContract } from '@/DataContracts/Album/AlbumForApiContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { SongInAlbumContract } from '@/DataContracts/Song/SongInAlbumContract';
import { SongWithPVsContract } from '@/DataContracts/Song/SongWithPVsContract';

export interface AlbumWithPVsAndTracksContract extends AlbumForApiContract {
	entryType: string /* TODO: enum */;
	pvs: PVContract[];
	tracks: (SongInAlbumContract & { song: SongWithPVsContract })[];
}
