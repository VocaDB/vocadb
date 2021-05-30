import EntryReportRepository from '@Repositories/EntryReportRepository';
import HttpClient from '@Shared/HttpClient';

import FakePromise from './FakePromise';

export default class FakeEntryReportRepository extends EntryReportRepository {
  public entryReportCount!: number;

  constructor() {
    super(new HttpClient());

    this.getNewReportCount = (): Promise<number> => {
      return FakePromise.resolve(this.entryReportCount);
    };
  }
}
