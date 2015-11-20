
module vdb.helpers {
	
	export class AjaxHelper {
		
		public static createUrl = (params: { [key: string]: string[] | number[]; }) => {
			
			if (!params)
				return null;

			var par = [];

			_.forOwn(params, (val, key) => {
				par.push(key + "=" + _.map(<string[]>val, v => encodeURIComponent(v)).join("&" + key + "="));
			});

			var query = par.join("&");
			return query;

		}

		public static deleteJSON_Url = (url: string, dataParamName: string, data: any[], success?: any) => {

			var dataParam = "?" + dataParamName + "=" + data.join("&" + dataParamName + "=");
			$.ajax(url + dataParam, { type: 'DELETE', success: success });

		}

		// Issues a PUT request with JSON-formatted body.
		public static putJSON = (url: string, data?: any, success?: any) => {
		
			$.ajax(url, {
				type: 'PUT',
				contentType: 'application/json; charset=utf-8',
				success: success,
				data: JSON.stringify(data)
			});

		}

		// Put JSON, including a parameter with one or more values in the URL (instead of body).
		// For example ?tag=vocarock&tag=metal
		public static putJSON_Url = (url: string, dataParamName: string, data: any[], success?: any) => {

			var dataParam = "?" + dataParamName + "=" + data.join("&" + dataParamName + "=");
			$.ajax(url + dataParam, { type: 'PUT', success: success });

		}

		public static putJSON_UrlParams = (url: string, params: { [key: string]: string[] | number[]; }, success?: any) => {

			var dataParam = "?" + AjaxHelper.createUrl(params);
			$.ajax(url + dataParam, { type: 'PUT', success: success });

		}
	}

} 