import ResourcesContract from '@DataContracts/ResourcesContract';
import ResourceRepository from '@Repositories/ResourceRepository';
import ko, { Observable } from 'knockout';
import _ from 'lodash';

export default class ResourcesManager {
	constructor(
		private resourcesRepo: ResourceRepository,
		private cultureCode: string,
	) {}

	private setsToLoad = (setNames: string[]): string[] => {
		var missing = _.filter(
			setNames,
			(setName) =>
				this.resources[setName as keyof Observable<ResourcesContract>] == null,
		);
		return missing;
	};

	public resources: Observable<ResourcesContract> = ko.observable({});

	public loadResources = (
		callback?: () => void,
		...setNames: string[]
	): void => {
		var setsToLoad = this.setsToLoad(setNames);
		this.resourcesRepo
			.getList(this.cultureCode, setsToLoad)
			.then((resources) => {
				_.each(
					setNames,
					(setName) =>
						(this.resources()[setName as keyof ResourcesContract] =
							resources[setName as keyof ResourcesContract]),
				);
				this.resources.valueHasMutated!();
				if (callback) callback();
			});
	};
}

export class ResourceSetFolderActivityEntry {
	public activityFeedEventNames = 'activityEntry_activityFeedEventNames';
}

export class ResourceSetFolderAlbum {
	public albumEditableFieldNames = 'album_albumEditableFieldNames';
}

export class ResourceSetFolderArtist {
	public artistEditableFieldNames = 'artist_artistEditableFieldNames';
}

export class ResourceSetFolderReleaseEvent {
	public releaseEventEditableFieldNames =
		'releaseEvent_releaseEventEditableFieldNames';
}

export class ResourceSetFolderSong {
	public songEditableFieldNames = 'song_songEditableFieldNames';
}

export class ResourceSetFolderSongList {
	public songListEditableFieldNames = 'songList_songListEditableFieldNames';
	public songListFeaturedCategoryNames =
		'songList_songListFeaturedCategoryNames';
}

export class ResourceSetFolderTag {
	public tagEditableFieldNames = 'tag_tagEditableFieldNames';
}

export class ResourceSetNames {
	public static activityEntry = new ResourceSetFolderActivityEntry();
	public static album = new ResourceSetFolderAlbum();
	public static artist = new ResourceSetFolderArtist();
	public static releaseEvent = new ResourceSetFolderReleaseEvent();
	public static song = new ResourceSetFolderSong();
	public static songList = new ResourceSetFolderSongList();
	public static tag = new ResourceSetFolderTag();

	public static artistTypeNames = 'artistTypeNames';
	public static contentLanguageSelectionNames = 'contentLanguageSelectionNames';
	public static discTypeNames = 'discTypeNames';
	public static songTypeNames = 'songTypeNames';
	public static userGroupNames = 'userGroupNames';
}
