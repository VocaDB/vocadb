import ArtistForEventContract from '@DataContracts/ReleaseEvents/ArtistForEventContract';
import ReleaseEventForEditContract from '@DataContracts/ReleaseEvents/ReleaseEventForEditContract';
import SongListBaseContract from '@DataContracts/SongListBaseContract';
import VenueForApiContract from '@DataContracts/Venue/VenueForApiContract';
import ArtistEventRoles from '@Models/Events/ArtistEventRoles';
import IEntryWithIdAndName from '@Models/IEntryWithIdAndName';
import ArtistRepository from '@Repositories/ArtistRepository';
import PVRepository from '@Repositories/PVRepository';
import ReleaseEventRepository from '@Repositories/ReleaseEventRepository';
import SongListRepository from '@Repositories/SongListRepository';
import VenueRepository from '@Repositories/VenueRepository';
import GlobalValues from '@Shared/GlobalValues';
import UrlMapper from '@Shared/UrlMapper';
import _ from 'lodash';
import {
	action,
	computed,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';
import moment from 'moment';

import AlbumArtistRolesEditStore, {
	ArtistRolesEditStore,
} from '../Artist/AlbumArtistRolesEditStore';
import BasicEntryLinkStore from '../BasicEntryLinkStore';
import DeleteEntryStore from '../DeleteEntryStore';
import NamesEditStore from '../Globalization/NamesEditStore';
import PVListEditStore from '../PVs/PVListEditStore';
import WebLinksEditStore from '../WebLinksEditStore';
import ArtistForEventEditStore from './ArtistForEventEditStore';

export class EventArtistRolesEditStore extends ArtistRolesEditStore {
	public constructor(roleNames: { [key: string]: string }) {
		super(roleNames, ArtistEventRoles[ArtistEventRoles.Default]);
	}
}

export default class ReleaseEventEditStore {
	@observable public readonly artistLinks: ArtistForEventEditStore[];
	public readonly artistRolesEditStore: EventArtistRolesEditStore;
	@observable public category: string /* TODO: enum */;
	@observable public customName: boolean;
	// Event date. This should always be in UTC.
	@observable public date?: Date;
	@observable public defaultNameLanguage: string /* TODO: enum */;
	public readonly deleteStore = new DeleteEntryStore((notes) =>
		this.eventRepo.delete({
			id: this.contract.id,
			notes: notes,
			hardDelete: false,
		}),
	);
	@observable public description: string;
	@observable public endDate?: Date;
	@observable public errors?: Record<string, string[]>;
	@observable public isSeriesEvent: boolean;
	public readonly names: NamesEditStore;
	public readonly pvs: PVListEditStore;
	public readonly series: BasicEntryLinkStore<IEntryWithIdAndName>;
	@observable public seriesNumber: string;
	@observable public seriesSuffix: string;
	public readonly songList: BasicEntryLinkStore<SongListBaseContract>;
	@observable public status: string /* TODO: enum */;
	@observable public submitting = false;
	public readonly trashStore = new DeleteEntryStore((notes) =>
		this.eventRepo.delete({
			id: this.contract.id,
			notes: notes,
			hardDelete: true,
		}),
	);
	public readonly venue: BasicEntryLinkStore<VenueForApiContract>;
	@observable public venueName: string;
	public readonly webLinks: WebLinksEditStore;

	public constructor(
		private readonly values: GlobalValues,
		private readonly eventRepo: ReleaseEventRepository,
		private readonly artistRepo: ArtistRepository,
		pvRepo: PVRepository,
		songListRepo: SongListRepository,
		venueRepo: VenueRepository,
		urlMapper: UrlMapper,
		artistRoleNames: { [key: string]: string },
		public readonly contract: ReleaseEventForEditContract,
	) {
		makeObservable(this);

		this.artistRolesEditStore = new AlbumArtistRolesEditStore(artistRoleNames);
		this.series = new BasicEntryLinkStore((entryId) =>
			eventRepo.getOneSeries({ id: entryId }),
		);
		this.songList = new BasicEntryLinkStore((entryId) =>
			songListRepo.getOne({ id: entryId }),
		);
		this.venue = new BasicEntryLinkStore((entryId) =>
			venueRepo.getOne({ id: entryId }),
		);

		this.artistLinks = contract.artists.map(
			(artist) => new ArtistForEventEditStore(artist),
		);
		this.category = contract.category;
		this.customName = contract.customName;
		this.date = contract.date ? moment(contract.date).toDate() : undefined;
		this.defaultNameLanguage = contract.defaultNameLanguage;
		this.description = contract.description;
		this.endDate = contract.endDate
			? moment(contract.endDate).toDate()
			: undefined;

		this.names = NamesEditStore.fromContracts(contract.names);
		this.pvs = new PVListEditStore(
			pvRepo,
			urlMapper,
			contract.pvs,
			false,
			true,
			false,
		);
		this.series.id = contract.series?.id;
		this.seriesNumber = contract.seriesNumber.toString();
		this.seriesSuffix = contract.seriesSuffix;
		this.isSeriesEvent = !this.series.isEmpty;
		this.songList.id = contract.songList?.id;
		this.status = contract.status;
		this.venue.id = contract.venue?.id;
		this.venueName = contract.venueName ?? '';
		this.webLinks = new WebLinksEditStore(contract.webLinks);

		reaction(
			() => this.isSeriesEvent,
			(isSeriesEvent) => {
				if (!isSeriesEvent) this.series.clear();
			},
		);
	}

	@computed public get artistLinkContracts(): ArtistForEventContract[] {
		return this.artistLinks.map((artistLink) => artistLink.toContract());
	}

	// Date as ISO string, in UTC, ready to be posted to server
	@computed public get dateStr(): string | undefined {
		return this.date?.toISOString() ?? undefined;
	}

	@computed public get endDateStr(): string | undefined {
		return this.endDate?.toISOString() ?? undefined;
	}

	@action public addArtist = async (
		artistId?: number,
		customArtistName?: string,
	): Promise<void> => {
		if (artistId) {
			const artist = await this.artistRepo.getOne({
				id: artistId,
				lang: this.values.languagePreference,
			});

			const data: ArtistForEventContract = {
				artist: artist,
				name: artist.name,
				id: 0,
				roles: 'Default',
			};

			const link = new ArtistForEventEditStore(data);
			runInAction(() => {
				this.artistLinks.push(link);
			});
		} else {
			const data: ArtistForEventContract = {
				artist: undefined,
				name: customArtistName,
				id: 0,
				roles: 'Default',
			};

			const link = new ArtistForEventEditStore(data);
			this.artistLinks.push(link);
		}
	};

	public readonly artistSearchParams = {
		createNewItem: "Add custom artist named '{0}'" /* TODO: localize */,
		acceptSelection: this.addArtist,
	};

	public editArtistRoles = (artist: ArtistForEventEditStore): void => {
		this.artistRolesEditStore.show(artist);
	};

	@action public removeArtist = (artist: ArtistForEventEditStore): void => {
		_.pull(this.artistLinks, artist);
	};

	@action public submit = async (pictureUpload?: File): Promise<number> => {
		this.submitting = true;

		try {
			const id = await this.eventRepo.edit(
				{
					artists: this.artistLinkContracts,
					category: this.category,
					customName: this.customName,
					date: this.dateStr,
					defaultNameLanguage: this.defaultNameLanguage,
					deleted: false,
					description: this.description,
					endDate: this.endDateStr,
					id: this.contract.id,
					name: '',
					names: this.names.toContracts(),
					pvs: this.pvs.toContracts(),
					series: this.series.entry,
					seriesNumber: Number(this.seriesNumber),
					seriesSuffix: this.seriesSuffix,
					songList: this.songList.entry,
					status: this.status,
					venue: this.venue.entry,
					venueName: this.venueName,
					webLinks: this.webLinks.items,
				},
				pictureUpload,
			);

			return id;
		} catch (error: any) {
			if (error.response) {
				if (error.response.status === 400) {
					runInAction(() => {
						this.errors = error.response.data.errors;
					});
				}
			}

			throw error;
		} finally {
			runInAction(() => {
				this.submitting = false;
			});
		}
	};
}
