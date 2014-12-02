
$(document).ready(function () {

	$("#globalSearchTerm").autocomplete({
		source: function (request, response) {

			var term = request.term;
			var entryType = $("#globalSearchObjectType").val();

			switch (entryType) {
				case "Undefined":
					$.post("../../Home/FindNames", { term: term }, function (results) {
						entryFindCallback(response, results);
					});
					break;
				case "Album":
					$.post("../../Album/FindNames", { term: term }, function (results) {
						entryFindCallback(response, results);
					});
					break;
				case "Artist":
					$.post("../../Artist/FindNames", { term: term }, function (results) {
						entryFindCallback(response, results);
					});
					break;
				case "Song":
					$.post("../../Song/FindNames", { term: term }, function (results) {
						entryFindCallback(response, results);
					});
					break;
				case "Tag":
					$.post("../../Tag/Find", { term: term }, function (results) {
						entryFindCallback(response, results);
					});
					break;
				case "User":
					$.post("../../User/FindByName", { term: term }, function (results) {
						entryFindCallback(response, results);
					});
			}

		},
		select: function (event, ui) {
			$("#globalSearchTerm").val(ui.item.value);
			$("#globalSearchBox").submit();
		}
	});


});

function entryFindCallback(response, results) {

	response($.map(results, function( item ) {
		return item; 
	}));

}

function setLanguagePreferenceCookie(languagePreference) {

	$.post("/Home/SetContentPreferenceCookie", { languagePreference: languagePreference }, function() {
		window.location.reload();
	});

	return false;

}