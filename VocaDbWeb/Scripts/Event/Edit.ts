
export function initPage() {

	$("#tabs").tabs();
	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#trashLink").button({ icons: { primary: 'ui-icon-trash' } });
	$("#restoreLink").button({ icons: { primary: 'ui-icon-trash' } });

}