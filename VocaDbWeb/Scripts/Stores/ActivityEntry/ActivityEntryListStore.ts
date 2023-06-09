import { ActivityEntryContract } from '@/DataContracts/ActivityEntry/ActivityEntryContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { EntryEditEvent } from '@/Models/ActivityEntries/EntryEditEvent';
import { EntryType } from '@/Models/EntryType';
import { GlobalValues } from '@/Shared/GlobalValues';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import {
	includesAny,
	LocationStateStore,
	StateChangeEvent,
} from '@/route-sphere';
import Ajv from 'ajv';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

import schema from './ActivityEntryListRouteParams.schema.json';

export enum ActivityEntrySortRule {
	CreateDateDescending = 'CreateDateDescending',
	CreateDate = 'CreateDate',
}

interface ActivityEntryListRouteParams {
	entryEditEvent?: EntryEditEvent;
	entryType?: EntryType;
	sort?: ActivityEntrySortRule;
}

const clearResultsByQueryKeys: (keyof ActivityEntryListRouteParams)[] = [
	'entryEditEvent',
	'entryType',
	'sort',
];

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const validate = ajv.compile<ActivityEntryListRouteParams>(schema);

export class ActivityEntryListStore
	implements LocationStateStore<ActivityEntryListRouteParams> {
	@observable entries: ActivityEntryContract[] = [];
	@observable entryEditEvent?: EntryEditEvent;
	@observable entryType = EntryType.Undefined;
	private lastEntryDate?: Date;
	@observable sort = ActivityEntrySortRule.CreateDateDescending;
	@observable userId?: number;

	constructor(
		private readonly values: GlobalValues,
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
		userId?: number,
	) {
		makeObservable(this);

		this.userId = userId;
	}

	@computed.struct get locationState(): ActivityEntryListRouteParams {
		return {
			entryEditEvent: this.entryEditEvent,
			entryType: this.entryType,
			sort: this.sort,
		};
	}
	set locationState(value: ActivityEntryListRouteParams) {
		this.entryEditEvent = value.entryEditEvent;
		this.entryType = value.entryType ?? EntryType.Undefined;
		this.sort = value.sort ?? ActivityEntrySortRule.CreateDateDescending;
	}

	validateLocationState(
		locationState: any,
	): locationState is ActivityEntryListRouteParams {
		return validate(locationState);
	}

	loadMore = async (): Promise<void> => {
		const result = await this.httpClient.get<
			PartialFindResultContract<ActivityEntryContract>
		>(this.urlMapper.mapRelative('/api/activityEntries'), {
			fields: 'Entry,ArchivedVersion',
			entryFields: 'AdditionalNames,MainPicture',
			lang: this.values.languagePreference,
			before:
				this.sort === ActivityEntrySortRule.CreateDateDescending &&
				this.lastEntryDate
					? this.lastEntryDate.toISOString()
					: undefined,
			since:
				this.sort === ActivityEntrySortRule.CreateDate && this.lastEntryDate
					? this.lastEntryDate.toISOString()
					: undefined,
			userId: this.userId,
			editEvent: this.entryEditEvent,
			entryType: this.entryType,
			sortRule: this.sort,
		});

		const entries = result.items;

		if (!entries) return;

		runInAction(() => {
			this.entries.push(...entries);
			this.lastEntryDate = new Date(entries.last()!.createDate);
		});
	};

	@action private clear = (): Promise<void> => {
		this.lastEntryDate = undefined;
		this.entries = [];
		return this.loadMore();
	};

	private pauseNotifications = false;

	updateResults = async (clearResults: boolean): Promise<void> => {
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;

		await this.clear();

		this.pauseNotifications = false;
	};

	onLocationStateChange = (
		event: StateChangeEvent<ActivityEntryListRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		this.updateResults(clearResults);
	};
}
