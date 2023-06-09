import { ArtistContract } from '@/types/DataContracts/Artist/ArtistContract';
import { LocalizedStringContract } from '@/types/DataContracts/Globalization/LocalizedStringContract';
import { AlbumType } from '@/types/Models/Albums/AlbumType';

// Corresponds to the CreateAlbumForApiContract record class in C#.
export interface CreateAlbumContract {
	artists: ArtistContract[];
	discType: AlbumType;
	names: LocalizedStringContract[];
}
