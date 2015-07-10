
module vdb.viewModels.songList {
	
	import dc = vdb.dataContracts;

	export class ImportSongListViewModel {
		
		constructor(private urlMapper: vdb.UrlMapper) {}

		public description = ko.observable("");

		public items = ko.observableArray<dc.songList.ImportedSongInListContract>([]);

		public loadMore = () => {
			
			$.getJSON(this.urlMapper.mapRelative('/api/songLists/import-songs'),
				{ url: this.url(), pageToken: this.nextPageToken(), parseAll: !this.onlyRanked() },
				(result: dc.songList.PartialImportedSongs) => {

				this.nextPageToken(result.nextPageToken);
				ko.utils.arrayPushAll(this.items, result.items);

			});

		}

		public missingSongs = ko.computed(() => _.some(this.items(), i => i.matchedSong == null));

		public name = ko.observable("");

		public nextPageToken = ko.observable<string>(null);

		public hasMore = ko.computed(() => this.nextPageToken() != null);

		public onlyRanked = ko.observable(false);

		public parse = () => {

			$.getJSON(this.urlMapper.mapRelative('/api/songLists/import'), { url: this.url(), parseAll: !this.onlyRanked() },(songList: dc.songList.ImportedSongListContract) => {

				this.name(songList.name);
				this.description(songList.description);
				this.nextPageToken(songList.songs.nextPageToken);
				this.items(songList.songs.items);
				this.parsed(true);

			});

		}

		public parsed = ko.observable(false);

		public submit = () => {

			var order = 1;
			var songs = _.chain(this.items()).filter(i => i.matchedSong != null).map((i: dc.songList.ImportedSongInListContract) => {
				return {
					order: order++,
					notes: '',
					song: i.matchedSong,
					songInListId: null
				};
			}).value();

			var contract: dc.songs.SongListForEditContract = {
				id: null,
				author: null,
				name: this.name(),
				description: this.description(),
				featuredCategory: 'Nothing',
				status: 'Finished',
				songLinks: songs
			};

			$.post(this.urlMapper.mapRelative('/api/songLists'), contract, (listId: number) => {
				window.location.href = vdb.utils.EntryUrlMapper.details('SongList', listId);
			}, 'json');

		}

		public url = ko.observable("");

		public wvrNumber = ko.observable(0);

	}

} 