
function initPage() {

	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash' } });
	$("#categoryName").autocomplete({
		source: "../../Tag/FindCategories"
	});

}