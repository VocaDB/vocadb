import { IncomingMessage } from 'http';

const readCookie = (cookieHeader: string | undefined, name: string): string | null => {
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

export const apiFetch = (path: string, req?: IncomingMessage): Promise<Response> => {
	const authCookie = readCookie(req?.headers.cookie, '.AspNetCore.Cookies');
	return fetch(
		process.env.NEXT_API_URL + path,
		authCookie
			? { credentials: 'include', headers: { Cookie: `.AspNetCore.Cookies=${authCookie}` } }
			: undefined
	);
};

