import { ArtistHelper } from '@/Helpers/ArtistHelper';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { ArtistFilter } from '@/Stores/Search/ArtistFilter';
import { pull } from 'lodash-es';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

// Manages artist filters for search
// These can be used wherever artist filtering is needed - search page, rated songs page, song list page
export class ArtistFilters {
	@observable artists: ArtistFilter[] = [];
	@observable artistParticipationStatus = 'Everything' /* TODO: enum */;
	@observable childVoicebanks = false;
	@observable includeMembers = false;

	constructor(
		private readonly values: GlobalValues,
		private readonly artistRepo: ArtistRepository,
	) {
		makeObservable(this);
	}

	@computed get artistIds(): number[] {
		return this.artists.map((a) => a.id);
	}
	set artistIds(value: number[]) {
		// OPTIMIZE
		this.artists = [];
		this.selectArtists(value);
	}

	@computed get hasMultipleArtists(): boolean {
		return this.artists.length > 1;
	}

	@computed get hasSingleArtist(): boolean {
		return this.artists.length === 1;
	}

	@computed get showChildVoicebanks(): boolean {
		return (
			this.hasSingleArtist &&
			ArtistHelper.canHaveChildVoicebanks(this.artists[0].artistType)
		);
	}

	@computed private get firstArtist(): ArtistFilter {
		return this.artists[0];
	}

	@computed get showMembers(): boolean {
		return (
			this.hasSingleArtist &&
			!!this.firstArtist.artistType &&
			ArtistHelper.groupTypes.includes(this.firstArtist.artistType)
		);
	}

	@computed get filters(): any {
		return {
			artistIds: this.artistIds,
			artistParticipationStatus: this.artistParticipationStatus,
			childVoicebanks: this.childVoicebanks,
			includeMembers: this.includeMembers,
		};
	}

	@action selectArtists = (selectedArtistIds?: number[]): void => {
		if (!selectedArtistIds) return;

		const filters = selectedArtistIds.map((a) => new ArtistFilter(a));
		this.artists.push(...filters);

		if (!this.artistRepo) return;

		for (const newArtist of filters) {
			const selectedArtistId = newArtist.id;

			this.artistRepo
				.getOne({ id: selectedArtistId, lang: this.values.languagePreference })
				.then((artist) => {
					runInAction(() => {
						newArtist.name = artist.name;
						newArtist.artistType = artist.artistType;
					});
				});
		}
	};

	selectArtist = (selectedArtistId?: number): void => {
		this.selectArtists([selectedArtistId!]);
	};

	@action removeArtist = (artist: ArtistFilter): void => {
		pull(this.artists, artist);
	};
}
