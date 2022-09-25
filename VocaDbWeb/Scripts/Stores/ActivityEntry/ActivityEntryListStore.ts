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
} from '@vocadb/route-sphere';
import Ajv, { JSONSchemaType } from 'ajv';
import _ from 'lodash';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

export enum ActivityEntrySortRule {
	CreateDateDescending = 'CreateDateDescending',
	CreateDate = 'CreateDate',
}

interface ActivityEntryListRouteParams {
	entryType?: string /* TODO: enum */;
	onlySubmissions?: boolean;
	sort?: ActivityEntrySortRule;
}

const clearResultsByQueryKeys: (keyof ActivityEntryListRouteParams)[] = [
	'entryType',
	'onlySubmissions',
	'sort',
];

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const schema: JSONSchemaType<ActivityEntryListRouteParams> = require('./ActivityEntryListRouteParams.schema');
const validate = ajv.compile(schema);

export class ActivityEntryListStore
	implements LocationStateStore<ActivityEntryListRouteParams> {
	@observable public additionsOnly = false;
	@observable public entries: ActivityEntryContract[] = [];
	@observable public entryType =
		EntryType[EntryType.Undefined]; /* TODO: enum */
	private lastEntryDate?: Date;
	@observable public sort = ActivityEntrySortRule.CreateDateDescending;
	@observable public userId?: number;

	public constructor(
		private readonly values: GlobalValues,
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
		userId?: number,
	) {
		makeObservable(this);

		this.userId = userId;
	}

	@computed.struct public get locationState(): ActivityEntryListRouteParams {
		return {
			entryType: this.entryType,
			onlySubmissions: this.additionsOnly,
			sort: this.sort,
		};
	}
	public set locationState(value: ActivityEntryListRouteParams) {
		this.entryType = value.entryType ?? EntryType[EntryType.Undefined];
		this.additionsOnly = value.onlySubmissions ?? false;
		this.sort = value.sort ?? ActivityEntrySortRule.CreateDateDescending;
	}

	public validateLocationState(
		locationState: any,
	): locationState is ActivityEntryListRouteParams {
		return validate(locationState);
	}

	public loadMore = async (): Promise<void> => {
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
			editEvent: this.additionsOnly ? EntryEditEvent.Created : undefined,
			entryType: this.entryType,
			sortRule: this.sort,
		});

		const entries = result.items;

		if (!entries) return;

		runInAction(() => {
			this.entries.push(...entries);
			this.lastEntryDate = new Date(_.last(entries)!.createDate);
		});
	};

	@action private clear = (): Promise<void> => {
		this.lastEntryDate = undefined;
		this.entries = [];
		return this.loadMore();
	};

	private pauseNotifications = false;

	public updateResults = async (clearResults: boolean): Promise<void> => {
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;

		await this.clear();

		this.pauseNotifications = false;
	};

	public onLocationStateChange = (
		event: StateChangeEvent<ActivityEntryListRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		this.updateResults(clearResults);
	};
}
