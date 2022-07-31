import SongType from '@Models/Songs/SongType';

import LocalizedStringContract from '../Globalization/LocalizedStringContract';
import ArtistForSongContract from './ArtistForSongContract';
import SongContract from './SongContract';

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
