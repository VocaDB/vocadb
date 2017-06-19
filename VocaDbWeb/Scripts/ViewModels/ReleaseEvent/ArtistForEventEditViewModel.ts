
namespace vdb.viewModels.events {

	import dc = vdb.dataContracts;

	// View model for editing artist for album link.
	export class ArtistForEventEditViewModel implements IEditableArtistWithSupport {

		public artist: dc.ArtistContract;

		// Unique link Id.
		public id: number;

		public name: string;

		// Roles as comma-separated string (for serializing to and from .NET enum for the server)
		public roles: KnockoutComputed<string>;

		// List of roles for this artist.
		public rolesArray: KnockoutObservableArray<string>;

		public toContract: () => dc.events.ArtistForEventContract = () => {
			return {
				artist: this.artist,
				id: this.id,
				name: this.name,
				roles: this.roles()
			};
		}

		constructor(data: dc.events.ArtistForEventContract) {

			this.artist = data.artist;
			this.id = data.id;

			this.name = data.name;
			this.rolesArray = ko.observableArray<string>([]);

			this.roles = ko.computed({
				read: () => {
					return this.rolesArray().join();
				},
				write: (value: string) => {
					this.rolesArray(_.map(value.split(","), val => val.trim()));
				}
			});

			this.roles(data.roles);

		}

	}

	export interface IEditableArtistWithSupport {

		rolesArray: KnockoutObservableArray<string>;

	}

}