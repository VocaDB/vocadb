import { IPRuleContract } from '../ViewModels/Admin/ManageIPRulesViewModel';
import UrlMapper from '../Shared/UrlMapper';

//module vdb.repositories {

    export default class AdminRepository {

        constructor(private urlMapper: UrlMapper) { }

		public addIpToBanList = (rule: IPRuleContract, callback: (result: boolean) => void) => {
			return $.post(this.urlMapper.mapRelative("/api/ip-rules"), rule, callback);
		}

        public checkSFS = (ip: string, callback: (html: string) => void) => {

            return $.get(this.urlMapper.mapRelative("/Admin/CheckSFS"), { ip: ip }, callback); 

        };

		public getTempBannedIps = (callback: (ips: string[]) => void) => {
            return $.getJSON(this.urlMapper.mapRelative("/api/admin/tempBannedIPs"), callback); 
		}

    }

//}