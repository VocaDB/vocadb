import ArtistContract from '@/DataContracts/Artist/ArtistContract';
import DuplicateEntryResultContract from '@/DataContracts/DuplicateEntryResultContract';
import SongType from '@/Models/Songs/SongType';

export default interface NewSongCheckResultContract {
	artists: ArtistContract[];

	matches: DuplicateEntryResultContract[];

	songType: SongType;

	title: string;

	titleLanguage: string; // TODO: content language selection
}
