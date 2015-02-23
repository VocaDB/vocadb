
module vdb.viewModels.artists {
	
	export class ArtistRolesEditViewModel {
		
		constructor(roleNames: string[]) {
			
			this.roleSelections = [];

			for (var role in roleNames) {
				if (role !== models.artists.ArtistRoles[models.artists.ArtistRoles.Default] && roleNames.hasOwnProperty(role)) {
					this.roleSelections.push({ id: role, name: roleNames[role], selected: ko.observable(false) });								
				}
			}

			this.roleSelections = _.sortBy(this.roleSelections, r => r.name);

		}

		public dialogVisible = ko.observable(false);

		public roleSelections: RoleSelection[];

		public save = () => {
			
			if (!this.selectedArtist())
				return;

			var selectedRoles = _.chain(this.roleSelections).filter(r => r.selected()).map(r => r.id).value();

			if (selectedRoles.length === 0)
				selectedRoles = ['Default'];

			this.selectedArtist().rolesArray(selectedRoles);
			this.dialogVisible(false);

		}

		public selectedArtist = ko.observable<vdb.viewModels.IEditableArtistWithSupport>(null);

		public show = (artist: ArtistForAlbumEditViewModel) => {
			
			_.forEach(this.roleSelections, r => {
				r.selected(artist && _.contains(artist.rolesArray(), r.id));
			});

			this.selectedArtist(artist);
			this.dialogVisible(true);

		}

	}

	export interface RoleSelection {

		id: string;
		
		name: string;

		selected: KnockoutObservable<boolean>;

	}

} 