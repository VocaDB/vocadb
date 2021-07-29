import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import HtmlHelper from '@Helpers/HtmlHelper';
import JQueryUIAutocomplete from '@JQueryUI/JQueryUIAutocomplete';
import HttpClient from '@Shared/HttpClient';
import $ from 'jquery';
import _ from 'lodash';
import React from 'react';

const httpClient = new HttpClient();

export interface EntryAutoCompleteParams<TContract> {
	acceptSelection: (
		entryId?: number,
		name?: string,
		entryType?: string,
		data?: TContract,
	) => void;

	createCustomItem?: string;

	// Label template for creating a new item.
	// {0} will be replaced with the searched item, for example "Create new song named {0}".
	// If empty or undefined, no new entries can be created this way.
	createNewItem?: string;

	// Creates the content for the first row of the autocomplete item.
	// HTML is escaped.
	createOptionFirstRow: (entry: TContract) => string;

	// Creates the content for the second row of the autocomplete item (optional).
	// HTML is escaped.
	createOptionSecondRow?: (entry: TContract) => string;

	extraQueryParams: any;

	filter?: (entry: TContract) => boolean;

	// Callback for manipulating the query params based on the actual query
	// queryParams: search query params (parameters to the query)
	// term: query term
	onQuery?: (queryParams: any, term: string) => void;

	singleRow?: boolean;

	termParamName?: string;
}

interface AutoCompleteItem<TContract> {
	data?: TContract;
	label: string;
	itemType?: string;
	term: string;
	value?: number;
}

interface EntryAutoCompleteProps<TContract extends any>
	extends React.InputHTMLAttributes<HTMLInputElement> {
	searchUrl: string;
	params: EntryAutoCompleteParams<TContract>;
}

const EntryAutoComplete = <TContract extends { id: number; name: string }>({
	searchUrl,
	params: {
		acceptSelection,
		createCustomItem,
		createNewItem,
		createOptionFirstRow,
		createOptionSecondRow,
		extraQueryParams,
		filter,
		onQuery,
		singleRow,
		termParamName = 'query',
	},
	...props
}: EntryAutoCompleteProps<TContract>): React.ReactElement => {
	const inputRef = React.useRef<HTMLInputElement>(undefined!);

	const getItems = (
		par: { term: string },
		response: (result: AutoCompleteItem<TContract>[]) => void,
	): void => {
		var queryParams: any = {};
		queryParams[termParamName] = par.term;

		if (onQuery) onQuery(queryParams, par.term);

		if (extraQueryParams) Object.assign(queryParams, extraQueryParams);

		httpClient
			.get<PartialFindResultContract<TContract>>(searchUrl, queryParams)
			.then((result) => {
				const filtered = !filter
					? result.items
					: _.filter(result.items, filter);

				const mapped: AutoCompleteItem<TContract>[] = _.map(
					filtered,
					(item) => {
						return {
							label: item.name,
							value: item.id,
							data: item,
							term: par.term,
						};
					},
				);

				if (createNewItem) {
					mapped.push({
						label: createNewItem.replace('{0}', par.term),
						term: par.term,
						itemType: 'new',
					});
				}

				if (createCustomItem) {
					mapped.push({
						label: createCustomItem.replace('{0}', par.term),
						term: par.term,
						itemType: 'custom',
					});
				}

				response(mapped);
			});
	};

	const selectItem = (
		event: Event,
		ui: { item?: AutoCompleteItem<TContract> },
	): boolean => {
		const item: AutoCompleteItem<TContract> = ui.item!;

		// namebox value is cleared when using keyboard
		acceptSelection(
			item.value,
			inputRef.current.value || item.term,
			item.itemType,
			item.data,
		);
		inputRef.current.value = '';

		return false;
	};

	const bold = (text: string, term: string): string => {
		return HtmlHelper.boldAndHtmlEncode(text, term);
	};

	const createHtml = (item: AutoCompleteItem<TContract>): string => {
		const data = item.data;

		if (!data) {
			return `<a><div>${item.label}</div></a>`;
		}

		var html: string = null!;
		const term = item.term;
		var firstRow: string;

		if (createOptionFirstRow && createOptionSecondRow) {
			firstRow = createOptionFirstRow(data);
			const secondRow = createOptionSecondRow(data);
			if (firstRow) {
				if (singleRow) {
					html = `<a><div>${bold(
						firstRow,
						term,
					)} <small class='extraInfo'>${HtmlHelper.htmlEncode(
						secondRow,
					)}</small></div></a>`;
				} else {
					html = `<a><div>${bold(
						firstRow,
						term,
					)}</div><div><small class='extraInfo'>${HtmlHelper.htmlEncode(
						secondRow,
					)}</small></div></a>`;
				}
			}
		} else if (createOptionFirstRow) {
			firstRow = createOptionFirstRow(data);
			if (firstRow) html = '<a><div>' + bold(firstRow, term) + '</div></a>';
		}

		return html;
	};

	return (
		<JQueryUIAutocomplete
			source={getItems}
			select={selectItem}
			renderItem={(
				ul: HTMLElement,
				item: AutoCompleteItem<TContract>,
			): JQuery => {
				return $('<li>')
					.data('item.ui-autocomplete', item)
					.append(createHtml(item))
					.appendTo(ul);
			}}
			{...props}
			ref={inputRef}
		/>
	);
};

export default EntryAutoComplete;
