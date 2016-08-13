/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../Shared/UrlMapper.ts" />

module vdb.repositories {

    export class AdminRepository {

        constructor(private urlMapper: vdb.UrlMapper) { }

		public addIpToBanList = (rule: vdb.viewModels.IPRuleContract, callback: (result: boolean) => void) => {
            return $.post(this.urlMapper.mapRelative("/api/admin/permBannedIPs"), rule, callback);
		}

        public checkSFS = (ip: string, callback: (html: string) => void) => {

            return $.get(this.urlMapper.mapRelative("/Admin/CheckSFS"), { ip: ip }, callback); 

        };

		public getTempBannedIps = (callback: (ips: string[]) => void) => {
            return $.getJSON(this.urlMapper.mapRelative("/api/admin/tempBannedIPs"), callback); 
		}

    }

}