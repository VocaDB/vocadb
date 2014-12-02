/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../typings/knockout/knockout.d.ts" />
/// <reference path="../../Shared/GlobalValues.ts" />
/// <reference path="../../Shared/Messages.d.ts" />
/// <reference path="../../Repositories/SongRepository.ts" />
/// <reference path="../../Repositories/UserRepository.ts" />
/// <reference path="../../ViewModels/Song/SongWithPreviewViewModel.ts" />

// Note: only for reference, not in use.

interface KnockoutBindingHandlers {
    pvPreviewStatus: KnockoutBindingHandler;
}

// Initializes and maintains song rating status for the HTML table.
// TODO: not in use?
ko.bindingHandlers.pvPreviewStatus = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {

        var urlMapper = new vdb.UrlMapper(vdb.values.baseAddress);
        var songRepository = new vdb.repositories.SongRepository(vdb.values.baseAddress);
        var userRepository = new vdb.repositories.UserRepository(urlMapper);
        var pvRows = $(element).find(".js-pvRow");
        var songsArray = valueAccessor();

        // Parse all rows and create child binding context for each of them
        var songItems = _.map(pvRows, function (pvRow) {

            var songId = $(pvRow).data("entry-id");

            var item = new vdb.viewModels.SongWithPreviewViewModel(songRepository, userRepository, songId, null);
            item.ratingComplete = vdb.ui.showThankYouForRatingMessage;

            var childBindingContext = bindingContext.createChildContext(item);
            ko.applyBindingsToDescendants(childBindingContext, pvRow);
            return item;
        
        });

        songsArray(songItems);

        return { controlsDescendantBindings: true };
    }
};
