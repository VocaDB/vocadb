
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

		$("#reportEntryPopup").dialog("option", "position", { my: "left top", at: "left bottom", of: $("#reportEntryLink") });
		$("#reportEntryPopup").dialog("open");
		return false;

	});

}