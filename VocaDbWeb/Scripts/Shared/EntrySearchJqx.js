
function clearFindList(findListElem) {

	$(findListElem).jqxListBox('selectIndex', -1);
	var rows = new Array();
	rows.push({ value: "", html: "Nothing" });
	$(findListElem).jqxListBox({ disabled: true });
	$(findListElem).jqxListBox({ source: rows });
	//$(findListElem).jqxListBox({ source: new Array() });
	//$(findListElem).jqxListBox({ source: new Array() });	

}

function initEntrySearch(nameBoxElem, findListElem, entityName, searchUrl, params) {

	var w = 400;
	var h = 350;
	var idElem = null;
	var acceptBtnElem = null;
	var allowCreateNew = false;
	var acceptSelection = null;
	var autoHide = false;
	var extraQueryParams = null;
	var createOptionFirstRow = null;
	var createOptionSecondRow = null;
	var createOptionHtml = null;
	var createTitle = null;

	if (params != null) {

		if (params.width != null)
			w = params.width;

		if (params.height != null)
			h = params.height;

		if (params.acceptBtnElem != null)
			acceptBtnElem = params.acceptBtnElem;

		if (params.allowCreateNew != null)
			allowCreateNew = params.allowCreateNew;

		if (params.acceptSelection != null)
			acceptSelection = params.acceptSelection;

		if (params.autoHide != null)
			autoHide = params.autoHide;

		if (params.idElem != null)
			idElem = params.idElem;

		if (params.extraQueryParams != null)
			extraQueryParams = params.extraQueryParams;

		createOptionFirstRow = params.createOptionFirstRow;
		createOptionSecondRow = params.createOptionSecondRow;
		createOptionHtml = params.createOptionHtml;
		createTitle = params.createTitle;

	}

	$(findListElem).jqxListBox({ width: w, height: h });

	if (autoHide) {
		$(findListElem).hide();
		if (acceptBtnElem)
			$(acceptBtnElem).hide();
	}

	$(findListElem).dblclick(function () {
	    if (acceptBtnElem != null)
	        $(acceptBtnElem).click();
	});

	if (idElem != null) {
		$(findListElem).bind('select', function (event) {

			var item = $(findListElem).jqxListBox('getItem', args.index);

			if (item != null) {
				$(idElem).val(item.value);
			}

		});
	}

	$(nameBoxElem).keyup(function () {

		var findTerm = $(this).val();

		if (isNullOrWhiteSpace(findTerm)) {

			clearFindList(findListElem);
			return;

		}

		var queryParams = { term: findTerm };

		if (extraQueryParams != null)
			jQuery.extend(queryParams, extraQueryParams);

		$.post(searchUrl, queryParams, function (results) {

			if (results.Term != null && $(nameBoxElem).val() != results.Term)
				return;

			var rows = new Array();

			$(results.Items).each(function () {

				var html = null;

				if (createOptionHtml != null) {
					html = createOptionHtml(this);
				} else if (createOptionFirstRow != null && createOptionSecondRow != null) {
					var firstRow = createOptionFirstRow(this);
					var secondRow = createOptionSecondRow(this);
					if (firstRow != null)
						html = "<div tabIndex=0 style='padding: 1px;'><div>" + firstRow + "</div><div>" + secondRow + "</div></div>";
				} else if (createOptionFirstRow != null) {
					var firstRow = createOptionFirstRow(this);
					if (firstRow != null)
						html = "<div tabIndex=0 style='padding: 1px;'><div>" + firstRow + "</div></div>";
				}

				var title = null;

				if (createTitle != null)
					title = createTitle(this);

				if (html != null)
					rows.push({ value: this.Id, html: html, title: title });

			});

			if (allowCreateNew)
				rows.push({ value: "", html: "<div tabIndex=0 style='padding: 1px;'><div>Create new " + entityName + " named '" + findTerm + "'</div></div>" });

			if (rows.length == 0) {
				rows.push({ value: "", html: "Nothing" });
				$(findListElem).jqxListBox({ disabled: true });
			} else {
				$(findListElem).jqxListBox({ disabled: false });
			}

			$(findListElem).show();
			$(findListElem).jqxListBox('selectIndex', -1);
			$(findListElem).jqxListBox('ensureVisible', 0);
			$(findListElem).jqxListBox({ source: rows });

			if (acceptBtnElem)
				$(acceptBtnElem).show();

		});

	});

	$(nameBoxElem).bind("paste", function (e) {
		var elem = $(this);
		setTimeout(function () {
			$(elem).trigger("keyup");
		}, 0);
	});

	if (acceptBtnElem != null) {
		$(acceptBtnElem).click(function () {

			var findTerm = $(nameBoxElem).val();

			if (isNullOrWhiteSpace(findTerm))
				return false;

			var selectedId = "";

			var selectedIndex = $(findListElem).jqxListBox('getSelectedIndex');

			if (selectedIndex < 0)
				return false;

			var item = null;

			item = $(findListElem).jqxListBox('getItem', selectedIndex);

			if (item != null && item.value != null) {
				selectedId = item.value;
			}

			$(nameBoxElem).val("");
			clearFindList(findListElem);

			if (autoHide) {
				$(findListElem).hide();
				if (acceptBtnElem)
					$(acceptBtnElem).hide();
			}

			acceptSelection(selectedId, findTerm);

			return false;

		});
	}

}