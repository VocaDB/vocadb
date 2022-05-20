import _ from 'lodash';
import { action, makeObservable, observable } from 'mobx';

import ArtistRoles from '../../Models/Artists/ArtistRoles';
import { IEditableArtistWithSupport } from '../ArtistForAlbumEditStore';

export interface RoleSelection {
	// Role Id, for example "VoiceManipulator"
	id: string;
	// User-visible role name, for example "Voice Manipulator"
	name: string;
	selected: boolean;
}

export class ArtistRolesEditStore {
	@observable public dialogVisible = false;
	public readonly roleSelections: RoleSelection[];
	@observable public selectedArtist?: IEditableArtistWithSupport = undefined;

	public constructor(
		roleNames: { [key: string]: string },
		private readonly defaultRoleName: string,
	) {
		makeObservable(this);

		this.roleSelections = [];

		for (const role in roleNames) {
			if (role !== this.defaultRoleName && roleNames.hasOwnProperty(role)) {
				this.roleSelections.push({
					id: role,
					name: roleNames[role],
					selected: false,
				});
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
		_.forEach(this.roleSelections, (r) => {
			r.selected = artist && _.includes(artist.rolesArray, r.id);
		});

		this.selectedArtist = artist;
		this.dialogVisible = true;
	};
}

export default class AlbumArtistRolesEditStore extends ArtistRolesEditStore {
	public constructor(roleNames: { [key: string]: string }) {
		super(roleNames, ArtistRoles[ArtistRoles.Default]);
	}
}
