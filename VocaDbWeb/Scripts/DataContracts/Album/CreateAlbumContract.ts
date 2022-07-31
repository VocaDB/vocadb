import AlbumType from '@Models/Albums/AlbumType';

import ArtistContract from '../Artist/ArtistContract';
import LocalizedStringContract from '../Globalization/LocalizedStringContract';

// Corresponds to the CreateAlbumForApiContract record class in C#.
export default interface CreateAlbumContract {
	artists: ArtistContract[];
	discType: AlbumType;
	names: LocalizedStringContract[];
}
