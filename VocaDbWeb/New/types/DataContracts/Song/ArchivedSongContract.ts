import { ArchivedTranslatedStringContract } from '@/types/DataContracts/ArchivedTranslatedStringContract';
import { ArchivedWebLinkContract } from '@/types/DataContracts/ArchivedWebLinkContract';
import { LocalizedStringContract } from '@/types/DataContracts/Globalization/LocalizedStringContract';
import { ObjectRefContract } from '@/types/DataContracts/ObjectRefContract';
import { ArchivedPVContract } from '@/types/DataContracts/PVs/ArchivedPVContract';
import { AlbumForSongRefContract } from '@/types/DataContracts/Song/AlbumForSongRefContract';
import { ArchivedArtistForSongContract } from '@/types/DataContracts/Song/ArchivedArtistForSongContract';
import { LyricsForSongContract } from '@/types/DataContracts/Song/LyricsForSongContract';
import { SongType } from '@/types/Models/Songs/SongType';

export interface ArchivedSongContract {
	albums?: AlbumForSongRefContract[];
	artists?: ArchivedArtistForSongContract[];
	id: number;
	lengthSeconds: number;
	lyrics?: LyricsForSongContract[];
	maxMilliBpm?: number;
	minMilliBpm?: number;
	names?: LocalizedStringContract[];
	nicoId?: string;
	notes: string;
	notesEng: string;
	originalVersion?: ObjectRefContract;
	publishDate?: string;
	pvs?: ArchivedPVContract[];
	releaseEvent?: ObjectRefContract;
	songType: SongType;
	translatedName: ArchivedTranslatedStringContract;
	webLinks?: ArchivedWebLinkContract[];
}
