import EntryReportRepository from '../../Repositories/EntryReportRepository';
import UrlMapper from '../../Shared/UrlMapper';

export default class FakeEntryReportRepository extends EntryReportRepository {
  public entryReportCount: number;

  constructor() {
    super(new UrlMapper(''));

    this.getNewReportCount = (callback) => {
      if (callback) callback(this.entryReportCount);
    };
  }
}
