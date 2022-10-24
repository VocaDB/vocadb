import { CommentContract } from '@/DataContracts/CommentContract';
import { EntryWithCommentsContract } from '@/DataContracts/EntryWithCommentsContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { UserBaseContract } from '@/DataContracts/User/UserBaseContract';
import { EntryType } from '@/Models/EntryType';
import { UserRepository } from '@/Repositories/UserRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import { BasicEntryLinkStore } from '@/Stores/BasicEntryLinkStore';
import {
	includesAny,
	StateChangeEvent,
	LocationStateStore,
} from '@vocadb/route-sphere';
import Ajv, { JSONSchemaType } from 'ajv';
import { map } from 'lodash-es';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

export enum CommentSortRule {
	CreateDateDescending = 'CreateDateDescending',
	CreateDate = 'CreateDate',
}

enum CommentOptionalField {
	Entry = 'Entry',
}

enum EntryOptionalField {
	AdditionalNames = 'AdditionalNames',
	MainPicture = 'MainPicture',
}

interface CommentListRouteParams {
	entryType?: string /* TODO: enum */;
	sort?: CommentSortRule;
	userId?: number;
}

const clearResultsByQueryKeys: (keyof CommentListRouteParams)[] = [
	'entryType',
	'sort',
	'userId',
];

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const schema: JSONSchemaType<CommentListRouteParams> = require('./CommentListRouteParams.schema');
const validate = ajv.compile(schema);

export class CommentListStore
	implements LocationStateStore<CommentListRouteParams> {
	@observable public entries: EntryWithCommentsContract[] = [];
	@observable public entryType = EntryType[EntryType.Undefined];
	@observable public lastEntryDate?: Date;
	@observable public sort = CommentSortRule.CreateDateDescending;
	public readonly user: BasicEntryLinkStore<UserBaseContract>;

	public constructor(
		private readonly values: GlobalValues,
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
		userRepo: UserRepository,
	) {
		makeObservable(this);

		this.user = new BasicEntryLinkStore<UserBaseContract>((entryId) =>
			userRepo.getOne({ id: entryId }),
		);
	}

	@computed.struct public get locationState(): CommentListRouteParams {
		return {
			entryType: this.entryType,
			sort: this.sort,
			userId: this.user.id,
		};
	}
	public set locationState(value: CommentListRouteParams) {
		this.entryType = value.entryType ?? EntryType[EntryType.Undefined];
		this.sort = value.sort ?? CommentSortRule.CreateDateDescending;
		this.user.id = value.userId;
	}

	public validateLocationState = (
		data: any,
	): data is CommentListRouteParams => {
		return validate(data);
	};

	public loadMore = async (): Promise<void> => {
		const result = await this.httpClient.get<
			PartialFindResultContract<CommentContract>
		>(this.urlMapper.mapRelative('/api/comments'), {
			fields: [CommentOptionalField.Entry].join(','),
			entryFields: [
				EntryOptionalField.AdditionalNames,
				EntryOptionalField.MainPicture,
			].join(','),
			lang: this.values.languagePreference,
			before:
				this.sort === CommentSortRule.CreateDateDescending && this.lastEntryDate
					? this.lastEntryDate.toISOString()
					: undefined,
			since:
				this.sort === CommentSortRule.CreateDate && this.lastEntryDate
					? this.lastEntryDate.toISOString()
					: undefined,
			userId: this.user.id,
			entryType: this.entryType,
			sortRule: this.sort,
		});

		const entries = map(
			result.items
				.map((comment) => ({
					globalEntryId: `${comment.entry?.entryType}.${comment.entry?.id}`,
					comment,
				}))
				.groupBy((x) => x.globalEntryId),
			(value) =>
				({
					comments: value.map(({ comment }) => comment),
					entry: value[0].comment.entry,
				} as EntryWithCommentsContract),
		);

		const lastEntry = entries.last()?.comments.last();

		runInAction(() => {
			this.entries.push(...entries);
			this.lastEntryDate =
				lastEntry && lastEntry.created
					? new Date(lastEntry.created)
					: undefined;
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
		event: StateChangeEvent<CommentListRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		this.updateResults(clearResults);
	};
}
