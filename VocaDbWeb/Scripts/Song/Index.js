
// Note: only for reference, not in use

function IndexViewModel(model) {

	var self = this;

	this.artistId = ko.observable(model.artistId);
	this.artistName = ko.observable(model.artistName);
	this.draftsOnly = ko.observable(model.draftsOnly);
	this.filter = ko.observable(model.filter);
	this.matchMode = ko.observable(model.matchMode);
	this.onlyWithPVs = ko.observable(model.onlyWithPVs);
	this.since = ko.observable(model.since);
	this.songType = ko.observable(model.songType);
	this.sort = ko.observable(model.sort);
	this.view = ko.observable(model.view);
	this.filterArtistName = ko.observable(model.artistName);
	this.songs = ko.observableArray([]);

	getArtistName();

	this.filterString = ko.computed(function () {

		var first = true;
		var str = "";
		var filterPre = vdb.resources.entryIndex.FilterPre + " ";

		function appendJoin() {

			if (first) {
				str = filterPre;
				first = false;
			} else {
				str += ", ";
			}

		}

		if (!model.artistId && !isNullOrWhiteSpace(model.filter)) {

			appendJoin();
			var f = model.filter;

			if (self.matchMode() == "Exact") {
				str += vdb.resources.entryIndex.ExactTitleFilter.replace("{0}", f);
			} else if (self.matchMode() == "StartsWith") {
				str += vdb.resources.entryIndex.StartsWithTitleFilter.replace("{0}", f);
			} else {
				str += vdb.resources.entryIndex.TitleFilter.replace("{0}", f);
			}

		}

		if (self.artistName()) {

			appendJoin();
			str += vdb.resources.song.ArtistFilter.replace("{0}", self.artistName());

		}

		if (model.songType && model.songType != 'Unspecified') {

			appendJoin();

			var songTypeName = vdb.resources.songTypes[model.songType];

			str += vdb.resources.song.SongTypeFilter.replace("{0}", songTypeName);

		}

		if (model.onlyWithPVs) {

			appendJoin();

			str += vdb.resources.song.OnlyWithPVsFilter;

		}

		if (model.since) {

			appendJoin();

			str += vdb.resources.song.SinceFilter.replace("{0}", model.since);

		}

		if (model.draftsOnly) {

			appendJoin();

			str += vdb.resources.entryIndex.DraftsOnlyFilter;

		}

		if (str)
			str += ".";

		return str;

	});

	this.hasFilter = ko.computed(function () {
		return self.filterString();
	});

	this.clearArtist = function () {

		self.artistId(undefined);
		self.filterArtistName(undefined);

	};

	function getArtistName() {

		if (!self.artistId()) {
			self.filterArtistName("");
			return;
		}

		var url = vdb.functions.mapUrl("/Artist/Info");
		$.post(url, { id: self.artistId() }, function (result) {
			self.artistName(result.Name);
			self.filterArtistName(result.Name);
		});

	}

	var findArtistsUrl = vdb.functions.mapUrl("/Artist/FindJson");

	$("#artistNameSearch").autocomplete({
		source: function (request, response) {
			$.post(findArtistsUrl, { term: request.term }, function (results) {
				var items = _.map(results.Items, function (item) {
					return { label: item.Name + " (" + item.ArtistType + ")", value: item.Id };
				});
				response(items);
			});
		},
		select: function (event, ui) {
			if (ui.item.value) {
				self.artistId(ui.item.value);
				self.filterArtistName(ui.item.label);
			}
		}
	});

}

function initPage(model) {

	$("#createLink").button({ disabled: $("#createLink").hasClass("disabled"), icons: { primary: 'ui-icon-plusthick'} });
	$("#filterBox").tooltip();
	$(".albumLink").vdbAlbumWithCoverToolTip();

	var viewModel = new IndexViewModel(model);
	ko.applyBindings(viewModel);

}
