
function initPage() {

	$("#tabs").tabs();
	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#restoreLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#mergeLink").button();
	$("#pvLoader")
		.ajaxStart(function () { $(this).show(); })
		.ajaxStop(function () { $(this).hide(); });

	$("#artistsTableBody a.artistLink").vdbArtistToolTip();
	
}
