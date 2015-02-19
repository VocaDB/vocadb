
module vdb.helpers {
	
	export class AjaxHelper {
		
		public static putJSON = (url: string, data: any, success?: () => void) => {
		
			$.ajax(url, { type: 'PUT', success: success, data: data });

		}

	}

} 