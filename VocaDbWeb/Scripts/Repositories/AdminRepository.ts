/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../Shared/UrlMapper.ts" />

module vdb.repositories {

    export class AdminRepository {

        public checkSFS = (ip: string, callback: (html: string) => void) => {

            return $.get(this.urlMapper.mapRelative("/Admin/CheckSFS"), { ip: ip }, callback); 

        };

        constructor(private urlMapper: vdb.UrlMapper) {}

    }

}