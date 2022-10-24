import { ArtistHelper } from '@/Helpers/ArtistHelper';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { ArtistFilter } from '@/Stores/Search/ArtistFilter';
import { pull } from 'lodash';
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
	@observable public artists: ArtistFilter[] = [];
	@observable public artistParticipationStatus = 'Everything' /* TODO: enum */;
	@observable public childVoicebanks = false;
	@observable public includeMembers = false;

	public constructor(
		private readonly values: GlobalValues,
		private readonly artistRepo: ArtistRepository,
	) {
		makeObservable(this);
	}

	@computed public get artistIds(): number[] {
		return this.artists.map((a) => a.id);
	}
	public set artistIds(value: number[]) {
		// OPTIMIZE
		this.artists = [];
		this.selectArtists(value);
	}

	@computed public get hasMultipleArtists(): boolean {
		return this.artists.length > 1;
	}

	@computed public get hasSingleArtist(): boolean {
		return this.artists.length === 1;
	}

	@computed public get showChildVoicebanks(): boolean {
		return (
			this.hasSingleArtist &&
			ArtistHelper.canHaveChildVoicebanks(this.artists[0].artistType)
		);
	}

	@computed private get firstArtist(): ArtistFilter {
		return this.artists[0];
	}

	@computed public get showMembers(): boolean {
		return (
			this.hasSingleArtist &&
			!!this.firstArtist.artistType &&
			ArtistHelper.groupTypes.includes(this.firstArtist.artistType)
		);
	}

	@computed public get filters(): any {
		return {
			artistIds: this.artistIds,
			artistParticipationStatus: this.artistParticipationStatus,
			childVoicebanks: this.childVoicebanks,
			includeMembers: this.includeMembers,
		};
	}

	@action public selectArtists = (selectedArtistIds?: number[]): void => {
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

	public selectArtist = (selectedArtistId?: number): void => {
		this.selectArtists([selectedArtistId!]);
	};

	@action public removeArtist = (artist: ArtistFilter): void => {
		pull(this.artists, artist);
	};
}
