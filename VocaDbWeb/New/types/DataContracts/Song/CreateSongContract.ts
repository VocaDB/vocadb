import { LocalizedStringContract } from '@/types/DataContracts/Globalization/LocalizedStringContract';
import { ArtistForSongContract } from '@/types/DataContracts/Song/ArtistForSongContract';
import { SongContract } from '@/types/DataContracts/Song/SongContract';
import { SongType } from '@/types/Models/Songs/SongType';

// Corresponds to the CreateSongForApiContract record class in C#.
export interface CreateSongContract {
	artists: ArtistForSongContract[];
	draft: boolean;
	names: LocalizedStringContract[];
	originalVersion?: SongContract;
	pvUrls: string[];
	reprintPVUrl: string;
	songType: SongType;
}
