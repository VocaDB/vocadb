import ActivityEntryContract from '@DataContracts/ActivityEntry/ActivityEntryContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import EntryEditEvent from '@Models/ActivityEntries/EntryEditEvent';
import EntryType from '@Models/EntryType';
import GlobalValues from '@Shared/GlobalValues';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import _ from 'lodash';
import { action, makeObservable, observable } from 'mobx';

enum ActivityEntrySortRule {
	CreateDateDescending = 'CreateDateDescending',

	CreateDate = 'CreateDate',
}

export default class ActivityEntryListStore {
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
	) {
		makeObservable(this);

		this.loadMore();
	}

	public loadMore = (): void => {
		const url = this.urlMapper.mapRelative('/api/activityEntries');
		const sortRule = this.sort;
		this.httpClient
			.get<PartialFindResultContract<ActivityEntryContract>>(url, {
				fields: 'Entry,ArchivedVersion',
				entryFields: 'AdditionalNames,MainPicture',
				lang: this.values.languagePreference,
				before:
					sortRule === ActivityEntrySortRule.CreateDateDescending &&
					this.lastEntryDate
						? this.lastEntryDate.toISOString()
						: null,
				since:
					sortRule === ActivityEntrySortRule.CreateDate && this.lastEntryDate
						? this.lastEntryDate.toISOString()
						: null,
				userId: this.userId,
				editEvent: this.additionsOnly ? EntryEditEvent.Created : null,
				entryType: this.entryType,
				sortRule: this.sort,
			})
			.then((result) => {
				const entries = result.items;

				if (!entries) return;

				this.entries = [...this.entries, ...entries];
				this.lastEntryDate = new Date(_.last(entries)!.createDate);
			});
	};

	@action private clear = (): void => {
		this.lastEntryDate = undefined;
		this.entries = [];
		this.loadMore();
	};
}
