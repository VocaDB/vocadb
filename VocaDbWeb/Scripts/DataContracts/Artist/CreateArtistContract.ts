import EntryPictureFileContract from '@/DataContracts/EntryPictureFileContract';
import LocalizedStringContract from '@/DataContracts/Globalization/LocalizedStringContract';
import WebLinkContract from '@/DataContracts/WebLinkContract';
import ArtistType from '@/Models/Artists/ArtistType';

// Corresponds to the CreateArtistForApiContract record class in C#.
export default interface CreateArtistContract {
	artistType: ArtistType;
	description: string;
	draft: boolean;
	names: LocalizedStringContract[];
	pictureData?: EntryPictureFileContract;
	webLink?: WebLinkContract;
}
