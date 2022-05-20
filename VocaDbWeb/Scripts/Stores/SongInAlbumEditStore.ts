import { makeObservable, observable } from 'mobx';

import ArtistContract from '../DataContracts/Artist/ArtistContract';
import SongInAlbumEditContract from '../DataContracts/Song/SongInAlbumEditContract';

export default class SongInAlbumEditStore {
	@observable public artists: ArtistContract[];
	@observable public artistString: string;
	@observable public discNumber: number;
	public readonly isCustomTrack: boolean;
	@observable public isNextDisc: boolean;
	@observable public selected: boolean;
	public readonly songAdditionalNames: string;
	public readonly songId: number;
	public readonly songInAlbumId: number;
	public readonly songName: string;
	@observable public trackNumber: number;

	public constructor(data: SongInAlbumEditContract) {
		makeObservable(this);

		this.artists = data.artists;
		this.artistString = data.artistString;
		this.discNumber = data.discNumber;
		this.isCustomTrack = data.isCustomTrack || false;
		this.songAdditionalNames = data.songAdditionalNames;
		this.songId = data.songId;
		this.songInAlbumId = data.songInAlbumId;
		this.songName = data.songName;
		this.trackNumber = data.trackNumber;

		this.isNextDisc = this.trackNumber === 1 && this.discNumber > 1;
		this.selected = false;

		// TODO
	}
}
