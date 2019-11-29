/// <reference path="../../Shared/UrlMapper.ts" />
/// <reference path="../../Repositories/EntryReportRepository.ts" />

//module vdb.tests.testSupport {

    export class FakeEntryReportRepository extends vdb.repositories.EntryReportRepository {

        public entryReportCount: number;

        constructor() {

            super(new UrlMapper(""));

            this.getNewReportCount = (callback) => {
                if (callback)
                    callback(this.entryReportCount);
            };

        }

    }

//}