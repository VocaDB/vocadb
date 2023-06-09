import { ArtistForAlbumContract } from '@/types/DataContracts/ArtistForAlbumContract';
import { EnglishTranslatedStringContract } from '@/types/DataContracts/Globalization/EnglishTranslatedStringContract';
import { LocalizedStringWithIdContract } from '@/types/DataContracts/Globalization/LocalizedStringWithIdContract';
import { PVContract } from '@/types/DataContracts/PVs/PVContract';
import { ReleaseEventContract } from '@/types/DataContracts/ReleaseEvents/ReleaseEventContract';
import { LyricsForSongContract } from '@/types/DataContracts/Song/LyricsForSongContract';
import { SongContract } from '@/types/DataContracts/Song/SongContract';
import { WebLinkContract } from '@/types/DataContracts/WebLinkContract';
import { EntryStatus } from '@/types/Models/EntryStatus';
import { SongType } from '@/types/Models/Songs/SongType';

// Corresponds to the SongForEditForApiContract record class in C#.
export interface SongForEditContract {
	albumEventId?: number;
	albumReleaseDate?: string;
	artists: ArtistForAlbumContract[];
	canDelete?: boolean;
	defaultNameLanguage: string;
	deleted: boolean;
	hasAlbums: boolean;
	id: number;
	lengthSeconds: number;
	lyrics: LyricsForSongContract[];
	maxMilliBpm?: number;
	minMilliBpm?: number;
	name?: string;
	names: LocalizedStringWithIdContract[];
	notes: EnglishTranslatedStringContract;
	originalVersion?: SongContract;
	// Publish date, should be in ISO format, UTC timezone. Only includes the date component, no time.
	publishDate?: string;
	pvs: PVContract[];
	releaseEvent?: ReleaseEventContract;
	songType: SongType;
	status: EntryStatus;
	tags: number[];
	updateNotes?: string;
	webLinks: WebLinkContract[];
	cultureCodes: string[];
}
