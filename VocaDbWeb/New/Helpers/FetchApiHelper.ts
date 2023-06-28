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

export const apiFetch = async (path: string, req?: IncomingMessage): Promise<Response> => {
	const authCookie = readCookie(req?.headers.cookie, '.AspNetCore.Cookies');
	const resp = await fetch(
		process.env.NEXT_PUBLIC_API_URL + path,
		authCookie
			? { credentials: 'include', headers: { Cookie: `.AspNetCore.Cookies=${authCookie}` } }
			: {}
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

