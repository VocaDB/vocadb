import _ from 'lodash';
import { action, computed, makeObservable, observable } from 'mobx';
import moment from 'moment';

import ArtistForEventContract from '../../DataContracts/ReleaseEvents/ArtistForEventContract';
import ReleaseEventContract from '../../DataContracts/ReleaseEvents/ReleaseEventContract';
import SongListBaseContract from '../../DataContracts/SongListBaseContract';
import VenueForApiContract from '../../DataContracts/Venue/VenueForApiContract';
import ArtistEventRoles from '../../Models/Events/ArtistEventRoles';
import IEntryWithIdAndName from '../../Models/IEntryWithIdAndName';
import AlbumArtistRolesEditStore, {
	ArtistRolesEditStore,
} from '../Artist/AlbumArtistRolesEditStore';
import BasicEntryLinkStore from '../BasicEntryLinkStore';
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
	@observable public customName = false;
	// Event date. This should always be in UTC.
	@observable public date?: Date;
	@observable public defaultNameLanguage: string;
	@observable public description: string;
	@observable public endDate?: Date;
	public readonly id: number;
	@observable public isSeriesEvent: boolean;
	public readonly names: NamesEditStore;
	public readonly pvs: PVListEditStore;
	public readonly series: BasicEntryLinkStore<IEntryWithIdAndName>;
	public readonly songList: BasicEntryLinkStore<SongListBaseContract>;
	@observable public submitting = false;
	public readonly venue: BasicEntryLinkStore<VenueForApiContract>;
	public readonly webLinks: WebLinksEditStore;

	public constructor(
		artistRoleNames: { [key: string]: string },
		contract: ReleaseEventContract,
	) {
		makeObservable(this);

		this.artistRolesEditStore = new AlbumArtistRolesEditStore(artistRoleNames);
		this.artistLinks = _.map(
			contract.artists,
			(artist) => new ArtistForEventEditStore(artist),
		);
		this.id = contract.id;
		this.date = contract.date ? moment(contract.date).toDate() : undefined;
		this.endDate = contract.endDate
			? moment(contract.endDate).toDate()
			: undefined;

		this.defaultNameLanguage = contract.defaultNameLanguage;
		this.names = NamesEditStore.fromContracts(contract.names);
		this.pvs = new PVListEditStore(pvRepo);
		this.series = new BasicEntryLinkStore(
			// TODO: contract.series,
			undefined,
		);
		this.isSeriesEvent = !this.series.isEmpty;

		this.songList = new BasicEntryLinkStore(
			// TODO: contract.songList,
			undefined,
		);
		this.venue = new BasicEntryLinkStore(
			// TODO: contract.venue,
			undefined,
		);
		this.webLinks = new WebLinksEditStore(contract.webLinks);
	}

	@computed public get artistLinkContracts(): ArtistForEventContract[] {
		return this.artistLinks;
	}

	// Date as ISO string, in UTC, ready to be posted to server
	@computed public get dateStr(): string | undefined {
		return this.date?.toISOString() ?? undefined;
	}

	@computed public get endDateStr(): string | undefined {
		return this.endDate?.toISOString() ?? undefined;
	}

	@computed public get isSeriesEventStr(): string {
		return this.isSeriesEvent ? 'true' : 'false';
	}
	public set isSeriesEventStr(value: string) {
		this.isSeriesEvent = value === 'true';
	}

	@action public submit = (): boolean => {
		this.submitting = true;
		return true;
	};
}
