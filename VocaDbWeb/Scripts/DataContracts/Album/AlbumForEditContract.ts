import AlbumDiscPropertiesContract from './AlbumDiscPropertiesContract';
import AlbumReleaseContract from './AlbumReleaseContract';
import ArtistForAlbumContract from '../ArtistForAlbumContract';
import EnglishTranslatedStringContract from '../Globalization/EnglishTranslatedStringContract';
import EntryPictureFileContract from '../EntryPictureFileContract';
import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import PVContract from '../PVs/PVContract';
import SongInAlbumEditContract from '../Song/SongInAlbumEditContract';
import WebLinkContract from '../WebLinkContract';

	export default interface AlbumForEditContract {

		artistLinks: ArtistForAlbumContract[];

		coverPictureMime?: string;

		defaultNameLanguage: string;

		description: EnglishTranslatedStringContract;

		discs: AlbumDiscPropertiesContract[];

		discType: string;

		id: number;

		identifiers: string[];

		names: LocalizedStringWithIdContract[];

		originalRelease: AlbumReleaseContract;

		pictures: EntryPictureFileContract[];

		pvs: PVContract[];

		songs: SongInAlbumEditContract[];

		status: string;

		updateNotes?: string;

		webLinks: WebLinkContract[];

	}