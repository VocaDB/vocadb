import ArtistApiContract from '@DataContracts/Artist/ArtistApiContract';
import ArtistHelper from '@Helpers/ArtistHelper';
import ArtistType from '@Models/Artists/ArtistType';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import ArtistRepository from '@Repositories/ArtistRepository';
import ko from 'knockout';

import SearchCategoryBaseViewModel from './SearchCategoryBaseViewModel';
import SearchViewModel from './SearchViewModel';

export default class ArtistSearchViewModel extends SearchCategoryBaseViewModel<ArtistApiContract> {
	constructor(
		searchViewModel: SearchViewModel,
		lang: ContentLanguagePreference,
		private readonly artistRepo: ArtistRepository,
		private readonly loggedUserId: number,
		artistType: string,
	) {
		super(searchViewModel);

		if (artistType) this.artistType(artistType);

		this.advancedFilters.filters.subscribe(this.updateResultsWithTotalCount);
		this.sort.subscribe(this.updateResultsWithTotalCount);
		this.artistType.subscribe(this.updateResultsWithTotalCount);
		this.onlyFollowedByMe.subscribe(this.updateResultsWithTotalCount);
		this.onlyRootVoicebanks.subscribe(this.updateResultsWithTotalCount);

		this.loadResults = (
			pagingProperties,
			searchTerm,
			tags,
			childTags,
			status,
			callback,
		): void => {
			this.artistRepo
				.getList(
					pagingProperties,
					lang,
					searchTerm,
					this.sort(),
					this.artistType() !== ArtistType[ArtistType.Unknown]
						? this.artistType()
						: null!,
					!this.onlyRootVoicebanks(),
					tags,
					childTags,
					this.onlyFollowedByMe() ? this.loggedUserId : null!,
					this.fields(),
					status,
					this.advancedFilters.filters(),
				)
				.then(callback);
		};
	}

	public artistTypeName = (artist: ArtistApiContract): string => {
		return this.searchViewModel.resources().artistTypeNames![artist.artistType];
	};

	public artistType = ko.observable('Unknown');
	public onlyFollowedByMe = ko.observable(false);
	public onlyRootVoicebanks = ko.observable(false);
	public showTags = ko.observable(false);
	public sort = ko.observable('Name');
	public sortName = ko.computed(() =>
		this.searchViewModel.resources().artistSortRuleNames != null
			? this.searchViewModel.resources().artistSortRuleNames![this.sort()]
			: '',
	);

	public canHaveChildVoicebanks = ko.computed(() =>
		ArtistHelper.canHaveChildVoicebanks(
			ArtistType[this.artistType() as keyof typeof ArtistType],
		),
	);

	public fields = ko.computed(() =>
		this.searchViewModel.showTags()
			? 'AdditionalNames,MainPicture,Tags'
			: 'AdditionalNames,MainPicture',
	);
}
