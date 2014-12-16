/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../DataContracts/NewSongCheckResultContract.ts" />
/// <reference path="../DataContracts/Song/SongContract.ts" />
/// <reference path="../DataContracts/SongListBaseContract.ts" />
/// <reference path="../DataContracts/Song/SongListContract.ts" />
/// <reference path="../DataContracts/Song/SongWithPVPlayerAndVoteContract.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />

module vdb.repositories {

    import dc = vdb.dataContracts;
	import cls = vdb.models;

    // Repository for managing songs and related objects.
    // Corresponds to the SongController class.
    export class SongRepository extends BaseRepository {

        public addSongToList: (listId: number, songId: number, newListName: string, callback?: Function) => void;

        public findDuplicate: (params, callback: (result: dc.NewSongCheckResultContract) => void) => void;

        private get: (relative: string, params: any, callback: any) => void;

		public getForEdit = (id: number, callback: (result: dc.songs.SongForEditContract) => void) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/songs/" + id + "/for-edit");
			$.getJSON(url, callback);

		}

        private getJSON: (relative: string, params: any, callback: any) => void;

		public getOneWithComponents = (id: number, fields: string, languagePreference: string, callback?: (result: dc.SongApiContract) => void) => {
			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/songs/" + id);
			$.getJSON(url, { fields: fields, lang: languagePreference || this.languagePreferenceStr }, callback);
        }

		public getOne = (id: number, callback?: (result: dc.SongContract) => void) => {
			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/songs/" + id);
			$.getJSON(url, { fields: 'AdditionalNames', lang: this.languagePreferenceStr }, callback);         
		}

		public getList = (paging: dc.PagingProperties, lang: string, query: string,
			sort: string, songTypes: string, tag: string,
			artistId: number,
			artistParticipationStatus: string,
			childVoicebanks: boolean,
			onlyWithPvs: boolean,
			pvServices: string,
			since: number,
			userCollectionId: number,
			fields: string,
			status: string,
			callback) => {

			var url = vdb.functions.mergeUrls(this.baseUrl, "/api/songs");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				query: query, fields: fields, lang: lang, nameMatchMode: 'Auto', sort: sort,
				songTypes: songTypes,
				tag: tag,
				artistId: artistId,
				artistParticipationStatus: artistParticipationStatus,
				childVoicebanks: childVoicebanks,
				onlyWithPvs: onlyWithPvs,
				pvServices: pvServices,
				since: since,
				userCollectionId: userCollectionId,
				status: status
			};

			$.getJSON(url, data, callback);

		}

        // Maps a relative URL to an absolute one.
        private mapUrl: (relative: string) => string;

        private post: (relative: string, params: any, callback: any) => void;

		public pvForSongAndService: (songId: number, pvService: cls.pvs.PVService, callback: (result: string) => void) => void; 

		public pvPlayer = (songId: number, params: PVEmbedParams, callback: (result: dc.songs.SongWithPVPlayerAndVoteContract) => void) => {
			this.getJSON("/PVPlayer/" + songId, params, callback);
		}

        public pvPlayerWithRating: (songId: number, callback: (result: dc.songs.SongWithPVPlayerAndVoteContract) => void) => void; 

        //public songListsForSong: (songId: number, callback: (result: dc.SongListContract[]) => void) => void;

        public songListsForSong: (songId: number, callback: (result: string) => void ) => void;

        public songListsForUser: (ignoreSongId: number, callback: (result: dc.SongListBaseContract[]) => void ) => void;

        public usersWithSongRating: (id: number, callback: (result: string) => void) => void;

        constructor(baseUrl: string, languagePreference = cls.globalization.ContentLanguagePreference.Default) {

			super(baseUrl, languagePreference);

            this.get = (relative, params, callback) => {
                $.get(this.mapUrl(relative), params, callback);
            }

            this.getJSON = (relative, params, callback) => {
                $.getJSON(this.mapUrl(relative), params, callback);
            }

            this.mapUrl = (relative: string) => {
                return vdb.functions.mergeUrls(baseUrl, "/Song") + relative;
            };

            this.post = (relative, params, callback) => {
                $.post(this.mapUrl(relative), params, callback);
            }

            this.addSongToList = (listId, songId, newListName, callback?) => {
                this.post("/AddSongToList", { listId: listId, songId: songId, newListName: newListName }, callback);
            }

            this.findDuplicate = (params, callback: (result: dc.NewSongCheckResultContract) => void) => {
                this.post("/FindDuplicate", params, callback);
            }

			this.pvForSongAndService = (songId: number, pvService: cls.pvs.PVService, callback: (result: string) => void) => {
				this.get("/PVForSongAndService", { songId: songId, service: cls.pvs.PVService[pvService] }, callback);
            }

			this.pvPlayerWithRating = (songId, callback) => {
				this.getJSON("/PVPlayerWithRating", { songId: songId }, callback);
			}

            this.songListsForSong = (songId, callback) => {
                this.get("/SongListsForSong", { songId: songId }, callback);
            }

            this.songListsForUser = (ignoreSongId, callback) => {                
                this.post("/SongListsForUser", { ignoreSongId: ignoreSongId }, callback);
            }

            this.usersWithSongRating = (id, callback: (result: string) => void ) => {
                this.post("/UsersWithSongRating", { songId: id }, callback);
            }

        }

	}

	export interface PVEmbedParams {

		enableScriptAccess?: boolean;

		elementId?: string;

		pvServices?: string;

	}

	export interface SongQueryParams extends CommonQueryParams {

		songTypes: string;

	}

}