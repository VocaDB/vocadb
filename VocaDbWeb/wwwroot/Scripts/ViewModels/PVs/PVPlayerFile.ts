import PVService from '@Models/PVs/PVService';

import { IPVPlayer } from './PVPlayerViewModel';

export default class PVPlayerFile implements IPVPlayer {
  constructor(
    private playerElementId: string,
    private wrapperElement: HTMLElement,
    public songFinishedCallback: () => void = null!,
    service: PVService = PVService.File,
  ) {
    this.service = service;
  }

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
        $("<audio id='" + this.playerElementId + "' />"),
      );
    }

    this.player = $('#' + this.playerElementId)[0] as HTMLAudioElement;
    this.player.onended = (): void => {
      if (this.player && this.songFinishedCallback) this.songFinishedCallback();
    };

    if (readyCallback) readyCallback();
  };

  public detach = (): void => {
    if (this.player) {
      this.player.onended = null;
    }

    this.player = null!;
  };

  private player: HTMLAudioElement = null!;

  public play = (pvId?: string): void => {
    if (!this.player) this.attach(false);

    if (pvId) {
      this.player.src = pvId;
      this.player.autoplay = true;
    } else {
      this.player.play();
    }
  };

  public service: PVService;
}
