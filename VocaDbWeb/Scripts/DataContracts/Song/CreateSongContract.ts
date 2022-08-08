import LocalizedStringContract from '@/DataContracts/Globalization/LocalizedStringContract';
import ArtistForSongContract from '@/DataContracts/Song/ArtistForSongContract';
import SongContract from '@/DataContracts/Song/SongContract';
import SongType from '@/Models/Songs/SongType';

// Corresponds to the CreateSongForApiContract record class in C#.
export default interface CreateSongContract {
	artists: ArtistForSongContract[];
	draft: boolean;
	names: LocalizedStringContract[];
	originalVersion?: SongContract;
	pvUrls: string[];
	reprintPVUrl: string;
	songType: SongType;
}
