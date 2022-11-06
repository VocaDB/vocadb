import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { SongInAlbumEditContract } from '@/DataContracts/Song/SongInAlbumEditContract';
import { makeObservable, observable, reaction } from 'mobx';

export class SongInAlbumEditStore {
	private static nextId = 1;

	readonly id: number;
	@observable artists: ArtistContract[];
	@observable artistString: string;
	@observable discNumber: number;
	readonly isCustomTrack: boolean;
	@observable isNextDisc: boolean;
	@observable selected: boolean;
	readonly songAdditionalNames: string;
	readonly songId: number;
	readonly songInAlbumId: number;
	readonly songName: string;
	@observable trackNumber: number;

	constructor(data: SongInAlbumEditContract) {
		makeObservable(this);

		this.id = SongInAlbumEditStore.nextId++;
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

		reaction(
			() => this.artists.map((artist) => ({ name: artist.name })),
			(artists) => {
				// TODO: construct proper artist string (from server)
				this.artistString = artists.map((a) => a.name).join(', ');
			},
		);
	}
}
