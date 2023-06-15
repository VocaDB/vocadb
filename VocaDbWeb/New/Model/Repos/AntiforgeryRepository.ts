import { apiFetch, readCookie } from '@/Helpers/FetchApiHelper';

export const getXsrfToken = async () => {
	await apiFetch('/api/antiforgery/token');

	const xsrfToken = readCookie(document.cookie, 'XSRF-TOKEN');

	return xsrfToken;
};

