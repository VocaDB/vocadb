
function initPage() {

	$("#trashLink").button({ icons: { primary: 'ui-icon-trash' } });
	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash' } })
		.next().button({ icons: { primary: "ui-icon-trash" } }).parent().buttonset();
	$("#restoreLink").button({ icons: { primary: 'ui-icon-trash' } });

}