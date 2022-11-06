import { ArtistRoles } from '@/Models/Artists/ArtistRoles';
import { IEditableArtistWithSupport } from '@/Stores/ArtistForAlbumEditStore';
import { action, makeObservable, observable } from 'mobx';

export interface RoleSelection {
	// Role Id, for example "VoiceManipulator"
	id: string;
	// User-visible role name, for example "Voice Manipulator"
	name?: string;
	selected: boolean;
}

class RoleSelectionStore implements RoleSelection {
	@observable selected: boolean;

	constructor(
		readonly id: string,
		readonly name: string | undefined,
		selected: boolean,
	) {
		makeObservable(this);

		this.selected = selected;
	}
}

export class ArtistRolesEditStore {
	@observable dialogVisible = false;
	readonly roleSelections: RoleSelection[];
	@observable selectedArtist?: IEditableArtistWithSupport = undefined;

	constructor(
		roleNames: { [key: string]: string | undefined },
		private readonly defaultRoleName: string,
	) {
		makeObservable(this);

		this.roleSelections = [];

		for (const role in roleNames) {
			if (role !== this.defaultRoleName && roleNames.hasOwnProperty(role)) {
				this.roleSelections.push(
					new RoleSelectionStore(role, roleNames[role], false),
				);
			}
		}

		this.roleSelections = this.roleSelections.sortBy((r) => r.name);
	}

	@action save = (): void => {
		if (!this.selectedArtist) return;

		var selectedRoles = this.roleSelections
			.filter((r) => r.selected)
			.map((r) => r.id);

		if (selectedRoles.length === 0) selectedRoles = [this.defaultRoleName];

		this.selectedArtist.rolesArray = selectedRoles;
		this.dialogVisible = false;
	};

	@action show = (artist: IEditableArtistWithSupport): void => {
		for (const r of this.roleSelections) {
			r.selected = artist && artist.rolesArray.includes(r.id);
		}

		this.selectedArtist = artist;
		this.dialogVisible = true;
	};
}

export class AlbumArtistRolesEditStore extends ArtistRolesEditStore {
	constructor(roleNames: { [key: string]: string | undefined }) {
		super(roleNames, ArtistRoles[ArtistRoles.Default]);
	}
}
