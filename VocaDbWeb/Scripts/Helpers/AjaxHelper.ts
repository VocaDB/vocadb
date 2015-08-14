
module vdb.helpers {
	
	export class AjaxHelper {
		
		public static deleteJSON_Url = (url: string, dataParamName: string, data: any[], success?: any) => {

			var dataParam = "?" + dataParamName + "=" + data.join("&" + dataParamName + "=");
			$.ajax(url + dataParam, { type: 'DELETE', success: success });

		}

		public static putJSON = (url: string, data?: any, success?: any) => {
		
			$.ajax(url, { type: 'PUT', success: success, data: data });

		}

		public static putJSON_Url = (url: string, dataParamName: string, data: any[], success?: any) => {

			var dataParam = "?" + dataParamName + "=" + data.join("&" + dataParamName + "=");
			$.ajax(url + dataParam, { type: 'PUT', success: success });

		}

	}

} 