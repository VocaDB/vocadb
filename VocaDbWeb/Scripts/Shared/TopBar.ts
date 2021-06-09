import UserRepository from '@Repositories/UserRepository';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import $ from 'jquery';

import HttpClient from './HttpClient';

$(() => {
	$('#globalSearchTerm').autocomplete({
		source: (
			request: { term: string },
			response: (items: string[]) => void,
		) => {
			var urlMapper = new UrlMapper(vdb.values.baseAddress);
			var term: string = request.term;
			var entryType = $('#globalSearchObjectType').val();
			var endpoint: string = null!;

			switch (entryType) {
				case 'Undefined':
					endpoint = '/api/entries/names';
					break;
				case 'Album':
					endpoint = '/api/albums/names';
					break;
				case 'Artist':
					endpoint = '/api/artists/names';
					break;
				case 'ReleaseEvent':
					endpoint = '/api/releaseEvents/names';
					break;
				case 'Song':
					endpoint = '/api/songs/names';
					break;
				case 'SongList':
					endpoint = '/api/songLists/featured/names';
					break;
				case 'Tag':
					endpoint = '/api/tags/names';
					break;
				case 'User':
					endpoint = '/api/users/names';
			}

			if (!endpoint) return;

			$.getJSON(urlMapper.mapRelative(endpoint), { query: term }, response);
		},
		select: (event: Event, ui) => {
			$('#globalSearchTerm').val('"' + ui.item.value + '"');
			$('#globalSearchBox').submit();
		},
	});
});

export function setLanguagePreferenceCookie(
	languagePreference: string,
): boolean {
	const httpClient = new HttpClient();
	var userRepo = new UserRepository(
		httpClient,
		new UrlMapper(vdb.values.baseAddress),
	);
	userRepo
		.updateUserSetting(
			vdb.values.loggedUserId,
			'languagePreference',
			languagePreference,
		)
		.then(() => {
			window.location.reload();
		});

	return false;
}
