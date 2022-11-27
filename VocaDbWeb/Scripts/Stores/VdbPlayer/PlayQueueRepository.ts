import { PVContract } from '@/DataContracts/PVs/PVContract';
import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { EntryStatus } from '@/Models/EntryStatus';
import { EntryType } from '@/Models/EntryType';
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

interface PlayQueueCommonEntryContract {
	id: number;
	name: string;
	status: EntryStatus;
	additionalNames: string;
	urlThumb: string;
	pvs: PVContract[];
	artistIds: number[];
	tagIds: number[];
}

export interface PlayQueueAlbumContract extends PlayQueueCommonEntryContract {
	entryType: EntryType.Album;
	artistString: string;
}

export interface PlayQueueReleaseEventContract
	extends PlayQueueCommonEntryContract {
	entryType: EntryType.ReleaseEvent;
}

export interface PlayQueueSongContract extends PlayQueueCommonEntryContract {
	entryType: EntryType.Song;
	artistString: string;
	songType: SongType;
}

export interface PlayQueuePVContract extends PlayQueueCommonEntryContract {
	entryType: EntryType.PV;
}

export type PlayQueueEntryContract =
	| PlayQueueAlbumContract
	| PlayQueueReleaseEventContract
	| PlayQueueSongContract
	| PlayQueuePVContract;

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
	static readonly albumOptionalFields = [
		AlbumOptionalField.AdditionalNames,
		AlbumOptionalField.Artists,
		AlbumOptionalField.MainPicture,
		AlbumOptionalField.PVs,
		AlbumOptionalField.Tags,
		AlbumOptionalField.Tracks,
	];

	static readonly eventOptionalFields = [
		ReleaseEventOptionalField.AdditionalNames,
		ReleaseEventOptionalField.Artists,
		ReleaseEventOptionalField.MainPicture,
		ReleaseEventOptionalField.PVs,
		ReleaseEventOptionalField.Tags,
	];

	static readonly songOptionalFields = [
		SongOptionalField.AdditionalNames,
		SongOptionalField.Artists,
		SongOptionalField.MainPicture,
		SongOptionalField.PVs,
		SongOptionalField.Tags,
	];

	abstract getSongs({
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
	constructor(
		private readonly songListRepo: SongListRepository,
		private readonly songRepo: SongRepository,
		private readonly userRepo: UserRepository,
	) {}

	create = (
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
