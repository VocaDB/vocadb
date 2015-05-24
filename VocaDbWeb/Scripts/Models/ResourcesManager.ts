
module vdb.models {

	import dc = vdb.dataContracts;

	export class ResourcesManager {

		constructor(private resourcesRepo: vdb.repositories.ResourceRepository,
			private cultureCode: string) { }

		private setsToLoad = (setNames: string[]) => {
			var missing = _.where(setNames, setName => this.resources[setName] == null);
			return missing;
		}

		public resources: KnockoutObservable<dc.ResourcesContract> = ko.observable({});

		public loadResources = (callback?: () => void, ...setNames: string[]) => {
			var setsToLoad = this.setsToLoad(setNames);
			this.resourcesRepo.getList(this.cultureCode, setsToLoad, resources => {
				_.each(setNames, setName => this.resources()[setName] = resources[setName]);
				this.resources.valueHasMutated();
				if (callback)
					callback();
			});
		}

	}

	export class ResourceSetFolderActivityEntry {

		public activityFeedEventNames = "activityEntry_activityFeedEventNames";

	}

	export class ResourceSetFolderAlbum {
		
		public albumEditableFieldNames = "album_albumEditableFieldNames";

	}

	export class ResourceSetFolderArtist {

		public artistEditableFieldNames = "artist_artistEditableFieldNames";

	}

	export class ResourceSetFolderSong {

		public songEditableFieldNames = "song_songEditableFieldNames";

	}

	export class ResourceSetNames {

		public static activityEntry: ResourceSetFolderActivityEntry = new ResourceSetFolderActivityEntry();
		public static album: ResourceSetFolderAlbum = new ResourceSetFolderAlbum();
		public static artist: ResourceSetFolderArtist = new ResourceSetFolderArtist();
		public static song: ResourceSetFolderSong = new ResourceSetFolderSong();

		public static artistTypeNames = "artistTypeNames";
		public static discTypeNames = "discTypeNames";
		public static songTypeNames = "songTypeNames";
		public static userGroupNames = "userGroupNames";
	}

}