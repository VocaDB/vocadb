import CommentContract from '@/DataContracts/CommentContract';
import TagSelectionContract from '@/DataContracts/Tag/TagSelectionContract';
import TagUsageForApiContract from '@/DataContracts/Tag/TagUsageForApiContract';
import HighchartsHelper from '@/Helpers/HighchartsHelper';
import TimeUnit from '@/Models/Aggregate/TimeUnit';
import EntryType from '@/Models/EntryType';
import ContentLanguagePreference from '@/Models/Globalization/ContentLanguagePreference';
import LoginManager from '@/Models/LoginManager';
import AlbumRepository from '@/Repositories/AlbumRepository';
import ArtistRepository from '@/Repositories/ArtistRepository';
import SongRepository from '@/Repositories/SongRepository';
import UserRepository from '@/Repositories/UserRepository';
import GlobalValues from '@/Shared/GlobalValues';
import UrlMapper from '@/Shared/UrlMapper';
import ArtistAlbumsStore from '@/Stores/Artist/ArtistAlbumsStore';
import ArtistSongsStore from '@/Stores/Artist/ArtistSongsStore';
import EditableCommentsStore from '@/Stores/EditableCommentsStore';
import EnglishTranslatedStringStore from '@/Stores/Globalization/EnglishTranslatedStringStore';
import PVPlayersFactory from '@/Stores/PVs/PVPlayersFactory';
import ReportEntryStore from '@/Stores/ReportEntryStore';
import TagListStore from '@/Stores/Tag/TagListStore';
import TagsEditStore from '@/Stores/Tag/TagsEditStore';
import { Options } from 'highcharts';
import { action, makeObservable, observable, runInAction } from 'mobx';

export class CustomizeArtistSubscriptionStore {
	@observable public dialogVisible = false;
	@observable public notificationsMethod: string /* TODO: enum */;

	public constructor(emailNotifications: boolean, siteNotifications: boolean) {
		makeObservable(this);

		this.notificationsMethod = !siteNotifications
			? 'Nothing'
			: !emailNotifications
			? 'Site'
			: 'Email';
	}

	@action public show = (): void => {
		this.dialogVisible = true;
	};
}

export default class ArtistDetailsStore {
	public readonly comments: EditableCommentsStore;
	public readonly customizeSubscriptionDialog: CustomizeArtistSubscriptionStore;
	@observable public hasArtistSubscription;
	@observable public showAllMembers = false;
	public readonly description: EnglishTranslatedStringStore;
	public readonly songsStore: ArtistSongsStore;
	@observable public songsOverTimeChart?: Options;
	public readonly collaborationAlbumsStore: ArtistAlbumsStore;
	public readonly mainAlbumsStore: ArtistAlbumsStore;
	public readonly reportStore: ReportEntryStore;
	public readonly tagsEditStore: TagsEditStore;
	public readonly tagUsages: TagListStore;

	public constructor(
		private readonly values: GlobalValues,
		loginManager: LoginManager,
		artistRepo: ArtistRepository,
		public readonly artistId: number,
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
		private readonly pvPlayersFactory: PVPlayersFactory,
		latestComments: CommentContract[],
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
			this.pvPlayersFactory,
		);
	}

	public addFollowedArtist = (): Promise<void> => {
		return this.userRepo
			.createArtistSubscription({ artistId: this.artistId })
			.then(() => {
				runInAction(() => {
					this.hasArtistSubscription = true;
					this.customizeSubscriptionDialog.notificationsMethod = 'Site';
				});
			});
	};

	public loadHighcharts = (): Promise<void> => {
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
							'Songs per month' /* TODO: localize */,
							null!,
							'Songs' /* TODO: localize */,
							points,
						);
					});
				}
			},
		);
	};

	public removeFollowedArtist = (): Promise<void> => {
		return this.userRepo
			.deleteArtistSubscription({ artistId: this.artistId })
			.then(() => {
				runInAction(() => {
					this.hasArtistSubscription = false;
				});
			});
	};
}
