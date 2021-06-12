import ArtistContract from '@DataContracts/Artist/ArtistContract';
import SongInAlbumEditContract from '@DataContracts/Song/SongInAlbumEditContract';
import ko, { Observable, ObservableArray } from 'knockout';
import _ from 'lodash';

export default class SongInAlbumEditViewModel {
	public artists: ObservableArray<ArtistContract>;

	public artistString: Observable<string>;

	public discNumber: Observable<number>;

	public isCustomTrack: boolean;

	public isNextDisc: Observable<boolean>;

	public selected: Observable<boolean>;

	public songAdditionalNames: string;

	public songId: number;

	public songInAlbumId: number;

	public songName: string;

	public trackNumber: Observable<number>;

	public constructor(data: SongInAlbumEditContract) {
		this.artists = ko.observableArray(data.artists);
		this.artistString = ko.observable(data.artistString);
		this.discNumber = ko.observable(data.discNumber);
		this.isCustomTrack = data.isCustomTrack || false;
		this.songAdditionalNames = data.songAdditionalNames;
		this.songId = data.songId;
		this.songInAlbumId = data.songInAlbumId;
		this.songName = data.songName;
		this.trackNumber = ko.observable(data.trackNumber);

		this.isNextDisc = ko.observable(
			this.trackNumber() === 1 && this.discNumber() > 1,
		);
		this.selected = ko.observable(false);

		this.artists.subscribe(() => {
			// TODO: construct proper artist string (from server)
			this.artistString(_.map(this.artists(), (a) => a.name).join(', '));
		});
	}
}
