import ArtistForAlbumEditViewModel from './ArtistForAlbumEditViewModel';

export default class CustomNameEditViewModel {
  public artistLink = ko.observable<ArtistForAlbumEditViewModel>();
  public dialogVisible = ko.observable(false);
  public name = ko.observable<string>(null);

  public open = (artist: ArtistForAlbumEditViewModel): void => {
    this.artistLink(artist);
    this.name(artist.isCustomName ? artist.name() : '');
    this.dialogVisible(true);
  };

  public save = (): void => {
    const isCustomName = !!this.name();

    this.artistLink().isCustomName = isCustomName;
    this.dialogVisible(false);

    if (isCustomName) {
      this.artistLink().name(this.name());
    }
  };
}
