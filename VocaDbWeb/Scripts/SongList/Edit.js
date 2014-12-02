
var Song = function (data) {

	this.songInListId = data.songInListId;
	this.notes = ko.observable(data.notes);
	this.order = ko.observable(data.order);
	this.songId = data.songId;
	this.songName = data.songName;
	this.songAdditionalNames = data.songAdditionalNames;
	this.songArtistString = data.songArtistString;

};

function SongListViewModel(data) {

	var self = this;

	this.id = data.id;
	this.currentName = data.name;
	this.name = ko.observable(data.name);
	this.description = ko.observable(data.description);
	this.featuredCategory = ko.observable(data.featuredCategory);
	this.songLinks = ko.observableArray([]);

	var mappedSongs = $.map(data.songLinks, function (item) { return new Song(item); });
	this.songLinks(mappedSongs);

	this.songLinks.subscribe(function() {

		for (var track = 0; track < self.songLinks().length; ++track) {
			self.songLinks()[track].order(track + 1);
		}

	});

	function acceptSongSelection(songId) {

		if (!isNullOrWhiteSpace(songId)) {
			$.post(vdb.functions.mapAbsoluteUrl("/Song/DataById"), { id: songId }, function (song) {
				var songInList = new Song({ songInListId: 0, order: 0, songId: song.id, songName: song.name, songAdditionalNames: song.additionalNames, SongArtistString: song.artistString, Notes: "" });
				self.songLinks.push(songInList);
			});
		}

	}

	this.removeSong = function (songLink) {
		self.songLinks.remove(songLink);
	};

	/*this.save = function () {
		ko.utils.postJson(location.href, { model: ko.toJS(self) });
	};*/
	
	this.songSearchParams = {
		acceptSelection: acceptSongSelection,
	};

};

function initPage(listId) {

	$("#tabs").tabs();
	$("#deleteLink").button({ icons: { primary: 'ui-icon-trash'} });

	$.getJSON("/SongList/Data", { id: listId }, function (songListData) {
		var viewModel = new SongListViewModel(songListData);
		ko.applyBindings(viewModel);
		$("#songListForm").validate();
		//$("#songListForm").validate({ submitHandler: function () { viewModel.save(); } });
	});

}