import AlbumType from '@Models/Albums/AlbumType';

import ArtistForAlbumContract from '../ArtistForAlbumContract';
import EntryPictureFileContract from '../EntryPictureFileContract';
import EnglishTranslatedStringContract from '../Globalization/EnglishTranslatedStringContract';
import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import PVContract from '../PVs/PVContract';
import SongInAlbumEditContract from '../Song/SongInAlbumEditContract';
import WebLinkContract from '../WebLinkContract';
import AlbumDiscPropertiesContract from './AlbumDiscPropertiesContract';
import AlbumReleaseContract from './AlbumReleaseContract';

// Corresponds to the AlbumForEditForApiContract record class in C#.
export default interface AlbumForEditContract {
	artistLinks: ArtistForAlbumContract[];
	canDelete?: boolean;
	coverPictureMime?: string;
	defaultNameLanguage: string;
	deleted?: boolean;
	description: EnglishTranslatedStringContract;
	discs: AlbumDiscPropertiesContract[];
	discType: AlbumType;
	id: number;
	identifiers: string[];
	name?: string;
	names: LocalizedStringWithIdContract[];
	originalRelease: AlbumReleaseContract;
	pictures: EntryPictureFileContract[];
	pvs: PVContract[];
	songs: SongInAlbumEditContract[];
	status: string;
	updateNotes?: string;
	webLinks: WebLinkContract[];
}
