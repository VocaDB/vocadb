import PVRatingButtonsViewModel from '../PVRatingButtonsViewModel';
import PVService from '@Models/PVs/PVService';
import SongRepository from '@Repositories/SongRepository';
import UserRepository from '@Repositories/UserRepository';

// View model for song with PV preview and rating buttons (for example, on front page and song index page).
export default class SongWithPreviewViewModel {
  // Destroy PV player (clears HTML)
  public destroyPV: () => void;

  // Whether preview mode is active.
  public preview: KnockoutObservable<boolean> = ko.observable(false);

  // PV player HTML.
  public previewHtml: KnockoutObservable<string> = ko.observable(null!);

  public pvService = ko.observable<string>(null!);

  // View model for rating buttons.
  public ratingButtons: KnockoutObservable<PVRatingButtonsViewModel> = ko.observable(
    null!,
  );

  // Event handler for the event when the song has been rated.
  public ratingComplete!: () => void;

  public switchPV: (pvService: string) => void;

  // Toggle preview status.
  public togglePreview: () => void;

  constructor(
    repository: SongRepository,
    userRepository: UserRepository,
    public songId: number,
    public pvServices: string,
  ) {
    this.destroyPV = (): void => {
      this.previewHtml(null!);
    };

    this.togglePreview = (): void => {
      if (this.preview()) {
        this.preview(false);
        this.ratingButtons(null!);
        return;
      }

      repository.pvPlayerWithRating(songId).then((result) => {
        if (!result) return;

        this.pvService(result.pvService);
        this.previewHtml(result.playerHtml);
        var ratingButtonsViewModel = new PVRatingButtonsViewModel(
          userRepository,
          result.song,
          this.ratingComplete,
        );
        this.ratingButtons(ratingButtonsViewModel);
        this.preview(true);
      });
    };

    this.switchPV = (newService: string): void => {
      this.pvService(newService);
      var service: PVService = PVService[newService as keyof typeof PVService];
      repository.pvForSongAndService(songId, service, (html) =>
        this.previewHtml(html),
      );
    };
  }
}
