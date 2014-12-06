
$(() => {

	$("#globalSearchTerm").autocomplete({
		source: (request, response: (items: string[]) => void) => {

			var urlMapper = new vdb.UrlMapper(vdb.values.hostAddress);
			var term: string = request.term;
			var entryType = $("#globalSearchObjectType").val();

			switch (entryType) {
				case "Undefined":
					$.getJSON(urlMapper.mapRelative("/api/entries/names"), { query: term }, (results) => {
						entryFindCallback(response, results);
					});
					break;
				case "Album":
					$.getJSON(urlMapper.mapRelative("/api/albums/names"), { query: term }, (results) => {
						entryFindCallback(response, results);
					});
					break;
				case "Artist":
					$.getJSON(urlMapper.mapRelative("/api/artists/names"), { query: term }, (results) => {
						entryFindCallback(response, results);
					});
					break;
				case "Song":
					$.getJSON(urlMapper.mapRelative("/api/songs/names"), { query: term }, (results) => {
						entryFindCallback(response, results);
					});
					break;
				case "Tag":
					$.getJSON(urlMapper.mapRelative("/api/tags/names"), { query: term, maxResults: 15 }, (results) => {
						entryFindCallback(response, results);
					});
					break;
				case "User":
					$.getJSON(urlMapper.mapRelative("api/users/names"), { query: term }, (results) => {
						entryFindCallback(response, results);
					});
			}

		},
		select: (event: Event, ui) => {
			$("#globalSearchTerm").val(ui.item.value);
			$("#globalSearchBox").submit();
		}
	});


});

function entryFindCallback(response: (items: string[]) => void, results: string[]) {

	response(results);
	/*response($.map(results, function (item) {
		return item;
	}));*/

}

function setLanguagePreferenceCookie(languagePreference: string) {

	$.post("/Home/SetContentPreferenceCookie", { languagePreference: languagePreference }, () => {
		window.location.reload();
	});

	return false;

}