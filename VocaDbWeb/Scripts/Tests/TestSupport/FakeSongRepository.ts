/// <reference path="../../DataContracts/NewSongCheckResultContract.ts" />
/// <reference path="../../Repositories/SongRepository.ts" />

module vdb.tests.testSupport {

    import dc = vdb.dataContracts;

    export class FakeSongRepository extends vdb.repositories.SongRepository {

        addedSongId: number;
        results: dc.NewSongCheckResultContract = null;
        song: dc.SongApiContract = null;
        songLists: dc.SongListBaseContract[] = [];

        constructor() {
            
            super("");

            this.addSongToList = (listId, songId, notes, newListName, callback?) => {

                this.addedSongId = songId;

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

}