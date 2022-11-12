import { SongListForEditContract } from '@/DataContracts/Song/SongListForEditContract';
import { ImportedSongInListContract } from '@/DataContracts/SongList/ImportedSongInListContract';
import { ImportedSongListContract } from '@/DataContracts/SongList/ImportedSongListContract';
import { PartialImportedSongs } from '@/DataContracts/SongList/PartialImportedSongs';
import { EntryStatus } from '@/Models/EntryStatus';
import { SongListFeaturedCategory } from '@/Models/SongLists/SongListFeaturedCategory';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

export class ImportSongListStore {
	@observable description = '';
	@observable items: ImportedSongInListContract[] = [];
	@observable name = '';
	@observable nextPageToken?: string;
	@observable onlyRanked = false;
	@observable parsed = false;
	@observable submitting = false;
	@observable totalSongs?: number;
	@observable url = '';

	constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
	) {
		makeObservable(this);
	}

	@computed get missingSongs(): boolean {
		return this.items.some((i) => i.matchedSong == null);
	}

	@computed get hasMore(): boolean {
		return this.nextPageToken != null;
	}

	loadMore = async (): Promise<void> => {
		const result = await this.httpClient.get<PartialImportedSongs>(
			this.urlMapper.mapRelative('/api/songLists/import-songs'),
			{
				url: this.url,
				pageToken: this.nextPageToken,
				parseAll: !this.onlyRanked,
			},
		);

		runInAction(() => {
			this.nextPageToken = result.nextPageToken;
			this.items.push(...result.items);
		});
	};

	parse = async (): Promise<void> => {
		const songList = await this.httpClient.get<ImportedSongListContract>(
			this.urlMapper.mapRelative('/api/songLists/import'),
			{
				url: this.url,
				parseAll: !this.onlyRanked,
			},
		);

		runInAction(() => {
			this.name = songList.name;
			this.description = songList.description;
			this.nextPageToken = songList.songs.nextPageToken;
			this.items = songList.songs.items;
			this.totalSongs = songList.songs.totalCount;
			this.parsed = true;
		});
	};

	@action submit = async (): Promise<number> => {
		try {
			this.submitting = true;

			const songs = this.items
				.filter((i) => i.matchedSong != null)
				.map((i: ImportedSongInListContract, index) => ({
					order: index + 1,
					notes: '',
					song: i.matchedSong,
					songInListId: null!,
				}));

			const contract: SongListForEditContract = {
				id: null!,
				author: null!,
				name: this.name,
				description: this.description,
				featuredCategory: SongListFeaturedCategory.Nothing,
				status: EntryStatus[EntryStatus.Finished],
				songLinks: songs,
			};

			const listId = await this.httpClient.post<number>(
				this.urlMapper.mapRelative('/api/songLists'),
				contract,
			);

			runInAction(() => {
				this.description = '';
				this.items = [];
				this.name = '';
				this.nextPageToken = undefined;
				this.onlyRanked = false;
				this.parsed = false;
				this.totalSongs = undefined;
				this.url = '';
			});

			return listId;
		} finally {
			runInAction(() => {
				this.submitting = false;
			});
		}
	};
}
