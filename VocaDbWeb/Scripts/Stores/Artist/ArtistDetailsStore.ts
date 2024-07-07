import { CommentContract } from '@/DataContracts/CommentContract';
import { TagSelectionContract } from '@/DataContracts/Tag/TagSelectionContract';
import { TagUsageForApiContract } from '@/DataContracts/Tag/TagUsageForApiContract';
import { HighchartsHelper } from '@/Helpers/HighchartsHelper';
import { TimeUnit } from '@/Models/Aggregate/TimeUnit';
import { ArtistType } from '@/Models/Artists/ArtistType';
import { EntryType } from '@/Models/EntryType';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { LoginManager } from '@/Models/LoginManager';
import { TagTargetType } from '@/Models/Tags/TagTargetType';
import { AlbumRepository } from '@/Repositories/AlbumRepository';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { SongRepository } from '@/Repositories/SongRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { UrlMapper } from '@/Shared/UrlMapper';
import { ArtistAlbumsStore } from '@/Stores/Artist/ArtistAlbumsStore';
import { ArtistSongsStore } from '@/Stores/Artist/ArtistSongsStore';
import { EditableCommentsStore } from '@/Stores/EditableCommentsStore';
import { EnglishTranslatedStringStore } from '@/Stores/Globalization/EnglishTranslatedStringStore';
import { ReportEntryStore } from '@/Stores/ReportEntryStore';
import { TagListStore } from '@/Stores/Tag/TagListStore';
import { TagsEditStore } from '@/Stores/Tag/TagsEditStore';
import type { Options } from 'highcharts';
import { action, makeObservable, observable, runInAction } from 'mobx';

export class CustomizeArtistSubscriptionStore {
	@observable dialogVisible = false;
	@observable notificationsMethod: string /* TODO: enum */;

	constructor(emailNotifications: boolean, siteNotifications: boolean) {
		makeObservable(this);

		this.notificationsMethod = !siteNotifications
			? 'Nothing'
			: !emailNotifications
			? 'Site'
			: 'Email';
	}

	@action show = (): void => {
		this.dialogVisible = true;
	};
}

export class ArtistDetailsStore {
	readonly comments: EditableCommentsStore;
	readonly customizeSubscriptionDialog: CustomizeArtistSubscriptionStore;
	@observable hasArtistSubscription;
	@observable showAllMembers = false;
	readonly description: EnglishTranslatedStringStore;
	readonly songsStore: ArtistSongsStore;
	@observable songsOverTimeChart?: Options;
	readonly collaborationAlbumsStore: ArtistAlbumsStore;
	readonly mainAlbumsStore: ArtistAlbumsStore;
	readonly reportStore: ReportEntryStore;
	readonly tagsEditStore: TagsEditStore;
	readonly tagUsages: TagListStore;

	constructor(
		private readonly values: GlobalValues,
		loginManager: LoginManager,
		artistRepo: ArtistRepository,
		readonly artistId: number,
		tagUsages: TagUsageForApiContract[],
		hasSubscription: boolean,
		emailNotifications: boolean,
		siteNotifications: boolean,
		hasEnglishDescription: boolean,
		private readonly urlMapper: UrlMapper,
		private readonly albumRepo: AlbumRepository,
		private readonly songRepo: SongRepository,
		private readonly userRepo: UserRepository,
		canDeleteAllComments: boolean,
		latestComments: CommentContract[],
		artistType: ArtistType
	) {
		makeObservable(this);

		this.hasArtistSubscription = hasSubscription;

		this.customizeSubscriptionDialog = new CustomizeArtistSubscriptionStore(
			emailNotifications,
			siteNotifications,
		);

		this.description = new EnglishTranslatedStringStore(
			hasEnglishDescription &&
				(values.languagePreference === ContentLanguagePreference.English ||
					values.languagePreference === ContentLanguagePreference.Romaji),
		);

		this.comments = new EditableCommentsStore(
			loginManager,
			artistRepo,
			artistId,
			canDeleteAllComments,
			canDeleteAllComments,
			false,
			latestComments,
			true,
		);

		this.tagsEditStore = new TagsEditStore(
			{
				getTagSelections: (): Promise<TagSelectionContract[]> =>
					userRepo.getArtistTagSelections({ artistId: artistId }),
				saveTagSelections: (tags): Promise<void> =>
					userRepo
						.updateArtistTags({ artistId: artistId, tags: tags })
						.then(this.tagUsages.updateTagUsages),
			},
			EntryType.Artist,
			artistType,
			() => artistRepo.getTagSuggestions({ artistId: artistId }),
		);

		this.tagUsages = new TagListStore(tagUsages);

		this.reportStore = new ReportEntryStore((reportType, notes) => {
			return artistRepo.createReport({
				artistId: artistId,
				reportType: reportType,
				notes: notes,
				versionNumber: undefined,
			});
		});

		this.mainAlbumsStore = new ArtistAlbumsStore(this.values, this.albumRepo);
		this.mainAlbumsStore.artistFilters.artistParticipationStatus =
			'OnlyMainAlbums';

		this.collaborationAlbumsStore = new ArtistAlbumsStore(
			this.values,
			this.albumRepo,
		);
		this.collaborationAlbumsStore.artistFilters.artistParticipationStatus =
			'OnlyCollaborations';

		this.songsStore = new ArtistSongsStore(
			this.values,
			this.urlMapper,
			this.songRepo,
			this.userRepo,
		);
	}

	addFollowedArtist = (): Promise<void> => {
		return this.userRepo
			.createArtistSubscription({ artistId: this.artistId })
			.then(() => {
				runInAction(() => {
					this.hasArtistSubscription = true;
					this.customizeSubscriptionDialog.notificationsMethod = 'Site';
				});
			});
	};

	loadHighcharts = (): Promise<void> => {
		// Delayed load highcharts stuff
		const highchartsPromise = import('highcharts');
		const songsPerMonthDataPromise = this.songRepo.getOverTime({
			timeUnit: TimeUnit.month,
			artistId: this.artistId,
		});

		return Promise.all([songsPerMonthDataPromise, highchartsPromise]).then(
			([points]) => {
				// Need at least 2 points because lone point looks weird
				if (points && points.length >= 2) {
					runInAction(() => {
						this.songsOverTimeChart = HighchartsHelper.dateLineChartWithAverage(
							'Songs per month' /* LOC */,
							null!,
							'Songs' /* LOC */,
							points,
						);
					});
				}
			},
		);
	};

	removeFollowedArtist = (): Promise<void> => {
		return this.userRepo
			.deleteArtistSubscription({ artistId: this.artistId })
			.then(() => {
				runInAction(() => {
					this.hasArtistSubscription = false;
				});
			});
	};
}
