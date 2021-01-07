
interface JQueryStatic {

	postJSON(url: string, success?: (data: any, textStatus: string, jqXHR: JQueryXHR) => any, dataType?: string): JQueryXHR;

	postJSON(url: string, data?: Object|string, success?: (data: any, textStatus: string, jqXHR: JQueryXHR) => any, dataType?: string): JQueryXHR;

}
