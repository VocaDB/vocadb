import SongType from '@/Models/Songs/SongType';

import ArtistContract from './Artist/ArtistContract';
import DuplicateEntryResultContract from './DuplicateEntryResultContract';

export default interface NewSongCheckResultContract {
	artists: ArtistContract[];

	matches: DuplicateEntryResultContract[];

	songType: SongType;

	title: string;

	titleLanguage: string; // TODO: content language selection
}
