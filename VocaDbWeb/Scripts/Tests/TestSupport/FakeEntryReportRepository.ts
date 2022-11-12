import { EntryReportRepository } from '@/Repositories/EntryReportRepository';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import { FakePromise } from '@/Tests/TestSupport/FakePromise';

export class FakeEntryReportRepository extends EntryReportRepository {
	entryReportCount!: number;

	constructor() {
		super(new HttpClient(), new UrlMapper(''));

		// eslint-disable-next-line no-empty-pattern
		this.getNewReportCount = ({}: {}): Promise<number> => {
			return FakePromise.resolve(this.entryReportCount);
		};
	}
}
