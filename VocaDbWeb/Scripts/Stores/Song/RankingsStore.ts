import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import { TagUsageForApiContract } from '@/DataContracts/Tag/TagUsageForApiContract';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { PVServiceIcons } from '@/Models/PVServiceIcons';
import { SongRepository } from '@/Repositories/SongRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import { ISongSearchItem } from '@/Stores/Search/SongSearchStore';
import { SongWithPreviewStore } from '@/Stores/Song/SongWithPreviewStore';
import {
	includesAny,
	StateChangeEvent,
	LocationStateStore,
} from '@/route-sphere';
import Ajv from 'ajv';
import { computed, makeObservable, observable, runInAction } from 'mobx';

import schema from './RankingsRouteParams.schema.json';

interface RankingsRouteParams {
	dateFilterType?: string;
	durationHours?: number;
	vocalistSelection?: string;
}

const clearResultsByQueryKeys: (keyof RankingsRouteParams)[] = [
	'dateFilterType',
	'durationHours',
	'vocalistSelection',
];

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const validate = ajv.compile<RankingsRouteParams>(schema);

export class RankingsStore implements LocationStateStore<RankingsRouteParams> {
	@observable dateFilterType = 'CreateDate' /* TODO: enum */;
	@observable durationHours?: number;
	private readonly pvServiceIcons: PVServiceIcons;
	@observable songs: ISongSearchItem[] = [];
	@observable vocalistSelection?: string;

	constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
		private readonly songRepo: SongRepository,
		private readonly userRepo: UserRepository,
		private readonly languagePreference: ContentLanguagePreference,
	) {
		makeObservable(this);

		this.pvServiceIcons = new PVServiceIcons(urlMapper);
	}

	getPVServiceIcons = (
		services: string,
	): { service: string; url: string }[] => {
		return this.pvServiceIcons.getIconUrls(services);
	};

	private getSongs = (): Promise<void> => {
		return this.httpClient
			.get<SongApiContract[]>(
				this.urlMapper.mapRelative('/api/songs/top-rated'),
				{
					durationHours: this.durationHours,
					fields: 'AdditionalNames,MainPicture,Tags' /* TODO: enum */,
					vocalist: this.vocalistSelection,
					filterBy: this.dateFilterType,
					languagePreference: this.languagePreference,
				},
			)
			.then((songs) => {
				for (const song of songs as any[]) {
					if (song.pvServices && song.pvServices !== 'Nothing') {
						song.previewStore = new SongWithPreviewStore(
							this.songRepo,
							this.userRepo,
							song.id,
							song.pvServices,
						);
						// TODO: showThankYouForRatingMessage
					} else {
						song.previewStore = undefined;
					}
				}

				runInAction(() => {
					this.songs = songs;
				});
			});
	};

	getTagUrl = (tag: TagUsageForApiContract): string => {
		return EntryUrlMapper.details_tag(tag.tag.id, tag.tag.urlSlug);
	};

	@computed.struct get locationState(): RankingsRouteParams {
		return {
			dateFilterType: this.dateFilterType,
			durationHours: this.durationHours,
			vocalistSelection: this.vocalistSelection,
		};
	}
	set locationState(value: RankingsRouteParams) {
		this.dateFilterType = value.dateFilterType ?? 'CreateDate';
		this.durationHours = value.durationHours;
		this.vocalistSelection = value.vocalistSelection;
	}

	validateLocationState = (data: any): data is RankingsRouteParams => {
		return validate(data);
	};

	private pauseNotifications = false;

	updateResults = async (clearResults: boolean): Promise<void> => {
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;

		await this.getSongs();

		this.pauseNotifications = false;
	};

	onLocationStateChange = (
		event: StateChangeEvent<RankingsRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		this.updateResults(clearResults);
	};
}
