module vdb {

	export interface EntryAutoCompleteParams<TContract> {

		acceptSelection: (entryId?: number, name?: string, entryType?: string) => void;

		createCustomItem?: string;

		createNewItem?: string;

		// Creates the content for the first row of the autocomplete item.
		// HTML is escaped.
		createOptionFirstRow: (entry: TContract) => string;

		// Creates the content for the second row of the autocomplete item (optional).
		// HTML is escaped.
		createOptionSecondRow: (entry: TContract) => string;

		extraQueryParams: any;

		filter?: (entry: TContract) => boolean;

		termParamName?: string;

	}

	export interface AutoCompleteItem<TContract> {

		data?: TContract;

		label: string;

		itemType?: string;

		term: string;

		value?: number;

	}

	export function initEntrySearch<TContract extends vdb.dataContracts.EntryWithTagUsagesContract>(nameBoxElem: HTMLElement, entityName: string, searchUrl: string, params: EntryAutoCompleteParams<TContract>) {

		if (!params)
			throw Error("params cannot be null");

		var createNewItem = params.createNewItem;
		var createCustomItem = params.createCustomItem;
		var acceptSelection = params.acceptSelection;
		var extraQueryParams = params.extraQueryParams;
		var createOptionFirstRow = params.createOptionFirstRow;
		var createOptionSecondRow = params.createOptionSecondRow;
		var filter = params.filter;
		var termParamName = params.termParamName || 'query';
		var method = 'GET';

		function bold(text: string, term: string) {
			return vdb.helpers.HtmlHelper.boldAndHtmlEncode(text, term);
		}

		function createHtml(item: AutoCompleteItem<TContract>) {

			var data = item.data;

			if (!data) {
				return "<a><div>" + item.label + "</div></a>";
			}

			var html: string = null;
			var term = item.term;
			var firstRow: string;

			if (createOptionFirstRow && createOptionSecondRow) {
				firstRow = createOptionFirstRow(data);
				var secondRow = createOptionSecondRow(data);
				if (firstRow)
					html = "<a><div>" + bold(firstRow, term) + "</div><div><small class='extraInfo'>" + vdb.helpers.HtmlHelper.htmlEncode(secondRow) + "</small></div></a>";
			} else if (createOptionFirstRow) {
				firstRow = createOptionFirstRow(data);
				if (firstRow)
					html = "<a><div>" + bold(firstRow, term) + "</div></a>";
			}

			return html;

		}

		function getItems(par, response: (result: AutoCompleteItem<TContract>[]) => void) {

			var queryParams = {};
			queryParams[termParamName] = par.term;

			if (extraQueryParams != null)
				jQuery.extend(queryParams, extraQueryParams);

			$.ajax({ type: method, url: searchUrl, data: queryParams, success: (result: vdb.dataContracts.PartialFindResultContract<TContract>) => {

				var filtered = (!filter ? result.items : _.filter(result.items, filter));

				var mapped: AutoCompleteItem<TContract>[] = _.map(filtered, (item) => {
					return { label: item.name, value: item.id, data: item, term: par.term };
				});

				if (createNewItem)
					mapped.push({ label: createNewItem.replace("{0}", par.term), value: null, term: par.term, itemType: 'new' });

				if (createCustomItem)
					mapped.push({ label: createCustomItem.replace("{0}", par.term), value: null, term: par.term, itemType: 'custom' });

				response(mapped);

			}});

		}

		function selectItem(event: Event, ui) {

			var item: AutoCompleteItem<TContract> = ui.item;

			// namebox value is cleared when using keyboard
			acceptSelection(item.value, $(nameBoxElem).val() || item.term, item.itemType);
			$(nameBoxElem).val("");

			return false;

		}

		var auto = $(nameBoxElem).autocomplete({
			source: getItems,
			select: selectItem
		}).data("ui-autocomplete");

		if (auto) {
			auto._renderItem = (ul: HTMLElement, item: AutoCompleteItem<TContract>) => {
				return $("<li>")
					.data("item.ui-autocomplete", item)
					.append(createHtml(item))
					.appendTo(ul);
			};
		}

	}

}