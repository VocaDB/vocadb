
class ViewModel {

	public loadUrl = () => {

		var regex = /http:\/\/vocadb\.net\/S\/(\d+)/g;
		var match = regex.exec(this.url());

		if (match && match.length < 2)
			return;

		var id = match[1];

		$.getJSON("http://vocadb.net/api/songs/" + id, { fields: 'Artists,PVs' }, song => {

			if (!song)
				return;

			var vocaDBUrl = "http://vocadb.net/S/" + id;

			var hasOriginalYT = _.some(<any[]>song.pVs, pv => pv.service === 'Youtube' && pv.pvType === 'Original');
			var ytReprint: any;

			if (!hasOriginalYT) {

				ytReprint = _.find(<any[]>song.pVs, pv => pv.service === 'Youtube' && pv.pvType === 'Reprint');

			}

			var producers = _.filter(<any[]>song.artists, a => !a.isSupport && a.artist && _.includes(a.categories, "Producer"));

			var artistPromises = _.map(producers, (p: any) => $.getJSON("http://vocadb.net/api/artists/" + p.artist.id, { fields: "AdditionalNames,WebLinks" }));

			$.when.apply($, artistPromises).done((...artists: any[]) => {

				var result = "";

				if (ytReprint) {
					result = "[YouTube reprint](" + ytReprint.url + ")\n\n";
				}

				result += "[Song on VocaDB](" + vocaDBUrl + ")\n\n";

				result += _.map(_.take(artists, artists.length - 2), (a: any) => {

					var nicoUser = _.find(a.webLinks, (l: any) => l.url.startsWith("http://www.nicovideo.jp/user/"));
					var mylist = _.find(a.webLinks, (l: any) => l.url.startsWith("http://www.nicovideo.jp/mylist/"));
					var ytChannel = _.find(a.webLinks, (l: any) => l.url.startsWith("https://www.youtube.com/channel/"));

					var artistVocaDB = "http://vocadb.net/Ar/" + a.id;

					var artistRow = "Artist: " + a.name;

					if (a.additionalNames) {
						artistRow += " (" + a.additionalNames + ")";
					}

					artistRow += " - ";

					if (mylist) {
						artistRow += "\n[MyList](" + mylist.url + "),";
					}

					if (nicoUser) {
						artistRow += "\n[Nico user page](" + nicoUser.url + "),";
					}

					if (ytChannel) {
						artistRow += "\n[YouTube](" + ytChannel.url + ")";						
					}

					artistRow += "\n[VocaDB](" + artistVocaDB + ")";

					return artistRow;

				}).join("\n\n");	

				this.title(song.name);
				this.result(result);

			});


		});

	}

	public result = ko.observable("");

	public title = ko.observable("");

	public url = ko.observable("");


}

ko.applyBindings(new ViewModel());