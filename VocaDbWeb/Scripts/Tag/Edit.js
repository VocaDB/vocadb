
function initPage() {

	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash' } })
		.next().button({ icons: { primary: "ui-icon-trash" } }).parent().buttonset();
	$("#trashLink").button({ icons: { primary: 'ui-icon-trash' } });
	$("#restoreLink").button({ icons: { primary: 'ui-icon-trash' } });

}