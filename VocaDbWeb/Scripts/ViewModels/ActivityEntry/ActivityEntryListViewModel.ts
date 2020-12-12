import ActivityEntryContract from '../../DataContracts/ActivityEntry/ActivityEntryContract';
import ArchivedVersionContract from '../../DataContracts/Versioning/ArchivedVersionContract';
import EntryContract from '../../DataContracts/EntryContract';
import EntryEditEvent from '../../Models/ActivityEntries/EntryEditEvent';
import EntryType from '../../Models/EntryType';
import EntryUrlMapper from '../../Shared/EntryUrlMapper';
import PartialFindResultContract from '../../DataContracts/PartialFindResultContract';
import ResourceRepository from '../../Repositories/ResourceRepository';
import ResourcesManager from '../../Models/ResourcesManager';
import { ResourceSetNames } from '../../Models/ResourcesManager';
import UrlMapper from '../../Shared/UrlMapper';

	enum ActivityEntrySortRule {

		CreateDateDescending,

		CreateDate,

	}

	export default class ActivityEntryListViewModel {
		
		constructor(private urlMapper: UrlMapper,
			resourceRepo: ResourceRepository,
			private languageSelection: string,
			cultureCode: string,
			private userId?: number,
			additionsOnly?: boolean) {

			this.additionsOnly = ko.observable(additionsOnly ?? false);
			this.entryType = ko.observable();
			this.sort = ko.observable("CreateDateDescending");

			this.additionsOnly.subscribe(this.clear);
			this.entryType.subscribe(this.clear);
			this.sort.subscribe(this.clear);

			this.resources = new ResourcesManager(resourceRepo, cultureCode);
			this.resources.loadResources(null, ResourceSetNames.artistTypeNames, ResourceSetNames.discTypeNames, ResourceSetNames.songTypeNames,
				ResourceSetNames.userGroupNames, ResourceSetNames.activityEntry.activityFeedEventNames, ResourceSetNames.album.albumEditableFieldNames, ResourceSetNames.artist.artistEditableFieldNames,
				ResourceSetNames.releaseEvent.releaseEventEditableFieldNames,
				ResourceSetNames.song.songEditableFieldNames, ResourceSetNames.songList.songListEditableFieldNames, ResourceSetNames.songList.songListFeaturedCategoryNames,
				ResourceSetNames.tag.tagEditableFieldNames, "activityEntrySortRuleNames");

			this.sortName = ko.computed(() => {
				return this.resources.resources().activityEntrySortRuleNames != null ? this.resources.resources().activityEntrySortRuleNames[this.sort()] : "";
			});

		}

		public additionsOnly: KnockoutObservable<boolean>;

		private clear = () => {
			this.lastEntryDate = null;
			this.entries([]);
			this.loadMore();
		}

		public entries = ko.observableArray<ActivityEntryContract>([]);

		public entryType: KnockoutObservable<EntryType>;

		public getActivityFeedEventName = (activityEntry: ActivityEntryContract) => {
			
			var activityFeedEventNames = this.resources.resources().activityEntry_activityFeedEventNames;

			if (activityFeedEventNames[activityEntry.editEvent + activityEntry.entry.entryType]) {
				return activityFeedEventNames[activityEntry.editEvent + activityEntry.entry.entryType];
			} else if (activityEntry.editEvent === EntryEditEvent[EntryEditEvent.Created]) {
				return activityFeedEventNames["CreatedNew"].replace("{0}", activityFeedEventNames["Entry" + activityEntry.entry.entryType]);
			} else {
				return activityFeedEventNames["Updated"].replace("{0}", activityFeedEventNames["Entry" + activityEntry.entry.entryType]);				
			}

		}

		public getChangedFieldNames = (entry: EntryContract, archivedVersion: ArchivedVersionContract) => {
			
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

		public getEntryTypeName = (entry: EntryContract) => {
			
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

		public getEntryUrl = (entry: EntryContract) => {
			return EntryUrlMapper.details_entry(entry, entry.urlSlug);
		}

		private lastEntryDate: Date;

		public loadMore = () => {
			
			var url = this.urlMapper.mapRelative("/api/activityEntries");
			var sortRule = ActivityEntrySortRule[this.sort()];
			$.getJSON(url, {
				fields: 'Entry,ArchivedVersion',
				entryFields: 'AdditionalNames,MainPicture',
				lang: this.languageSelection,
				before: sortRule === ActivityEntrySortRule.CreateDateDescending && this.lastEntryDate ? this.lastEntryDate.toISOString() : null,
				since: sortRule === ActivityEntrySortRule.CreateDate && this.lastEntryDate ? this.lastEntryDate.toISOString() : null,
				userId: this.userId,
				editEvent: this.additionsOnly() ? EntryEditEvent.Created : null,
				entryType: this.entryType(),
				sortRule: this.sort()
			}, (result: PartialFindResultContract<ActivityEntryContract>) => {

				var entries = result.items;

				if (!entries && entries.length > 0)
					return;

				ko.utils.arrayPushAll(this.entries, entries);
				this.lastEntryDate = new Date(_.last(entries).createDate);

			});

		}

		public resources: ResourcesManager;

		public sort: KnockoutObservable<string>;
		public sortName: KnockoutComputed<string>;

	}