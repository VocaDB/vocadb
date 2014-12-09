module vdb {

	export interface EntryAutoCompleteParams<TContract> {

		acceptSelection: (entryId?: number, name?: string, entryType?: string) => void;

		createCustomItem?: string;

		createNewItem?: string;

		createOptionFirstRow: (entry: TContract) => string;

		createOptionHtml?: (entry: TContract) => string;

		createOptionSecondRow: (entry: TContract) => string;

		extraQueryParams: any;

		filter?: (entry: TContract) => boolean;

		height?: number;

		termParamName?: string;

		width?: number;

	}

	export interface AutoCompleteItem<TContract> {

		data?: TContract;

		label: string;

		itemType?: string;

		term: string;

		value?: number;

	}

	export function initEntrySearch<TContract extends vdb.dataContracts.EntryWithTagUsagesContract>(nameBoxElem: HTMLElement, entityName: string, searchUrl: string, params: EntryAutoCompleteParams<TContract>) {

		var w = 400;
		var h = 350;
		var createNewItem = null;
		var createCustomItem = null;
		var acceptSelection = null;
		var extraQueryParams = null;
		var createOptionFirstRow = null;
		var createOptionSecondRow = null;
		var createOptionHtml = null;
		var filter: (entry: TContract) => boolean = null;
		var method = 'GET';
		var termParamName = 'query';

		if (params) {

			if (params.width)
				w = params.width;

			if (params.height)
				h = params.height;

			if (params.createNewItem)
				createNewItem = params.createNewItem;

			if (params.acceptSelection != null)
				acceptSelection = params.acceptSelection;

			if (params.extraQueryParams != null)
				extraQueryParams = params.extraQueryParams;

			if (params.filter != null)
				filter = params.filter;

			if (params.termParamName)
				termParamName = params.termParamName;

			createOptionFirstRow = params.createOptionFirstRow;
			createOptionSecondRow = params.createOptionSecondRow;
			createOptionHtml = params.createOptionHtml;
			createCustomItem = params.createCustomItem;

		}

		function bold(text: string, term: string) {
			return vdb.functions.boldCaseInsensitive(text, term);
		}

		function createHtml(item: AutoCompleteItem<TContract>) {

			var data = item.data;

			if (!data) {
				return "<a><div>" + item.label + "</div></a>";
			}

			var html = null;
			var term = item.term;
			var firstRow;

			if (createOptionHtml) {
				html = createOptionHtml(data);
			} else if (createOptionFirstRow && createOptionSecondRow) {
				firstRow = createOptionFirstRow(data);
				var secondRow = createOptionSecondRow(data);
				if (firstRow)
					html = "<a><div>" + bold(firstRow, term) + "</div><div><small class='extraInfo'>" + secondRow + "</small></div></a>";
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