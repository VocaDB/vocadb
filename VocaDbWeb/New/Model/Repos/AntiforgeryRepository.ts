import { apiFetch, readCookie } from '@/Helpers/FetchApiHelper';

export const getXsrfToken = async () => {
	// TODO: Convert this back to apiFetch, once we use the correct domain
	await apiFetch('/api/antiforgery/token');

	const xsrfToken = readCookie(document.cookie, 'XSRF-TOKEN');

	return xsrfToken;
};

