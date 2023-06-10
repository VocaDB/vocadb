import { IncomingMessage } from 'http';

type IncomingRequest = IncomingMessage & {
	cookies: Partial<{
		[key: string]: string;
	}>;
};

export const apiFetch = (path: string): Promise<Response> => {
	return fetch(process.env.API_URL + path);
};

