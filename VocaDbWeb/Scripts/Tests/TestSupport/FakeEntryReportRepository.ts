import EntryReportRepository from '@Repositories/EntryReportRepository';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';

import FakePromise from './FakePromise';

export default class FakeEntryReportRepository extends EntryReportRepository {
	public entryReportCount!: number;

	public constructor() {
		super(new HttpClient(), new UrlMapper(''));

		// eslint-disable-next-line no-empty-pattern
		this.getNewReportCount = ({}: {}): Promise<number> => {
			return FakePromise.resolve(this.entryReportCount);
		};
	}
}
