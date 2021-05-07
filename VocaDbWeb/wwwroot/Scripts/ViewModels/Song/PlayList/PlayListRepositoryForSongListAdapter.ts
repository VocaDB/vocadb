import AdvancedSearchFilter from '../../Search/AdvancedSearchFilter';
import ContentLanguagePreference from '../../../Models/Globalization/ContentLanguagePreference';
import { IPlayListRepository } from './PlayListViewModel';
import { ISongForPlayList } from './PlayListViewModel';
import PagingProperties from '../../../DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '../../../DataContracts/PartialFindResultContract';
import SongListRepository from '../../../Repositories/SongListRepository';
import { SongOptionalFields } from '../../../Models/EntryOptionalFields';
import SongType from '../../../Models/Songs/SongType';

export default class PlayListRepositoryForSongListAdapter
  implements IPlayListRepository {
  constructor(
    private songListRepo: SongListRepository,
    private songListId: number,
    private query: KnockoutObservable<string>,
    private songType: KnockoutObservable<string>,
    private tagIds: KnockoutObservable<number[]>,
    private childTags: KnockoutObservable<boolean>,
    private artistIds: KnockoutComputed<number[]>,
    private artistParticipationStatus: KnockoutObservable<string>,
    private childVoicebanks: KnockoutObservable<boolean>,
    private advancedFilters: KnockoutObservableArray<AdvancedSearchFilter>,
    private sort: KnockoutObservable<string>,
  ) {}

  public getSongs = (
    pvServices: string,
    paging: PagingProperties,
    fields: SongOptionalFields,
    lang: ContentLanguagePreference,
    callback: (result: PartialFindResultContract<ISongForPlayList>) => void,
  ): void => {
    this.songListRepo
      .getSongs(
        this.songListId,
        this.query(),
        this.songType() !== SongType[SongType.Unspecified]
          ? this.songType()
          : null,
        this.tagIds(),
        this.childTags(),
        this.artistIds(),
        this.artistParticipationStatus(),
        this.childVoicebanks(),
        this.advancedFilters(),
        pvServices,
        paging,
        fields,
        this.sort(),
        lang,
      )
      .then((result) => {
        var mapped = _.map(result.items, (song, idx) => {
          return {
            name:
              song.order +
              '. ' +
              song.song.name +
              (song.notes ? ' (' + song.notes + ')' : ''),
            song: song.song,
            indexInPlayList: paging.start + idx,
          };
        });

        callback({ items: mapped, totalCount: result.totalCount });
      });
  };
}
