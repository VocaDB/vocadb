import { PVContract } from '@/DataContracts/PVs/PVContract';
import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { EntryStatus } from '@/Models/EntryStatus';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { PVService } from '@/Models/PVs/PVService';
import { SongType } from '@/Models/Songs/SongType';
import { AlbumOptionalField } from '@/Repositories/AlbumRepository';
import { ReleaseEventOptionalField } from '@/Repositories/ReleaseEventRepository';
import {
	SongListGetSongsQueryParams,
	SongListRepository,
} from '@/Repositories/SongListRepository';
import {
	SongGetListQueryParams,
	SongOptionalField,
	SongRepository,
} from '@/Repositories/SongRepository';
import {
	UserGetRatedSongsListQueryParams,
	UserRepository,
} from '@/Repositories/UserRepository';
import { PlayQueueRepositoryForRatedSongsAdapter } from '@/Stores/VdbPlayer/PlayQueueRepositoryForRatedSongsAdapter';
import { PlayQueueRepositoryForSongListAdapter } from '@/Stores/VdbPlayer/PlayQueueRepositoryForSongListAdapter';
import { PlayQueueRepositoryForSongsAdapter } from '@/Stores/VdbPlayer/PlayQueueRepositoryForSongsAdapter';

export type PlayQueueRepositoryQueryParams =
	| UserGetRatedSongsListQueryParams
	| SongListGetSongsQueryParams
	| SongGetListQueryParams;

// TODO: Remove.
export enum EntryType {
	Album = 'Album',
	ReleaseEvent = 'ReleaseEvent',
	Song = 'Song',
}

export interface PlayQueueAlbumContract {
	entryType: EntryType.Album;
	id: number;
	name: string;
	status: EntryStatus;
	additionalNames: string;
	urlThumb: string;
	pvs: PVContract[];
	artistString: string;
}

export interface PlayQueueReleaseEventContract {
	entryType: EntryType.ReleaseEvent;
	id: number;
	name: string;
	status: EntryStatus;
	additionalNames: string;
	urlThumb: string;
	pvs: PVContract[];
}

export interface PlayQueueSongContract {
	entryType: EntryType.Song;
	id: number;
	name: string;
	status: EntryStatus;
	additionalNames: string;
	urlThumb: string;
	pvs: PVContract[];
	artistString: string;
	songType: SongType;
}

export type PlayQueueEntryContract =
	| PlayQueueAlbumContract
	| PlayQueueReleaseEventContract
	| PlayQueueSongContract;

export interface PlayQueueItemContract {
	entry: PlayQueueEntryContract;
	pvId: number;
}

export enum PlayQueueRepositoryType {
	RatedSongs = 'RatedSongs',
	SongList = 'SongList',
	Songs = 'Songs',
}

export abstract class PlayQueueRepository<
	TQueryParams extends PlayQueueRepositoryQueryParams
> {
	public static readonly albumOptionalFields = [
		AlbumOptionalField.AdditionalNames,
		AlbumOptionalField.MainPicture,
		AlbumOptionalField.PVs,
		AlbumOptionalField.Tracks,
	];

	public static readonly eventOptionalFields = [
		ReleaseEventOptionalField.AdditionalNames,
		ReleaseEventOptionalField.MainPicture,
		ReleaseEventOptionalField.PVs,
	];

	public static readonly songOptionalFields = [
		SongOptionalField.AdditionalNames,
		SongOptionalField.MainPicture,
		SongOptionalField.PVs,
	];

	public abstract getSongs({
		lang,
		pagingProps,
		pvServices,
		queryParams,
	}: {
		lang: ContentLanguagePreference;
		pagingProps: PagingProperties;
		pvServices?: PVService[];
		queryParams: TQueryParams;
	}): Promise<PartialFindResultContract<PlayQueueSongContract>>;
}

export class PlayQueueRepositoryFactory {
	public constructor(
		private readonly songListRepo: SongListRepository,
		private readonly songRepo: SongRepository,
		private readonly userRepo: UserRepository,
	) {}

	public create = (
		type: PlayQueueRepositoryType,
	): PlayQueueRepository<PlayQueueRepositoryQueryParams> => {
		switch (type) {
			case PlayQueueRepositoryType.RatedSongs:
				return new PlayQueueRepositoryForRatedSongsAdapter(this.userRepo);

			case PlayQueueRepositoryType.SongList:
				return new PlayQueueRepositoryForSongListAdapter(this.songListRepo);

			case PlayQueueRepositoryType.Songs:
				return new PlayQueueRepositoryForSongsAdapter(this.songRepo);
		}
	};
}
