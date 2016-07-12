
interface KnockoutBindingHandlers {
	// Album autocomplete search box.
	albumAutoComplete: KnockoutBindingHandler;
}

module vdb.knockoutExtensions {
	
	import dc = vdb.dataContracts;

	export function albumAutoComplete(element: HTMLElement, valueAccessor) {

		var properties: AlbumAutoCompleteParams = ko.utils.unwrapObservable(valueAccessor());

		var filter = properties.filter;

		if (properties.ignoreId) {

			filter = (item) => {

				if (item.id == properties.ignoreId) {
					return false;
				}

				return properties.filter != null ? properties.filter(item) : true;

			}

		}

		var queryParams = {
			nameMatchMode: 'Auto',
			lang: vdb.models.globalization.ContentLanguagePreference[vdb.values.languagePreference],
			preferAccurateMatches: true,
			maxResults: 15
		};
		if (properties.extraQueryParams)
			jQuery.extend(queryParams, properties.extraQueryParams);

		vdb.initEntrySearch(element, vdb.functions.mapAbsoluteUrl("/api/albums"),
			{
				acceptSelection: properties.acceptSelection,
				createNewItem: properties.createNewItem,
				createCustomItem: properties.createCustomItem,
				createOptionFirstRow: (item: dc.AlbumContract) => (item.name + " (" + item.discType + ")"),
				createOptionSecondRow: (item: dc.AlbumContract) => (item.artistString),
				extraQueryParams: queryParams,
				filter: filter,
				termParamName: 'query'
			});


	}

}

ko.bindingHandlers.albumAutoComplete = {
	init: vdb.knockoutExtensions.albumAutoComplete
}