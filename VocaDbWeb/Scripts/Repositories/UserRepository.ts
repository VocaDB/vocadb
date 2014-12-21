/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../DataContracts/User/UserMessageSummaryContract.ts" />
/// <reference path="../DataContracts/User/UserMessagesContract.ts" />
/// <reference path="../Models/SongVoteRating.ts" />

module vdb.repositories {

    import cls = vdb.models;
    import dc = vdb.dataContracts;

    // Repository for managing users and related objects.
    // Corresponds to the UserController class.
    export class UserRepository {

		public deleteComment = (commentId: number, callback) => {
			
			var url = this.urlMapper.mapRelative("/User/DeleteComment/");
			$.post(url, { commentId: commentId }, callback);

		}

		public getAlbumCollectionList = (
			userId: number,
			paging: dc.PagingProperties, lang: string, query: string,
			tag: string,
			artistId: number,
			purchaseStatuses: string,
			releaseEventName: string,
			sort: string,
			callback) => {

			var url = this.urlMapper.mapRelative("/api/users/" + userId + "/albums");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				query: query,
				tag: tag,
				artistId: artistId,
				purchaseStatuses: purchaseStatuses,
				releaseEventName: releaseEventName,
				fields: "MainPicture",
				lang: lang,
				nameMatchMode: 'Auto',
				sort: sort
			};

			$.getJSON(url, data, callback);

		}

		public getFollowedArtistsList = (
			userId: number,
			paging: dc.PagingProperties, lang: string,
			artistType: string,
			callback) => {

			var url = this.urlMapper.mapRelative("/api/users/" + userId + "/followedArtists");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				fields: "MainPicture", lang: lang, nameMatchMode: 'Auto', artistType: artistType
			};

			$.getJSON(url, data, callback);

		}

		public getList = (
			paging: dc.PagingProperties,
			query: string,
			sort: string,
			groups: string,
			includeDisabled: boolean,
			onlyVerified: boolean,
			callback: (result: dc.PartialFindResultContract<dc.UserWithIconContract>) => void) => {

			var url = this.urlMapper.mapRelative("/api/users");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				query: query, nameMatchMode: 'Auto', sort: sort,
				includeDisabled: includeDisabled,
				onlyVerified: onlyVerified,
				groups: groups
			};

			$.getJSON(url, data, callback);

		}

        public getMessageBody = (messageId: number, callback?: (result: string) => void) => {

            var url = this.mapUrl("/MessageBody");
            $.get(url, { messageId: messageId }, callback);

        };

        public getMessageSummaries = (maxCount: number = 200, unread: boolean = false, iconSize: number = 40, callback?: (result: dc.UserMessagesContract) => void ) => {

            var url = this.mapUrl("/MessagesJson");
            $.getJSON(url, { maxCount: maxCount, unread: unread, iconSize: iconSize }, callback);

		};

		public getProfileComments = (userId: number, paging: dc.PagingProperties, callback) => {

			var url = this.urlMapper.mapRelative("/api/users/" + userId + "/profileComments");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				userId: userId
			};

			$.getJSON(url, data, callback);

		};

		public getRatedSongsList = (
			userId: number,
			paging: dc.PagingProperties, lang: string, query: string,
			tag: string,
			artistId: number,
			childVoicebanks: boolean,
			rating: string,
			songListId: number,
			groupByRating: boolean,
			sort: string,
			callback) => {

			var url = this.urlMapper.mapRelative("/api/users/" + userId + "/ratedSongs");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				query: query, tag: tag,
				artistId: artistId,
				childVoicebanks: childVoicebanks,
				rating: rating, songListId: songListId, groupByRating: groupByRating,
				fields: "ThumbUrl", lang: lang, nameMatchMode: 'Auto', sort: sort
			};

			$.getJSON(url, data, callback);

		}

		public getSongLists = (userId: number, callback) => {
	    
			var url = this.urlMapper.mapRelative("/api/users/" + userId + "/songLists");
			$.getJSON(url, callback);

		}

		public requestEmailVerification = (callback?: () => void) => {

			var url = this.mapUrl("/RequestEmailVerification");
			$.post(url, callback);

		};

		// Updates artist subscription settings for an artist followed by a user.
		public updateArtistSubscription = (artistId: number, emailNotifications?: boolean, siteNotifications?: boolean) => {

			$.post(this.mapUrl("/UpdateArtistSubscription"), {
				artistId: artistId, emailNotifications: emailNotifications, siteNotifications: siteNotifications
			});

		};

        // Updates rating score for a song.
        // songId: Id of the song to be updated.
        // rating: Song rating.
        // callback: Callback function to be executed when the operation is complete.
        public updateSongRating: (songId: number, rating: vdb.models.SongVoteRating, callback: any) => void;

        // Maps a relative URL to an absolute one.
        private mapUrl: (relative: string) => string;

        constructor(private urlMapper: vdb.UrlMapper) {

            this.mapUrl = (relative: string) => {
                return urlMapper.mapRelative("/User") + relative;
            };

			this.updateSongRating = (songId: number, rating: cls.SongVoteRating, callback: any) => {

				$.post(this.mapUrl("/AddSongToFavorites"), { songId: songId, rating: vdb.models.SongVoteRating[rating] }, callback);

			};

        }

    }

}