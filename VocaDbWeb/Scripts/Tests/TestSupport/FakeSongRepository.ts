/// <reference path="../../DataContracts/NewSongCheckResultContract.ts" />
/// <reference path="../../Repositories/SongRepository.ts" />

//module vdb.tests.testSupport {

	import dc = vdb.dataContracts;

	export interface SongInList {
		listId: number;
		songId: number;
		notes: string;
	}

    export class FakeSongRepository extends vdb.repositories.SongRepository {

        results: dc.NewSongCheckResultContract = null;
        song: dc.SongApiContract = null;
		songLists: dc.SongListBaseContract[] = [];
		songsInLists: SongInList[] = [];

        constructor() {
            
            super("");

            this.addSongToList = (listId, songId, notes, newListName, callback?) => {

				if (listId !== 0) {
					this.songsInLists.push({ listId: listId, songId: songId, notes: notes });
				} else {
					const nextListId = (_.max(_.map(this.songLists, sl => sl.id)) || 0) + 1;
					this.songLists.push({ id: nextListId, name: newListName });
					this.songsInLists.push({ listId: nextListId, songId: songId, notes: notes });
				}

                if (callback)
                    callback();

            }

            this.findDuplicate = (params, callback: (result: dc.NewSongCheckResultContract) => void) => {
                if (callback)
                    callback(this.results);
            };

            this.getOneWithComponents = (id, fields, languagePreference, callback?) => {
                if (callback)
                    callback(this.song);
            }

            this.songListsForSong = (songId, callback) => {
                if (callback)
                    callback("Miku!");
            }

            this.songListsForUser = (ignoreSongId, callback) => {
                if (callback)
                    callback(this.songLists);
            }
        
        }

    }

//}