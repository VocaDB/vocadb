import { IncomingMessage } from 'http';

export const readCookie = (cookieHeader: string | undefined, name: string): string | null => {
	if (!cookieHeader) {
		return null;
	}
	var nameEQ = name + '=';
	var ca = cookieHeader.split(';');
	for (var i = 0; i < ca.length; i++) {
		var c = ca[i];
		while (c.charAt(0) == ' ') c = c.substring(1, c.length);
		if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
	}
	return null;
};

export function authApiGet<T>(path: string, options?: ApiFetchOptions): Promise<T> {
	return apiGet(path, { credentials: true, ...options });
}

export function apiGet<T>(path: string, options?: ApiFetchOptions): Promise<T> {
	return apiFetch(path, options).then((resp) => resp.json());
}

interface ApiFetchOptions {
	credentials?: boolean;
}

/**  If an endpoint returns JSON Data, apiGet should be used instead. */
export const apiFetch = async (path: string, options?: ApiFetchOptions): Promise<Response> => {
	const resp = await fetch(
		process.env.NEXT_PUBLIC_API_URL + path,
		options?.credentials ? { credentials: 'include' } : {}
	);

	if (!resp.ok) {
		return Promise.reject(resp);
	}

	return resp;
};

export const apiPost = async (
	path: string,
	data: any,
	token?: string | null
): Promise<Response> => {
	let headers: any = {
		'Content-Type': 'application/json',
	};

	if (token) {
		headers.requestVerificationToken = token;
	}

	const resp = await fetch(process.env.NEXT_PUBLIC_API_URL + path, {
		headers,
		body: JSON.stringify(data),
		method: 'POST',
		credentials: 'include',
	});

	if (!resp.ok) {
		return Promise.reject(resp);
	}

	return resp;
};

