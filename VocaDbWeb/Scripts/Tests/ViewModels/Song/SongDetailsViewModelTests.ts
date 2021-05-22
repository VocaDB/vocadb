import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import SongVoteRating from '@Models/SongVoteRating';
import HttpClient from '@Shared/HttpClient';
import { SongDetailsAjax } from '@ViewModels/Song/SongDetailsViewModel';
import { SongDetailsResources } from '@ViewModels/Song/SongDetailsViewModel';
import SongDetailsViewModel from '@ViewModels/Song/SongDetailsViewModel';
import { SongListsViewModel } from '@ViewModels/Song/SongDetailsViewModel';
import _ from 'lodash';

import FakeSongRepository from '../../TestSupport/FakeSongRepository';
import FakeUserRepository from '../../TestSupport/FakeUserRepository';

var rep: FakeSongRepository;
var userRep = new FakeUserRepository();
var res: SongDetailsResources = { createNewList: 'Create new list' };
var data: SongDetailsAjax = {
  id: 39,
  version: 0,
  selectedLyricsId: 0,
  selectedPvId: 0,
  songType: 'Original',
  tagUsages: [],
  userRating: 'Nothing',
  latestComments: [],
};

var target: SongDetailsViewModel;

QUnit.module('SongDetailsViewModelTests', {
  setup: () => {
    rep = new FakeSongRepository();
    rep.songLists = [
      { id: 1, name: 'Favorite Mikus', featuredCategory: 'Nothing' },
      { id: 2, name: 'Favorite Lukas', featuredCategory: 'Nothing' },
      { id: 3, name: 'Mikupa 2013', featuredCategory: 'Concerts' },
    ];
    target = new SongDetailsViewModel(
      new HttpClient(),
      rep,
      userRep,
      null!,
      res,
      false,
      data,
      [],
      0,
      ContentLanguagePreference.Default,
      false,
      null!,
    );
  },
});

QUnit.test('constructor', () => {
  equal(target.id, 39, 'id');
  ok(target.songListDialog, 'songListDialog');
  ok(target.userRating, 'userRating');
  equal(
    target.userRating.rating(),
    SongVoteRating['Nothing'],
    'userRating.rating',
  );
});

QUnit.test('showSongLists has lists', () => {
  target.songListDialog.showSongLists();

  equal(
    target.songListDialog.songLists().length,
    2,
    'songListDialog.songLists.length',
  );
  equal(
    target.songListDialog.songLists()[0].name,
    'Favorite Mikus',
    'songListDialog.songLists[0].name',
  );
  equal(
    target.songListDialog.selectedListId(),
    1,
    'songListDialog.selectedListId',
  );
});

QUnit.test('showSongLists no lists', () => {
  rep.songLists = [];
  target.songListDialog.showSongLists();

  equal(
    target.songListDialog.songLists().length,
    0,
    'songListDialog.songLists.length',
  );
  equal(
    target.songListDialog.selectedListId(),
    null,
    'songListDialog.selectedListId',
  );
});

QUnit.test('showSongLists only featured lists', () => {
  rep.songLists = [
    { id: 1, name: 'Mikupa 2013', featuredCategory: 'Concerts' },
  ];

  target.songListDialog.showSongLists();

  equal(
    target.songListDialog.songLists().length,
    1,
    'songListDialog.songLists.length',
  );
  equal(
    target.songListDialog.songLists()[0].name,
    'Mikupa 2013',
    'songListDialog.songLists[0].name',
  );
  equal(
    target.songListDialog.selectedListId(),
    1,
    'songListDialog.selectedListId',
  );
  equal(
    target.songListDialog.tabName(),
    SongListsViewModel.tabName_Featured,
    'target.songListDialog.tabName',
  );
});

QUnit.test('addSongToList', () => {
  target.songListDialog.showSongLists();

  target.songListDialog.addSongToList();

  equal(rep.songLists.length, 3, 'rep.songLists.length');
  var songInList = rep.songsInLists[0];
  equal(songInList.songId, 39, 'songInList.songId: Song as expected');
  equal(songInList.listId, 1, 'songInList.listId: List as expected');
});

QUnit.test(
  'addSongToList custom list when there are only featured lists',
  () => {
    rep.songLists = [
      { id: 1, name: 'Mikupa 2013', featuredCategory: 'Concerts' },
      { id: 2, name: 'Mikupa 2014', featuredCategory: 'Concerts' },
    ];

    target.songListDialog.showSongLists();
    target.songListDialog.tabName(SongListsViewModel.tabName_New);
    target.songListDialog.newListName('Favorite Rinnssss');

    target.songListDialog.addSongToList();

    equal(
      rep.songLists.length,
      3,
      'rep.songLists.length: New list was created',
    );
    var newList = _.find(
      rep.songLists,
      (sl) => sl.name === 'Favorite Rinnssss',
    )!;
    ok(newList, 'newList: New list was created');
    equal(
      rep.songsInLists.length,
      1,
      'rep.songsInLists: Song was added to list',
    );
    var songInList = rep.songsInLists[0];
    equal(songInList.songId, 39, 'songInList.songId: Song as expected');
    equal(songInList.listId, newList.id, 'songInList.listId: List as expected');
  },
);

QUnit.test('tabName featured lists tab', () => {
  target.songListDialog.showSongLists();

  target.songListDialog.tabName('Featured');

  equal(
    target.songListDialog.songLists().length,
    1,
    'songListDialog.songLists.length',
  );
  equal(
    target.songListDialog.songLists()[0].name,
    'Mikupa 2013',
    'songListDialog.songLists[0].name',
  );
});

QUnit.test('songInListsDialog show', () => {
  target.songInListsDialog.show();

  equal(
    target.songInListsDialog.dialogVisible(),
    true,
    'songInListsDialog.dialogVisible',
  );
  ok(target.songInListsDialog.contentHtml(), 'songInListsDialog.contentHtml');
});

QUnit.test('getMatchedSite no match', () => {
  const result = target.getMatchedSite('http://google.com');

  equal(result, null, 'result is null');
});

QUnit.test('getMatchedSite match http', () => {
  const result = target.getMatchedSite('http://vocadb.net/S/3939');

  ok(result, 'result is a match');
  equal(result.siteUrl, 'https://vocadb.net/', 'Site URL converted to https');
  equal(result.id, '3939', 'id');
});

QUnit.test('getMatchedSite match https', () => {
  const result = target.getMatchedSite('https://vocadb.net/S/3939');

  ok(result, 'result is a match');
  equal(result.siteUrl, 'https://vocadb.net/', 'Site URL');
  equal(result.id, '3939', 'id');
});
