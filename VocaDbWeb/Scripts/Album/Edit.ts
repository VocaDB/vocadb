
export function initPage() {

	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#restoreLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#trashLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#mergeLink").button();
	$("#pvLoader")
		.ajaxStart(function() { $(this).show(); })
		.ajaxStop(function () { $(this).hide(); });

}
