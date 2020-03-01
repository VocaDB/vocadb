import UrlMapper from '../Shared/UrlMapper';

//module vdb.repositories {

    export default class EntryReportRepository {

        public getNewReportCount = (callback: (count) => void) => {
            var url = this.urlMapper.mapRelative("/entryReports/newReportsCount");
            $.getJSON(url, null, callback);
        };

        constructor(private urlMapper: UrlMapper) {}

    }

//}