
//module vdb.viewModels.activityEntry {
	
	import cls = models;
	import dc = dataContracts;
	import EntryType = models.EntryType;
	import rep = repositories;
	import resSets = models.ResourceSetNames;

	export class ActivityEntryListViewModel {
		
		constructor(private urlMapper: UrlMapper,
			resourceRepo: rep.ResourceRepository,
			private languageSelection: string,
			cultureCode: string,
			private userId?: number,
			editEvent?: cls.activityEntries.EntryEditEvent) {
			
			this.editEvent = ko.observable(editEvent);
			this.editEvent.subscribe(this.clear);

			this.editEventFilter_all = ko.computed({
				read: () => this.editEvent() == null,
				write: (val: boolean) => this.editEvent(val ? null : cls.activityEntries.EntryEditEvent.Created)
			});

			this.resources = new models.ResourcesManager(resourceRepo, cultureCode);
			this.resources.loadResources(this.loadMore, resSets.artistTypeNames, resSets.discTypeNames, resSets.songTypeNames,
				resSets.userGroupNames, resSets.activityEntry.activityFeedEventNames, resSets.album.albumEditableFieldNames, resSets.artist.artistEditableFieldNames,
				resSets.releaseEvent.releaseEventEditableFieldNames,
				resSets.song.songEditableFieldNames, resSets.songList.songListEditableFieldNames, resSets.songList.songListFeaturedCategoryNames,
				resSets.tag.tagEditableFieldNames);


		}

		private clear = () => {
			this.lastEntryDate = null;
			this.entries([]);
			this.loadMore();
		}

		public entries = ko.observableArray<dc.activityEntry.ActivityEntryContract>([]);

		public editEvent: KnockoutObservable<cls.activityEntries.EntryEditEvent>;

		public editEventFilter_all: KnockoutComputed<boolean>;

		public getActivityFeedEventName = (activityEntry: dc.activityEntry.ActivityEntryContract) => {
			
			var activityFeedEventNames = this.resources.resources().activityEntry_activityFeedEventNames;

			if (activityFeedEventNames[activityEntry.editEvent + activityEntry.entry.entryType]) {
				return activityFeedEventNames[activityEntry.editEvent + activityEntry.entry.entryType];
			} else if (activityEntry.editEvent === cls.activityEntries.EntryEditEvent[cls.activityEntries.EntryEditEvent.Created]) {
				return activityFeedEventNames["CreatedNew"].replace("{0}", activityFeedEventNames["Entry" + activityEntry.entry.entryType]);
			} else {
				return activityFeedEventNames["Updated"].replace("{0}", activityFeedEventNames["Entry" + activityEntry.entry.entryType]);				
			}

		}

		public getChangedFieldNames = (entry: dc.EntryContract, archivedVersion: dc.versioning.ArchivedVersionContract) => {
			
			if (archivedVersion == null || archivedVersion.changedFields == null || archivedVersion.changedFields.length === 0)
				return null;

			var sets = this.resources.resources();
			var namesSet: { [key: string]: string; };

			switch (EntryType[entry.entryType]) {
				case EntryType.Album:
					namesSet = sets.album_albumEditableFieldNames;
					break;

				case EntryType.Artist:
					namesSet = sets.artist_artistEditableFieldNames;
					break;

				case EntryType.ReleaseEvent:
					namesSet = sets.releaseEvent_releaseEventEditableFieldNames;
					break;

				case EntryType.Song:
					namesSet = sets.song_songEditableFieldNames;
					break;

				case EntryType.SongList:
					namesSet = sets.songList_songListEditableFieldNames;
					break;

				case EntryType.Tag:
					namesSet = sets.tag_tagEditableFieldNames;
					break;

				default:
					return archivedVersion.changedFields.join(", ");
			}

			var names = _.map(archivedVersion.changedFields, f => namesSet[f]);
			return names.join(", ");

		}

		public getEntryTypeName = (entry: dc.EntryContract) => {
			
			var sets = this.resources.resources();

			switch (EntryType[entry.entryType]) {
				case EntryType.Album:
					return sets.discTypeNames[entry.discType];
				
				case EntryType.Artist:
					return sets.artistTypeNames[entry.artistType];

				case EntryType.Song:
					return sets.songTypeNames[entry.songType];

				case EntryType.SongList:
					return sets.songList_songListFeaturedCategoryNames[entry.songListFeaturedCategory];

				case EntryType.Tag:
					return entry.tagCategoryName;

				default:
					return null;
			}

		}

		public getEntryUrl = (entry: dc.EntryContract) => {
			return utils.EntryUrlMapper.details_entry(entry, entry.urlSlug);
		}

		private lastEntryDate: Date;

		public loadMore = () => {
			
			var url = this.urlMapper.mapRelative("/api/activityEntries");
			$.getJSON(url, {
				fields: 'Entry,ArchivedVersion',
				entryFields: 'AdditionalNames,MainPicture',
				lang: this.languageSelection,
				before: this.lastEntryDate ? this.lastEntryDate.toISOString() : null,
				userId: this.userId,
				editEvent: this.editEvent() ? cls.activityEntries.EntryEditEvent[this.editEvent()] : null
			}, (result: dc.PartialFindResultContract<dc.activityEntry.ActivityEntryContract>) => {

				var entries = result.items;

				if (!entries && entries.length > 0)
					return;

				ko.utils.arrayPushAll(this.entries, entries);
				this.lastEntryDate = new Date(_.last(entries).createDate);

			});

		}

		public resources: vdb.models.ResourcesManager;

	}

//} 