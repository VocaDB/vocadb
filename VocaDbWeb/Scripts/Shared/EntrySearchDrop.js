function initEntrySearch(nameBoxElem, findListElem, entityName, searchUrl, params) {

	var w = 400;
	var h = 350;
	var idElem = null;
	var createNewItem = null;
	var createCustomItem = null;
	var acceptSelection = null;
	var extraQueryParams = null;
	var createOptionFirstRow = null;
	var createOptionSecondRow = null;
	var createOptionHtml = null;
	var createTitle = null;
	var filter = null;
	var method = 'POST';
	var termParamName = 'term';

	if (params != null) {

		if (params.width != null)
			w = params.width;

		if (params.height != null)
			h = params.height;

		if (params.createNewItem)
			createNewItem = params.createNewItem;

		if (params.acceptSelection != null)
			acceptSelection = params.acceptSelection;

		if (params.idElem != null)
			idElem = params.idElem;

		if (params.extraQueryParams != null)
			extraQueryParams = params.extraQueryParams;

		if (params.filter != null)
			filter = params.filter;

		if (params.method)
			method = params.method;

		if (params.termParamName)
			termParamName = params.termParamName;

		createOptionFirstRow = params.createOptionFirstRow;
		createOptionSecondRow = params.createOptionSecondRow;
		createOptionHtml = params.createOptionHtml;
		createTitle = params.createTitle;
		createCustomItem = params.createCustomItem;

	}

	function bold(text, term) {
		return vdb.functions.boldCaseInsensitive(text, term);
	}

	function createHtml(item) {

		var data = item.data;

		if (!data) {
			return "<a><div>" + item.label + "</div></a>";
		}

		var html = null;
		var term = item.term;

		if (createOptionHtml) {
			html = createOptionHtml(data);
		} else if (createOptionFirstRow && createOptionSecondRow) {
			var firstRow = createOptionFirstRow(data);
			var secondRow = createOptionSecondRow(data);
			if (firstRow)
				html = "<a><div>" + bold(firstRow, term) + "</div><div><small class='extraInfo'>" + secondRow + "</small></div></a>";
		} else if (createOptionFirstRow) {
			var firstRow = createOptionFirstRow(data);
			if (firstRow)
				html = "<a><div>" + bold(firstRow, term) + "</div></a>";
		}

		return html;

	}

	function getItems(par, response) {

		var queryParams = {};
		queryParams[termParamName] = par.term;

		if (extraQueryParams != null)
			jQuery.extend(queryParams, extraQueryParams);

		$.ajax({ type: method, url: searchUrl, data: queryParams, success: function (result) {

			var filtered = (!filter ? result.Items || result.items : _.filter(result.Items || result.items, filter));

			var mapped = $.map(filtered, function (item) {
				return { label: item.Name || item.name, value: item.Id || item.id, data: item, term: par.term };
			});

			if (createNewItem)
				mapped.push({ label: createNewItem.replace("{0}", par.term), value: "", term: par.term, itemType: 'new' });

			if (createCustomItem)
				mapped.push({ label: createCustomItem.replace("{0}", par.term), value: "", term: par.term, itemType: 'custom' });

			response(mapped);

		}});

	}

	function selectItem(event, ui) {

		if (idElem)
			$(idElem).val(ui.item.value);

		// namebox value is cleared when using keyboard
		acceptSelection(ui.item.value, $(nameBoxElem).val() || ui.item.term, ui.item.itemType);
		$(nameBoxElem).val("");

		return false;

	}

	var auto = $(nameBoxElem).autocomplete({
		source: getItems,
		select: selectItem
	})
	.data("ui-autocomplete");

	if (auto) {
		auto._renderItem = function (ul, item) {
			return $("<li>")
				.data("item.ui-autocomplete", item)
				.append(createHtml(item))
				.appendTo(ul);
		};
	}

}