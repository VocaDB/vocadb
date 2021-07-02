import SongApiContract from '@DataContracts/Song/SongApiContract';
import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import PVServiceIcons from '@Models/PVServiceIcons';
import SongRepository from '@Repositories/SongRepository';
import UserRepository from '@Repositories/UserRepository';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import ui from '@Shared/MessagesTyped';
import ObservableUrlParamRouter from '@Shared/Routing/ObservableUrlParamRouter';
import UrlMapper from '@Shared/UrlMapper';
import $ from 'jquery';
import ko from 'knockout';
import _ from 'lodash';

import SongWithPreviewViewModel from './SongWithPreviewViewModel';

export default class RankingsViewModel {
	public constructor(
		private urlMapper: UrlMapper,
		private songRepo: SongRepository,
		private userRepo: UserRepository,
		private languagePreference: ContentLanguagePreference,
	) {
		this.router = new ObservableUrlParamRouter({
			dateFilterType: this.dateFilterType,
			durationHours: this.durationHours,
			vocalistSelection: this.vocalistSelection,
		});

		this.dateFilterType.subscribe(this.getSongs);
		this.durationHours.subscribe(this.getSongs);
		this.vocalistSelection.subscribe(this.getSongs);
		this.pvServiceIcons = new PVServiceIcons(urlMapper);

		this.getSongs();
	}

	public dateFilterType = ko.observable('CreateDate');

	public durationHours = ko.observable(168);

	public getPVServiceIcons = (
		services: string,
	): { service: string; url: string }[] => {
		return this.pvServiceIcons.getIconUrls(services);
	};

	private getSongs = (): void => {
		$.getJSON(
			this.urlMapper.mapRelative('/api/songs/top-rated'),
			{
				durationHours: this.durationHours(),
				fields: 'AdditionalNames,ThumbUrl,Tags',
				vocalist: this.vocalistSelection(),
				filterBy: this.dateFilterType(),
				languagePreference: this.languagePreference,
			},
			(songs: SongApiContract[]) => {
				_.each(songs, (song: any) => {
					if (song.pvServices && song.pvServices !== 'Nothing') {
						song.previewViewModel = new SongWithPreviewViewModel(
							this.songRepo,
							this.userRepo,
							song.id,
							song.pvServices,
						);
						song.previewViewModel.ratingComplete =
							ui.showThankYouForRatingMessage;
					} else {
						song.previewViewModel = null;
					}
				});

				this.songs(songs);
			},
		);
	};

	public getTagUrl = (tag: TagUsageForApiContract): string => {
		return EntryUrlMapper.details_tag(tag.tag.id, tag.tag.urlSlug);
	};

	private pvServiceIcons: PVServiceIcons;

	private router: ObservableUrlParamRouter;

	public songs = ko.observableArray<SongApiContract>(null!);

	public vocalistSelection = ko.observable<string>(null!);
}
