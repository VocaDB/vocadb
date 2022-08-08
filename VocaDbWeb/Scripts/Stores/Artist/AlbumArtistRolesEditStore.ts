import ArtistRoles from '@/Models/Artists/ArtistRoles';
import _ from 'lodash';
import { action, makeObservable, observable } from 'mobx';

import { IEditableArtistWithSupport } from '../ArtistForAlbumEditStore';

export interface RoleSelection {
	// Role Id, for example "VoiceManipulator"
	id: string;
	// User-visible role name, for example "Voice Manipulator"
	name?: string;
	selected: boolean;
}

class RoleSelectionStore implements RoleSelection {
	@observable public selected: boolean;

	public constructor(
		public readonly id: string,
		public readonly name: string | undefined,
		selected: boolean,
	) {
		makeObservable(this);

		this.selected = selected;
	}
}

export class ArtistRolesEditStore {
	@observable public dialogVisible = false;
	public readonly roleSelections: RoleSelection[];
	@observable public selectedArtist?: IEditableArtistWithSupport = undefined;

	public constructor(
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

		this.roleSelections = _.sortBy(this.roleSelections, (r) => r.name);
	}

	@action public save = (): void => {
		if (!this.selectedArtist) return;

		var selectedRoles = _.chain(this.roleSelections)
			.filter((r) => r.selected)
			.map((r) => r.id)
			.value();

		if (selectedRoles.length === 0) selectedRoles = [this.defaultRoleName];

		this.selectedArtist.rolesArray = selectedRoles;
		this.dialogVisible = false;
	};

	@action public show = (artist: IEditableArtistWithSupport): void => {
		for (const r of this.roleSelections) {
			r.selected = artist && artist.rolesArray.includes(r.id);
		}

		this.selectedArtist = artist;
		this.dialogVisible = true;
	};
}

export default class AlbumArtistRolesEditStore extends ArtistRolesEditStore {
	public constructor(roleNames: { [key: string]: string | undefined }) {
		super(roleNames, ArtistRoles[ArtistRoles.Default]);
	}
}
