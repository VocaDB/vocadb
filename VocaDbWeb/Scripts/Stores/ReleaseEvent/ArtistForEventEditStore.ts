import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { ArtistForEventContract } from '@/DataContracts/ReleaseEvents/ArtistForEventContract';
import { IEditableArtistWithSupport } from '@/Stores/ArtistForAlbumEditStore';
import { computed, makeObservable, observable } from 'mobx';

// Store for editing artist for album link.
export class ArtistForEventEditStore implements IEditableArtistWithSupport {
	public readonly artist: ArtistContract;
	// Unique link Id.
	public readonly id: number;
	public readonly name: string;
	// List of roles for this artist.
	@observable public rolesArray: string[];

	public constructor(data: ArtistForEventContract) {
		makeObservable(this);

		this.artist = data.artist!;
		this.id = data.id!;

		this.name = data.name!;
		this.rolesArray = [];

		this.roles = data.roles;
	}

	// Roles as comma-separated string (for serializing to and from .NET enum for the server)
	@computed public get roles(): string {
		return this.rolesArray.join();
	}
	public set roles(value: string) {
		this.rolesArray = value.split(',').map((val) => val.trim());
	}

	public toContract = (): ArtistForEventContract => {
		return {
			artist: this.artist,
			id: this.id,
			name: this.name,
			roles: this.roles,
		};
	};
}
