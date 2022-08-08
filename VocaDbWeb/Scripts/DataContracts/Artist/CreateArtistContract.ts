import ArtistType from '@/Models/Artists/ArtistType';

import EntryPictureFileContract from '../EntryPictureFileContract';
import LocalizedStringContract from '../Globalization/LocalizedStringContract';
import WebLinkContract from '../WebLinkContract';

// Corresponds to the CreateArtistForApiContract record class in C#.
export default interface CreateArtistContract {
	artistType: ArtistType;
	description: string;
	draft: boolean;
	names: LocalizedStringContract[];
	pictureData?: EntryPictureFileContract;
	webLink?: WebLinkContract;
}
