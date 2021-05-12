import SongListForEditContract from '@DataContracts/Song/SongListForEditContract';
import ImportedSongInListContract from '@DataContracts/SongList/ImportedSongInListContract';
import ImportedSongListContract from '@DataContracts/SongList/ImportedSongListContract';
import PartialImportedSongs from '@DataContracts/SongList/PartialImportedSongs';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import UrlMapper from '@Shared/UrlMapper';
import _ from 'lodash';

export default class ImportSongListViewModel {
  constructor(private urlMapper: UrlMapper) {}

  public description = ko.observable('');

  public items = ko.observableArray<ImportedSongInListContract>([]);

  public loadMore = (): void => {
    $.getJSON(
      this.urlMapper.mapRelative('/api/songLists/import-songs'),
      {
        url: this.url(),
        pageToken: this.nextPageToken(),
        parseAll: !this.onlyRanked(),
      },
      (result: PartialImportedSongs) => {
        this.nextPageToken(result.nextPageToken);
        ko.utils.arrayPushAll(this.items, result.items);
      },
    ).fail((jqXHR: JQueryXHR) => {
      if (jqXHR.statusText) alert(jqXHR.statusText);
    });
  };

  public missingSongs = ko.computed(() =>
    _.some(this.items(), (i) => i.matchedSong == null),
  );

  public name = ko.observable('');

  public nextPageToken = ko.observable<string>(null!);

  public hasMore = ko.computed(() => this.nextPageToken() != null);

  public onlyRanked = ko.observable(false);

  public parse = (): void => {
    $.getJSON(
      this.urlMapper.mapRelative('/api/songLists/import'),
      { url: this.url(), parseAll: !this.onlyRanked() },
      (songList: ImportedSongListContract) => {
        this.name(songList.name);
        this.description(songList.description);
        this.nextPageToken(songList.songs.nextPageToken);
        this.items(songList.songs.items);
        this.totalSongs(songList.songs.totalCount);
        this.parsed(true);
      },
    ).fail((jqXHR: JQueryXHR) => {
      if (jqXHR.statusText) alert(jqXHR.statusText);
    });
  };

  public parsed = ko.observable(false);

  public submit = (): void => {
    var order = 1;
    var songs = _.chain(this.items())
      .filter((i) => i.matchedSong != null)
      .map((i: ImportedSongInListContract) => {
        return {
          order: order++,
          notes: '',
          song: i.matchedSong,
          songInListId: null!,
        };
      })
      .value();

    var contract: SongListForEditContract = {
      id: null!,
      author: null!,
      name: this.name(),
      description: this.description(),
      featuredCategory: 'Nothing',
      status: 'Finished',
      songLinks: songs,
    };

    $.postJSON(
      this.urlMapper.mapRelative('/api/songLists'),
      contract,
      (listId: number) => {
        window.location.href = EntryUrlMapper.details('SongList', listId);
      },
      'json',
    );
  };

  public totalSongs = ko.observable<number>(null!);

  public url = ko.observable('');

  public wvrNumber = ko.observable(0);
}
