
module vdb.viewModels.releaseEvents {

	import dc = vdb.dataContracts;
	import rep = repositories;

	export class ReleaseEventEditViewModel {

		constructor(
			private readonly repo: rep.ReleaseEventRepository,
			userRepository: rep.UserRepository,
			pvRepository: rep.PVRepository,
			private readonly artistRepository: rep.ArtistRepository,
			private readonly urlMapper: vdb.UrlMapper,
			private readonly artistRoleNames: { [key: string]: string; },
			contract: dc.ReleaseEventContract) {

			this.artistRolesEditViewModel = new artists.AlbumArtistRolesEditViewModel(artistRoleNames);
			this.artistLinks = ko.observableArray(_.map(contract.artists, artist => new events.ArtistForEventEditViewModel(artist)));
			this.id = contract.id;
			this.date = ko.observable(contract.date ? moment(contract.date).toDate() : null);
			this.dateStr = ko.computed(() => (this.date() ? this.date().toISOString() : null));

			this.defaultNameLanguage = ko.observable(contract.defaultNameLanguage);
			this.names = globalization.NamesEditViewModel.fromContracts(contract.names);
			this.pvs = new pvs.PVListEditViewModel(pvRepository, urlMapper, contract.pvs, false, true);
			this.series = new BasicEntryLinkViewModel(contract.series, null);
			this.isSeriesEvent = ko.observable(!this.series.isEmpty());

			this.isSeriesEventStr = ko.computed<string>({
				read: () => this.isSeriesEvent() ? "true" : "false",
				write: (val) => this.isSeriesEvent(val === "true")
			});

			this.isSeriesEvent.subscribe(val => {
				if (!val)
					this.series.clear();
			});

			this.songList = new BasicEntryLinkViewModel(contract.songList, null);
			this.webLinks = new WebLinksEditViewModel(contract.webLinks);

			this.artistLinkContracts = ko.computed(() => ko.toJS(this.artistLinks()));

			if (contract.id) {
				window.setInterval(() => userRepository.refreshEntryEdit(models.EntryType.ReleaseEvent, contract.id), 10000);				
			}

		}

		addArtist = (artistId?: number, customArtistName?: string) => {

			if (artistId) {

				this.artistRepository.getOne(artistId, artist => {

					const data: dc.events.ArtistForEventContract = {
						artist: artist,
						name: artist.name,
						id: 0,
						roles: 'Default'
					};

					const link = new events.ArtistForEventEditViewModel(data);
					this.artistLinks.push(link);

				});

			} else {

				const data: dc.events.ArtistForEventContract = {
					artist: null,
					name: customArtistName,
					id: 0,
					roles: 'Default'
				};

				const link = new events.ArtistForEventEditViewModel(data);
				this.artistLinks.push(link);

			}

		};

		public artistLinks: KnockoutObservableArray<events.ArtistForEventEditViewModel>;

		public artistLinkContracts: KnockoutComputed<dc.events.ArtistForEventContract[]>;

		public artistRolesEditViewModel: EventArtistRolesEditViewModel;

		public artistSearchParams = {
			createNewItem: "Add custom artist named '{0}'",
			acceptSelection: this.addArtist
		};

		public customName = ko.observable(false);

		// Event date. This should always be in UTC.
		public date: KnockoutObservable<Date>;

		// Date as ISO string, in UTC, ready to be posted to server
		public dateStr: KnockoutComputed<string>;

		public defaultNameLanguage: KnockoutObservable<string>;

		public deleteViewModel = new DeleteEntryViewModel(notes => {
			this.repo.delete(this.id, notes, () => {
				window.location.href = this.urlMapper.mapRelative(utils.EntryUrlMapper.details(models.EntryType.ReleaseEvent, this.id));
			});
		});

		public description = ko.observable<string>();

		public editArtistRoles = (artist: events.ArtistForEventEditViewModel) => {
			this.artistRolesEditViewModel.show(artist);
		}

		private id: number;

		public isSeriesEvent: KnockoutObservable<boolean>;

		public isSeriesEventStr: KnockoutComputed<string>;

		public names: globalization.NamesEditViewModel;
		public pvs: pvs.PVListEditViewModel;

		public removeArtist = (artist: events.ArtistForEventEditViewModel) => {
			this.artistLinks.remove(artist);
		};

		public series: BasicEntryLinkViewModel<models.IEntryWithIdAndName>;

		public songList: BasicEntryLinkViewModel<dc.SongListBaseContract>;

		public submit = () => {
			this.submitting(true);
			return true;
		}

		public submitting = ko.observable(false);

		public translateArtistRole = (role: string) => {
			return this.artistRoleNames[role];
		};

        public webLinks: WebLinksEditViewModel;

	}

	export class EventArtistRolesEditViewModel extends artists.ArtistRolesEditViewModel {

		constructor(roleNames: { [key: string]: string; }) {
			super(roleNames, models.events.ArtistEventRoles[models.events.ArtistEventRoles.Default]);
		}

	}

}