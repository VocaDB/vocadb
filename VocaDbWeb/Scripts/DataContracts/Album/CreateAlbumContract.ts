import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { LocalizedStringContract } from '@/DataContracts/Globalization/LocalizedStringContract';
import { AlbumType } from '@/Models/Albums/AlbumType';

// Corresponds to the CreateAlbumForApiContract record class in C#.
export interface CreateAlbumContract {
	artists: ArtistContract[];
	discType: AlbumType;
	names: LocalizedStringContract[];
}
