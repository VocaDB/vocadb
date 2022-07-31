import { ArchivedTranslatedStringContract } from '@/DataContracts/ArchivedTranslatedStringContract';
import { ArchivedWebLinkContract } from '@/DataContracts/ArchivedWebLinkContract';
import { LocalizedStringContract } from '@/DataContracts/Globalization/LocalizedStringContract';
import { ObjectRefContract } from '@/DataContracts/ObjectRefContract';
import { ArchivedPVContract } from '@/DataContracts/PVs/ArchivedPVContract';
import { AlbumForSongRefContract } from '@/DataContracts/Song/AlbumForSongRefContract';
import { ArchivedArtistForSongContract } from '@/DataContracts/Song/ArchivedArtistForSongContract';
import { LyricsForSongContract } from '@/DataContracts/Song/LyricsForSongContract';
import { SongType } from '@/Models/Songs/SongType';

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
