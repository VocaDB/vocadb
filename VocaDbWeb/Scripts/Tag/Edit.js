
function initPage(tagName) {

	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash' } });
	$("#categoryName").autocomplete({
		source: "../../Tag/FindCategories"
	});
	$("#aliasedTo").autocomplete({
		source: function (params, callback) {
			$.post(vdb.functions.mapUrl("/Tag/Find"), { term: params.term, allowAliases: false }, function (result) {
				var tags = _.filter(result, function (item) { return item != tagName; });
				callback(tags);
			});
		}
	});

}