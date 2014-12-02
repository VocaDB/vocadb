
function initReportEntryPopup(saveStr, createReportUrl, params) {

	$("#reportEntryPopup").dialog({ autoOpen: false, width: 300, modal: false, buttons: [{ text: saveStr, click: function () {

		$("#reportEntryPopup").dialog("close");

		var reportType = $("#reportType").val();
		var notes = $("#reportNotes").val();

		var queryParams = { reportType: reportType, notes: notes };

		if (params != null)
			jQuery.extend(queryParams, params);

		$.post(createReportUrl, queryParams);

		vdb.ui.showSuccessMessage(vdb.resources.shared.reportSent);

	}}]});

	$("#reportEntryLink").click(function () {

		var addToListLinkPos = $("#reportEntryLink").offset();
		if (addToListLinkPos != null) {
			$("#reportEntryPopup").dialog("option", "position", [addToListLinkPos.left, addToListLinkPos.top - $(window).scrollTop() + 35]);
		}

		$("#reportEntryPopup").dialog("open");
		return false;

	});

}