import SongWithPVAndVoteContract from '@DataContracts/Song/SongWithPVAndVoteContract';
import SongRepository from '@Repositories/SongRepository';
import UserRepository from '@Repositories/UserRepository';
import PVRatingButtonsStore from '@Stores/PVRatingButtonsStore';
import { action, makeObservable, observable, runInAction } from 'mobx';

// Store for song with PV preview and rating buttons (for example, on front page and song index page).
export default class SongWithPreviewStore {
	// Whether preview mode is active.
	@observable public preview = false;
	@observable public selectedSong?: SongWithPVAndVoteContract;
	@observable public pvService?: string = undefined /* TODO: enum */;
	// View model for rating buttons.
	@observable public ratingButtons?: PVRatingButtonsStore = undefined;

	// Event handler for the event when the song has been rated.
	public ratingComplete = (): void => {};

	public constructor(
		private readonly songRepo: SongRepository,
		private readonly userRepo: UserRepository,
		private readonly songId: number,
		public readonly pvServices: string,
	) {
		makeObservable(this);
	}

	// Destroy PV player (clears HTML)
	@action public destroyPV = (): void => {
		this.selectedSong = undefined;
	};

	// Toggle preview status.
	@action public togglePreview = (): void => {
		if (this.preview) {
			this.preview = false;
			this.ratingButtons = undefined;
			return;
		}

		this.songRepo.pvPlayerWithRating({ songId: this.songId }).then((result) => {
			if (!result) return;

			runInAction(() => {
				this.pvService = result.pvService;
				this.selectedSong = result.song;
				const ratingButtonsStore = new PVRatingButtonsStore(
					this.userRepo,
					result.song,
					this.ratingComplete,
				);
				this.ratingButtons = ratingButtonsStore;
				this.preview = true;
			});
		});
	};

	@action public switchPV = (newService: string): void => {
		this.pvService = newService;
	};
}
