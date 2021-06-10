import UrlMapper from './UrlMapper';

export default class UrlMapperFactory {
	public createUrlMapper = (baseUrl: string): UrlMapper =>
		new UrlMapper(baseUrl);
}
