import { AjaxHelper } from '@/Helpers/AjaxHelper';
import axios from 'axios';

export class HeaderNames {
	static readonly ContentType = 'Content-Type';
}

export class MediaTypes {
	static readonly APPLICATION_FORM_URLENCODED =
		'application/x-www-form-urlencoded';
	static readonly APPLICATION_JSON = 'application/json';
}

export interface ErrorResponse<T = any> {
	data: T;
	status: number;
	statusText: string;
	headers: any;
}

export interface HttpClientError<T = ErrorResponse> extends Error {
	response?: T;
}

export class HttpClient {
	delete = async <T>(url: string, config?: { headers?: any }): Promise<T> => {
		const response = await axios.delete<T>(url, config);
		return response.data;
	};

	get = async <T>(url: string, data?: any): Promise<T> => {
		const response = await axios.get<T>(url, {
			params: data,
			// HACK: This is required for advanced search filters.
			paramsSerializer: AjaxHelper.stringify,
		});
		return response.data;
	};

	post = async <T>(
		url: string,
		data?: any,
		config?: { headers?: any },
	): Promise<T> => {
		const response = await axios.post<T>(url, data, config);
		return response.data;
	};

	put = async <T>(url: string, data?: any): Promise<T> => {
		const response = await axios.put<T>(url, data);
		return response.data;
	};
}

export const httpClient = new HttpClient();
