import { readCookie } from '@/Helpers/FetchApiHelper';

export const getXsrfToken = async () => {
	await fetch('/api/antiforgery/token');

	const xsrfToken = readCookie(document.cookie, 'XSRF-TOKEN');

	return xsrfToken;
};

