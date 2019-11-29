
//namespace vdb.viewModels {

    import dc = vdb.dataContracts;
    import rep = vdb.repositories;

    // View model for editing artist for album link.
    export class ArtistForAlbumEditViewModel implements IEditableArtistWithSupport {
        
		public artist: dc.ArtistContract;

        // Unique link Id.
        public id: number;

        // Whether the roles of this artist can be customized.
		public isCustomizable: KnockoutComputed<boolean>;

		public isCustomName: boolean;

        public isSupport: KnockoutObservable<boolean>;

		public name: KnockoutObservable<string>;

		public nameDialogVisible = ko.observable(false);

        // Roles as comma-separated string (for serializing to and from .NET enum for the server)
        public roles: KnockoutComputed<string>;

        // List of roles for this artist.
		public rolesArray: KnockoutObservableArray<string>;

	    public rolesArrayTyped: KnockoutComputed<models.artists.ArtistRoles[]>;

		public toContract: () => dc.ArtistForAlbumContract = () => {
			return {
				artist: this.artist,
				id: this.id,
				isCustomName: this.isCustomName,
				isSupport: this.isSupport(),
				name: this.name(),
				roles: this.roles()
			};
		}

        constructor(repository: rep.AlbumRepository, data: dc.ArtistForAlbumContract) {

            this.artist = data.artist;
			this.id = data.id;
			this.isCustomName = data.isCustomName;
            this.isSupport = ko.observable(data.isSupport);

            this.name = ko.observable(data.name);
            this.rolesArray = ko.observableArray<string>([]);

            this.isCustomizable = ko.computed(() => {
                return !this.artist || helpers.ArtistHelper.isCustomizable(this.artist.artistType);
            });

            this.roles = ko.computed({
                read: () => {
                    return this.rolesArray().join();
                },
                write: (value: string) => {
                    this.rolesArray(_.map(value.split(","), val => val.trim()));
                }
            });

            this.roles(data.roles);
			this.rolesArrayTyped = ko.pureComputed(() => helpers.ArtistHelper.getRolesArray(this.rolesArray()));
        
        }
    
	}

	export interface IEditableArtistWithSupport {
		
		rolesArray: KnockoutObservableArray<string>;

	}

//}