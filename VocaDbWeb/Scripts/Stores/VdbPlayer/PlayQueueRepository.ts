import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { PVService } from '@/Models/PVs/PVService';
import { AlbumOptionalField } from '@/Repositories/AlbumRepository';
import { ReleaseEventOptionalField } from '@/Repositories/ReleaseEventRepository';
import { SongListGetSongsQueryParams } from '@/Repositories/SongListRepository';
import {
	SongGetListQueryParams,
	SongOptionalField,
} from '@/Repositories/SongRepository';
import { UserGetRatedSongsListQueryParams } from '@/Repositories/UserRepository';
import { PlayQueueSongContract } from '@/Stores/VdbPlayer/PlayQueueStore';

export type PlayQueueRepositoryQueryParams =
	| UserGetRatedSongsListQueryParams
	| SongListGetSongsQueryParams
	| SongGetListQueryParams;

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
