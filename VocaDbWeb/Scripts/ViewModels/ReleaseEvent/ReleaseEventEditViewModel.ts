
import { AlbumArtistRolesEditViewModel } from '../Artist/ArtistRolesEditViewModel';
import ArtistEventRoles from '../../Models/Events/ArtistEventRoles';
import ArtistForEventContract from '../../DataContracts/ReleaseEvents/ArtistForEventContract';
import ArtistForEventEditViewModel from '../ReleaseEvent/ArtistForEventEditViewModel';
import ArtistRepository from '../../Repositories/ArtistRepository';
import ArtistRolesEditViewModel from '../Artist/ArtistRolesEditViewModel';
import BasicEntryLinkViewModel from '../BasicEntryLinkViewModel';
import DeleteEntryViewModel from '../DeleteEntryViewModel';
import EntryType from '../../Models/EntryType';
import EntryUrlMapper from '../../Shared/EntryUrlMapper';
import IEntryWithIdAndName from '../../Models/IEntryWithIdAndName';
import NamesEditViewModel from '../Globalization/NamesEditViewModel';
import PVListEditViewModel from '../PVs/PVListEditViewModel';
import PVRepository from '../../Repositories/PVRepository';
import ReleaseEventContract from '../../DataContracts/ReleaseEvents/ReleaseEventContract';
import ReleaseEventRepository from '../../Repositories/ReleaseEventRepository';
import SongListBaseContract from '../../DataContracts/SongListBaseContract';
import UrlMapper from '../../Shared/UrlMapper';
import UserRepository from '../../Repositories/UserRepository';
import WebLinksEditViewModel from '../WebLinksEditViewModel';

//module vdb.viewModels.releaseEvents {

	export default class ReleaseEventEditViewModel {

		constructor(
			private readonly repo: ReleaseEventRepository,
			userRepository: UserRepository,
			pvRepository: PVRepository,
			private readonly artistRepository: ArtistRepository,
			private readonly urlMapper: UrlMapper,
			private readonly artistRoleNames: { [key: string]: string; },
			contract: ReleaseEventContract) {

			this.artistRolesEditViewModel = new AlbumArtistRolesEditViewModel(artistRoleNames);
			this.artistLinks = ko.observableArray(_.map(contract.artists, artist => new ArtistForEventEditViewModel(artist)));
			this.id = contract.id;
			this.date = ko.observable(contract.date ? moment(contract.date).toDate() : null);
			this.dateStr = ko.computed(() => (this.date() ? this.date().toISOString() : null));
			this.endDate = ko.observable(contract.endDate ? moment(contract.endDate).toDate() : null);
			this.endDateStr = ko.computed(() => (this.endDate() ? this.endDate().toISOString() : null));

			this.defaultNameLanguage = ko.observable(contract.defaultNameLanguage);
			this.names = NamesEditViewModel.fromContracts(contract.names);
			this.pvs = new PVListEditViewModel(pvRepository, urlMapper, contract.pvs, false, true, false);
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
				window.setInterval(() => userRepository.refreshEntryEdit(EntryType.ReleaseEvent, contract.id), 10000);				
			}

		}

		addArtist = (artistId?: number, customArtistName?: string) => {

			if (artistId) {

				this.artistRepository.getOne(artistId, artist => {

					const data: ArtistForEventContract = {
						artist: artist,
						name: artist.name,
						id: 0,
						roles: 'Default'
					};

					const link = new ArtistForEventEditViewModel(data);
					this.artistLinks.push(link);

				});

			} else {

				const data: ArtistForEventContract = {
					artist: null,
					name: customArtistName,
					id: 0,
					roles: 'Default'
				};

				const link = new ArtistForEventEditViewModel(data);
				this.artistLinks.push(link);

			}

		};

		public artistLinks: KnockoutObservableArray<ArtistForEventEditViewModel>;

		public artistLinkContracts: KnockoutComputed<ArtistForEventContract[]>;

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
			this.repo.delete(this.id, notes, false, this.redirectToDetails);
		});

		public description = ko.observable<string>();

		public editArtistRoles = (artist: ArtistForEventEditViewModel) => {
			this.artistRolesEditViewModel.show(artist);
		}

		public endDate: KnockoutObservable<Date>;

		public endDateStr: KnockoutComputed<string>;

		private id: number;

		public isSeriesEvent: KnockoutObservable<boolean>;

		public isSeriesEventStr: KnockoutComputed<string>;

		public names: NamesEditViewModel;
		public pvs: PVListEditViewModel;

		private redirectToDetails = () => {
			window.location.href = this.urlMapper.mapRelative(EntryUrlMapper.details(EntryType.ReleaseEvent, this.id));
		}

		private redirectToRoot = () => {
			window.location.href = this.urlMapper.mapRelative("Event");
		}

		public removeArtist = (artist: ArtistForEventEditViewModel) => {
			this.artistLinks.remove(artist);
		};

		public series: BasicEntryLinkViewModel<IEntryWithIdAndName>;

		public songList: BasicEntryLinkViewModel<SongListBaseContract>;

		public submit = () => {
			this.submitting(true);
			return true;
		}

		public submitting = ko.observable(false);

		public translateArtistRole = (role: string) => {
			return this.artistRoleNames[role];
		};

		public trashViewModel = new DeleteEntryViewModel(notes => {
			this.repo.delete(this.id, notes, true, this.redirectToRoot);
		});

        public webLinks: WebLinksEditViewModel;

	}

	export class EventArtistRolesEditViewModel extends ArtistRolesEditViewModel {

		constructor(roleNames: { [key: string]: string; }) {
			super(roleNames, ArtistEventRoles[ArtistEventRoles.Default]);
		}

	}

//}