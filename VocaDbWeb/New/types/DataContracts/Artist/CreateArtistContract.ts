import { EntryPictureFileContract } from '@/types/DataContracts/EntryPictureFileContract';
import { LocalizedStringContract } from '@/types/DataContracts/Globalization/LocalizedStringContract';
import { WebLinkContract } from '@/types/DataContracts/WebLinkContract';
import { ArtistType } from '@/types/Models/Artists/ArtistType';

// Corresponds to the CreateArtistForApiContract record class in C#.
export interface CreateArtistContract {
	artistType: ArtistType;
	description: string;
	draft: boolean;
	names: LocalizedStringContract[];
	pictureData?: EntryPictureFileContract;
	webLink?: WebLinkContract;
}
