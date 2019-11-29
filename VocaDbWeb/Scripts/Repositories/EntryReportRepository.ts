/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../Shared/UrlMapper.ts" />

//module vdb.repositories {

    export class EntryReportRepository {

        public getNewReportCount = (callback: (count) => void) => {
            var url = this.urlMapper.mapRelative("/entryReports/newReportsCount");
            $.getJSON(url, null, callback);
        };

        constructor(private urlMapper: vdb.UrlMapper) {}

    }

//}