import EntryContract from '@DataContracts/EntryContract';
import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import functions from '@Shared/GlobalFunctions';
import HttpClient from '@Shared/HttpClient';

// Repository for finding base class of common entry types.
// Corresponds to the EntryApiController.
export default class EntryRepository {
	// Maps a relative URL to an absolute one.
	private mapUrl = (relative: string): string => {
		return functions.mergeUrls(
			functions.mergeUrls(this.baseUrl, '/api/entries'),
			relative,
		);
	};

	public constructor(
		private readonly httpClient: HttpClient,
		private baseUrl: string,
	) {}

	public getList = ({
		paging,
		lang,
		query,
		tags,
		childTags,
		fields,
		status,
	}: {
		paging: PagingProperties;
		lang: ContentLanguagePreference;
		query: string;
		tags: number[];
		childTags: boolean;
		fields: string;
		status?: string;
	}): Promise<PartialFindResultContract<EntryContract>> => {
		var url = this.mapUrl('');
		var data = {
			start: paging.start,
			getTotalCount: paging.getTotalCount,
			maxResults: paging.maxEntries,
			query: query,
			fields: fields,
			lang: lang,
			nameMatchMode: 'Auto',
			tagId: tags,
			childTags: childTags,
			status: status,
		};

		return this.httpClient.get<PartialFindResultContract<EntryContract>>(
			url,
			data,
		);
	};
}
