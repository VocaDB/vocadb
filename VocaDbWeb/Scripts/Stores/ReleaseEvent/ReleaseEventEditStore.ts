import { ArtistForEventContract } from '@/DataContracts/ReleaseEvents/ArtistForEventContract';
import { ReleaseEventForEditContract } from '@/DataContracts/ReleaseEvents/ReleaseEventForEditContract';
import { SongListBaseContract } from '@/DataContracts/SongListBaseContract';
import { VenueForApiContract } from '@/DataContracts/Venue/VenueForApiContract';
import { EntryStatus } from '@/Models/EntryStatus';
import { ArtistEventRoles } from '@/Models/Events/ArtistEventRoles';
import { EventCategory } from '@/Models/Events/EventCategory';
import { ContentLanguageSelection } from '@/Models/Globalization/ContentLanguageSelection';
import { IEntryWithIdAndName } from '@/Models/IEntryWithIdAndName';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { PVRepository } from '@/Repositories/PVRepository';
import { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';
import { SongListRepository } from '@/Repositories/SongListRepository';
import { VenueRepository } from '@/Repositories/VenueRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { UrlMapper } from '@/Shared/UrlMapper';
import {
	AlbumArtistRolesEditStore,
	ArtistRolesEditStore,
} from '@/Stores/Artist/AlbumArtistRolesEditStore';
import { BasicEntryLinkStore } from '@/Stores/BasicEntryLinkStore';
import { DeleteEntryStore } from '@/Stores/DeleteEntryStore';
import { NamesEditStore } from '@/Stores/Globalization/NamesEditStore';
import { PVListEditStore } from '@/Stores/PVs/PVListEditStore';
import { ArtistForEventEditStore } from '@/Stores/ReleaseEvent/ArtistForEventEditStore';
import { WebLinksEditStore } from '@/Stores/WebLinksEditStore';
import dayjs from 'dayjs';
import { pull } from 'lodash-es';
import {
	action,
	computed,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';

export class EventArtistRolesEditStore extends ArtistRolesEditStore {
	constructor(roleNames: { [key: string]: string | undefined }) {
		super(roleNames, ArtistEventRoles[ArtistEventRoles.Default]);
	}
}

export class ReleaseEventEditStore {
	@observable readonly artistLinks: ArtistForEventEditStore[];
	readonly artistRolesEditStore: EventArtistRolesEditStore;
	@observable category: EventCategory;
	@observable customName: boolean;
	// Event date. This should always be in UTC.
	@observable date?: Date;
	@observable defaultNameLanguage: ContentLanguageSelection;
	readonly deleteStore: DeleteEntryStore;
	@observable description: string;
	@observable endDate?: Date;
	@observable errors?: Record<string, string[]>;
	@observable isSeriesEvent: boolean;
	readonly names: NamesEditStore;
	readonly pvs: PVListEditStore;
	readonly series: BasicEntryLinkStore<IEntryWithIdAndName>;
	@observable seriesNumber: string;
	@observable seriesSuffix: string;
	readonly songList: BasicEntryLinkStore<SongListBaseContract>;
	@observable status: EntryStatus;
	@observable submitting = false;
	readonly trashStore: DeleteEntryStore;
	readonly venue: BasicEntryLinkStore<VenueForApiContract>;
	@observable venueName: string;
	readonly webLinks: WebLinksEditStore;

	constructor(
		private readonly values: GlobalValues,
		private readonly eventRepo: ReleaseEventRepository,
		private readonly artistRepo: ArtistRepository,
		pvRepo: PVRepository,
		songListRepo: SongListRepository,
		venueRepo: VenueRepository,
		urlMapper: UrlMapper,
		artistRoleNames: { [key: string]: string | undefined },
		readonly contract: ReleaseEventForEditContract,
	) {
		makeObservable(this);

		this.deleteStore = new DeleteEntryStore((notes) =>
			this.eventRepo.delete({
				id: this.contract.id,
				notes: notes,
				hardDelete: false,
			}),
		);

		this.trashStore = new DeleteEntryStore((notes) =>
			this.eventRepo.delete({
				id: this.contract.id,
				notes: notes,
				hardDelete: true,
			}),
		);

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
		this.date = contract.date ? dayjs(contract.date).toDate() : undefined;
		this.defaultNameLanguage = contract.defaultNameLanguage;
		this.description = contract.description;
		this.endDate = contract.endDate
			? dayjs(contract.endDate).toDate()
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

	@computed get artistLinkContracts(): ArtistForEventContract[] {
		return this.artistLinks.map((artistLink) => artistLink.toContract());
	}

	// Date as ISO string, in UTC, ready to be posted to server
	@computed get dateStr(): string | undefined {
		return this.date?.toISOString() ?? undefined;
	}

	@computed get endDateStr(): string | undefined {
		return this.endDate?.toISOString() ?? undefined;
	}

	@action addArtist = async (
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

	editArtistRoles = (artist: ArtistForEventEditStore): void => {
		this.artistRolesEditStore.show(artist);
	};

	@action removeArtist = (artist: ArtistForEventEditStore): void => {
		pull(this.artistLinks, artist);
	};

	@action submit = async (pictureUpload: File | undefined): Promise<number> => {
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
				runInAction(() => {
					this.errors = undefined;

					if (error.response.status === 400)
						this.errors = error.response.data.errors;
				});
			}

			throw error;
		} finally {
			runInAction(() => {
				this.submitting = false;
			});
		}
	};
}
