import { ArtistRoles } from '@/Models/Artists/ArtistRoles';
import { IEditableArtistWithSupport } from '@/ViewModels/ArtistForAlbumEditViewModel';
import ko, { Observable } from 'knockout';
import _ from 'lodash';

export class ArtistRolesEditViewModel {
	public constructor(
		roleNames: { [key: string]: string },
		private readonly defaultRoleName: string,
	) {
		this.roleSelections = [];

		for (var role in roleNames) {
			if (role !== this.defaultRoleName && roleNames.hasOwnProperty(role)) {
				this.roleSelections.push({
					id: role,
					name: roleNames[role],
					selected: ko.observable(false),
				});
			}
		}

		this.roleSelections = _.sortBy(this.roleSelections, (r) => r.name);
	}

	public dialogVisible = ko.observable(false);

	public roleSelections: RoleSelection[];

	public save = (): void => {
		if (!this.selectedArtist()) return;

		var selectedRoles = _.chain(this.roleSelections)
			.filter((r) => r.selected())
			.map((r) => r.id)
			.value();

		if (selectedRoles.length === 0) selectedRoles = [this.defaultRoleName];

		this.selectedArtist()!.rolesArray(selectedRoles);
		this.dialogVisible(false);
	};

	public selectedArtist = ko.observable<IEditableArtistWithSupport>(null!);

	public show = (artist: IEditableArtistWithSupport): void => {
		for (const r of this.roleSelections) {
			r.selected(artist && artist.rolesArray().includes(r.id));
		}

		this.selectedArtist(artist);
		this.dialogVisible(true);
	};
}

export class AlbumArtistRolesEditViewModel extends ArtistRolesEditViewModel {
	public constructor(roleNames: { [key: string]: string }) {
		super(roleNames, ArtistRoles[ArtistRoles.Default]);
	}
}

export interface RoleSelection {
	// Role Id, for example "VoiceManipulator"
	id: string;

	// User-visible role name, for example "Voice Manipulator"
	name: string;

	selected: Observable<boolean>;
}
