import { IPVPlayer } from './PVPlayerViewModel';
import PVService from '../../Models/PVs/PVService';

export default class PVPlayerYoutube implements IPVPlayer {
  constructor(
    private playerElementId: string,
    private wrapperElement: HTMLElement,
    public songFinishedCallback: () => void = null,
  ) {}

  public attach = (
    reset: boolean = false,
    readyCallback?: () => void,
  ): void => {
    if (!reset && this.player) {
      if (readyCallback) readyCallback();
      return;
    }

    if (reset) {
      $(this.wrapperElement).empty();
      $(this.wrapperElement).append(
        $("<div id='" + this.playerElementId + "' />"),
      );
    }

    this.player = new YT.Player(this.playerElementId, {
      width: 560,
      height: 315,
      events: {
        onStateChange: (event: YT.EventArgs): void => {
          // This will still be fired once if the user disabled autoplay mode.
          if (
            this.player &&
            event.data == YT.PlayerState.ENDED &&
            this.songFinishedCallback
          ) {
            this.songFinishedCallback();
          }
        },
        onReady: (): void => {
          if (readyCallback) readyCallback();
        },
        onError: (): void => {
          // Some delay, to let the user read the error message and to prevent infinite loop
          setTimeout(() => {
            if (this.player && this.songFinishedCallback) {
              this.songFinishedCallback();
            }
          }, 3000);
        },
      },
    });
  };

  public detach = (): void => {
    this.player = null;
  };

  private player: YT.Player = null;

  private doPlay = (pvId: string): void => {
    if (pvId) {
      this.player.loadVideoById(pvId);
    } else {
      this.player.playVideo();
    }
  };

  public play = (pvId: string): void => {
    if (!this.player) {
      this.attach(false, () => this.doPlay(pvId));
    } else {
      this.doPlay(pvId);
    }
  };

  public service = PVService.Youtube;
}
