
function initPage() {

	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#restoreLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#trashLink").button({ icons: { primary: 'ui-icon-trash'} });
	$("#mergeLink").button();
	//$(".helpToolTip").tooltip({ placement: "right", html: true });
	$("#pvLoader")
		.ajaxStart(function() { $(this).show(); })
		.ajaxStop(function () { $(this).hide(); });

	$("#releaseEvent").autocomplete({
		source: "../../Album/FindReleaseEvents"
	});

	var editArtist;

	$("#editArtistRolesPopup").dialog({ autoOpen: false, width: 550, modal: true, buttons: [{ text: vdb.resources.shared.save, click: function () {

		var checkedRoles = $("#editArtistRolesPopup input.artistRoleCheck:checked").map(function () {
			return $(this).attr("id").split("_")[1];
		}).toArray();

		if (checkedRoles.length == 0)
			checkedRoles = ['Default'];

		var link = editArtist;
		if (link)
			link.rolesArray(checkedRoles);

		$("#editArtistRolesPopup").dialog("close");

	}}]});

	$("input.artistRoleCheck").button();

	$(document).on("click", "a.artistRolesEdit", function () {

		var data = ko.dataFor(this);

		editArtist = data;

		var roles = data.rolesArray();
		$("#editArtistRolesPopup input.artistRoleCheck").each(function () {
			$(this).removeAttr("checked");
			$(this).button("refresh");
		});

		$(roles).each(function () {
			var check = $("#editArtistRolesPopup #artistRole_" + this.trim());
			$(check).attr("checked", "checked");
			$(check).button("refresh");
		});

		$("#editArtistRolesPopup").dialog("open");

		return false;

	});

}
