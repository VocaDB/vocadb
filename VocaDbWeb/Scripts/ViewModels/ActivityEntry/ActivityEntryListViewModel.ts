
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
			this.resources.loadResources(null, resSets.artistTypeNames, resSets.discTypeNames, resSets.songTypeNames, resSets.userGroupNames);

			this.loadMore();

		}

		public entries = ko.observableArray<dc.activityEntry.ActivityEntryContract>([]);

		public getEntryTypeName = (entry: dc.EntryContract) => {
			
			switch (entry.entryType) {
				case EntryType[EntryType.Album]:
					return this.resources.resources().discTypeNames[entry.discType];
				
				case EntryType[EntryType.Artist]:
					return this.resources.resources().artistTypeNames[entry.artistType];

				case EntryType[EntryType.Song]:
					return this.resources.resources().songTypeNames[entry.songType];

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