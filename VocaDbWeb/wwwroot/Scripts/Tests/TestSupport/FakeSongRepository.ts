import NewSongCheckResultContract from '../../DataContracts/NewSongCheckResultContract';
import SongApiContract from '../../DataContracts/Song/SongApiContract';
import SongListBaseContract from '../../DataContracts/SongListBaseContract';
import SongRepository from '../../Repositories/SongRepository';
import HttpClient from '../../Shared/HttpClient';
import FakePromise from './FakePromise';

export interface SongInList {
  listId: number;
  songId: number;
  notes: string;
}

export default class FakeSongRepository extends SongRepository {
  results: NewSongCheckResultContract = null;
  song: SongApiContract = null;
  songLists: SongListBaseContract[] = [];
  songsInLists: SongInList[] = [];

  constructor() {
    super(new HttpClient(), '');

    this.addSongToList = (
      listId,
      songId,
      notes,
      newListName,
      callback?,
    ): void => {
      if (listId !== 0) {
        this.songsInLists.push({
          listId: listId,
          songId: songId,
          notes: notes,
        });
      } else {
        const nextListId =
          (_.max(_.map(this.songLists, (sl) => sl.id)) || 0) + 1;
        this.songLists.push({ id: nextListId, name: newListName });
        this.songsInLists.push({
          listId: nextListId,
          songId: songId,
          notes: notes,
        });
      }

      if (callback) callback();
    };

    this.findDuplicate = (params): Promise<NewSongCheckResultContract> => {
      return FakePromise.resolve(this.results);
    };

    this.getOneWithComponents = (
      id,
      fields,
      languagePreference,
    ): Promise<SongApiContract> => {
      return FakePromise.resolve(this.song);
    };

    this.songListsForSong = (songId, callback): void => {
      if (callback) callback('Miku!');
    };

    this.songListsForUser = (ignoreSongId, callback): void => {
      if (callback) callback(this.songLists);
    };
  }
}
