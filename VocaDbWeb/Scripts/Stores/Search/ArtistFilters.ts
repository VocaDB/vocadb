import ArtistHelper from '@Helpers/ArtistHelper';
import { ArtistAutoCompleteParams } from '@KnockoutExtensions/AutoCompleteParams';
import ArtistType from '@Models/Artists/ArtistType';
import ArtistRepository from '@Repositories/ArtistRepository';
import GlobalValues from '@Shared/GlobalValues';
import _ from 'lodash';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

import ArtistFilter from './ArtistFilter';

// Manages artist filters for search
// These can be used wherever artist filtering is needed - search page, rated songs page, song list page
export default class ArtistFilters {
	@observable public artists: ArtistFilter[] = [];
	@observable public artistParticipationStatus = 'Everything' /* TODO: enum */;
	public readonly artistSearchParams: ArtistAutoCompleteParams;
	@observable public childVoicebanks: boolean;
	@observable public includeMembers = false;

	public constructor(
		private readonly values: GlobalValues,
		private readonly artistRepo: ArtistRepository,
		childVoicebanks?: boolean,
	) {
		makeObservable(this);

		this.artistSearchParams = { acceptSelection: this.selectArtist };

		this.childVoicebanks = childVoicebanks || false;
	}

	@computed public get artistIds(): number[] {
		return _.map(this.artists, (a) => a.id);
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
			_.includes(ArtistHelper.groupTypes, this.firstArtist.artistType)
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

		const filters = _.map(selectedArtistIds, (a) => new ArtistFilter(a));
		this.artists.push(...filters);

		if (!this.artistRepo) return;

		_.forEach(filters, (newArtist) => {
			const selectedArtistId = newArtist.id;

			this.artistRepo
				.getOne({ id: selectedArtistId, lang: this.values.languagePreference })
				.then((artist) => {
					runInAction(() => {
						newArtist.name = artist.name;
						newArtist.artistType =
							ArtistType[artist.artistType as keyof typeof ArtistType];
					});
				});
		});
	};

	public selectArtist = (selectedArtistId?: number): void => {
		this.selectArtists([selectedArtistId!]);
	};

	@action public removeArtist = (artist: ArtistFilter): void => {
		_.pull(this.artists, artist);
	};
}
