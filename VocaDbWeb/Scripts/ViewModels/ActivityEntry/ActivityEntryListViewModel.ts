
module vdb.viewModels.activityEntry {
	
	import dc = dataContracts;
	import EntryType = models.EntryType;
	import rep = repositories;
	import resSets = models.ResourceSetNames;

	export class ActivityEntryListViewModel {
		
		constructor(private urlMapper: UrlMapper,
			resourceRepo: rep.ResourceRepository,
			private languageSelection: string,
			cultureCode: string) {
			
			this.resources = new models.ResourcesManager(resourceRepo, cultureCode);
			this.resources.loadResources(this.loadMore, resSets.artistTypeNames, resSets.discTypeNames, resSets.songTypeNames,
				resSets.userGroupNames, resSets.activityEntry.activityFeedEventNames, resSets.album.albumEditableFieldNames, resSets.artist.artistEditableFieldNames,
				resSets.song.songEditableFieldNames, resSets.songList.songListEditableFieldNames, resSets.songList.songListFeaturedCategoryNames,
				resSets.tag.tagEditableFieldNames);

		}

		public entries = ko.observableArray<dc.activityEntry.ActivityEntryContract>([]);

		public getActivityFeedEventName = (activityEntry: dc.activityEntry.ActivityEntryContract) => {
			
			var activityFeedEventNames = this.resources.resources().activityEntry_activityFeedEventNames;

			if (activityFeedEventNames[activityEntry.editEvent + activityEntry.entry.entryType]) {
				return activityFeedEventNames[activityEntry.editEvent + activityEntry.entry.entryType];
			} else if (activityEntry.editEvent === "Created") {
				return activityFeedEventNames["CreatedNew"].replace("{0}", activityFeedEventNames["Entry" + activityEntry.entry.entryType]);
			} else {
				return activityFeedEventNames["Updated"].replace("{0}", activityFeedEventNames["Entry" + activityEntry.entry.entryType]);				
			}

		}

		public getChangedFieldNames = (entry: dc.EntryContract, archivedVersion: dc.versioning.ArchivedVersionContract) => {
			
			if (archivedVersion == null || archivedVersion.changedFields == null)
				return null;

			var r = this.resources.resources();

			switch (EntryType[entry.entryType]) {
				case EntryType.Album:
					return _.map(archivedVersion.changedFields, f => r.album_albumEditableFieldNames[f]);

				case EntryType.Artist:
					return _.map(archivedVersion.changedFields, f => r.artist_artistEditableFieldNames[f]);

				case EntryType.Song:
					return _.map(archivedVersion.changedFields, f => r.song_songEditableFieldNames[f]);

				case EntryType.SongList:
					return _.map(archivedVersion.changedFields, f => r.songList_songListEditableFieldNames[f]);

				case EntryType.Tag:
					return _.map(archivedVersion.changedFields, f => r.tag_tagEditableFieldNames[f]);

				default:
					return archivedVersion.changedFields;			
			}


		}

		public getEntryTypeName = (entry: dc.EntryContract) => {
			
			switch (EntryType[entry.entryType]) {
				case EntryType.Album:
					return this.resources.resources().discTypeNames[entry.discType];
				
				case EntryType.Artist:
					return this.resources.resources().artistTypeNames[entry.artistType];

				case EntryType.Song:
					return this.resources.resources().songTypeNames[entry.songType];

				case EntryType.SongList:
					return this.resources.resources().songList_songListFeaturedCategoryNames[entry.songListFeaturedCategory];

				case EntryType.Tag:
					return entry.tagCategoryName;

				default:
					return null;
			}

		}

		private lastEntryDate: Date;

		public loadMore = () => {
			
			var url = this.urlMapper.mapRelative("/api/activityEntries");
			$.getJSON(url, {
				fields: 'Entry,ArchivedVersion',
				entryFields: 'MainPicture',
				lang: this.languageSelection,
				before: this.lastEntryDate ? this.lastEntryDate.toISOString() : null
			}, (entries: dc.activityEntry.ActivityEntryContract[]) => {

				if (!entries && entries.length > 0)
					return;

				ko.utils.arrayPushAll(this.entries, entries);
				this.lastEntryDate = new Date(_.last(entries).createDate);

			});

		}

		public resources: vdb.models.ResourcesManager;

	}

} 