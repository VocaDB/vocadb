import { ArtistContract } from '@/types/DataContracts/Artist/ArtistContract';
import { DuplicateEntryResultContract } from '@/types/DataContracts/DuplicateEntryResultContract';
import { SongType } from '@/types/Models/Songs/SongType';

export interface NewSongCheckResultContract {
	artists: ArtistContract[];

	matches: DuplicateEntryResultContract[];

	songType: SongType;

	title: string;

	titleLanguage: string; // TODO: content language selection
}
