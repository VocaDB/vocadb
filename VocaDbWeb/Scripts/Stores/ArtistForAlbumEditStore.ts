import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { ArtistForAlbumContract } from '@/DataContracts/ArtistForAlbumContract';
import { ArtistHelper } from '@/Helpers/ArtistHelper';
import { ArtistRoles } from '@/Models/Artists/ArtistRoles';
import { computed, makeObservable, observable } from 'mobx';

export interface IEditableArtistWithSupport {
	rolesArray: string[];
}

// Store for editing artist for album link.
export class ArtistForAlbumEditStore implements IEditableArtistWithSupport {
	public readonly artist: ArtistContract;
	// Unique link Id.
	public readonly id: number;
	public isCustomName: boolean;
	@observable public isSupport: boolean;
	@observable public name: string;
	@observable public nameDialogVisible = false;
	@observable public rolesArray: string[];

	public constructor(data: ArtistForAlbumContract) {
		makeObservable(this);

		this.artist = data.artist!;
		this.id = data.id!;
		this.isCustomName = data.isCustomName!;
		this.isSupport = data.isSupport!;

		this.name = data.name!;
		this.rolesArray = [];

		this.roles = data.roles;
	}

	// Whether the roles of this artist can be customized.
	@computed public get isCustomizable(): boolean {
		return !this.artist || ArtistHelper.isCustomizable(this.artist.artistType);
	}

	// Roles as comma-separated string (for serializing to and from .NET enum for the server)
	@computed public get roles(): string {
		return this.rolesArray.join();
	}
	public set roles(value: string) {
		this.rolesArray = value.split(',').map((val) => val.trim());
	}

	@computed public get rolesArrayTyped(): ArtistRoles[] {
		return ArtistHelper.getRolesArray(this.rolesArray);
	}

	public toContract = (): ArtistForAlbumContract => {
		return {
			artist: this.artist,
			id: this.id,
			isCustomName: this.isCustomName,
			isSupport: this.isSupport,
			name: this.name,
			roles: this.roles,
		};
	};
}
