/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../DataContracts/User/UserMessageSummaryContract.ts" />
/// <reference path="../DataContracts/User/UserMessagesContract.ts" />
/// <reference path="../Models/SongVoteRating.ts" />

module vdb.repositories {

    import cls = vdb.models;
    import dc = vdb.dataContracts;

    // Repository for managing users and related objects.
    // Corresponds to the UserController class.
    export class UserRepository implements ICommentRepository {

		public createComment = (userId: number, contract: dc.CommentContract, callback: (contract: dc.CommentContract) => void) => {

			$.post(this.urlMapper.mapRelative("/api/users/" + userId + "/profileComments"), contract, callback, 'json');

		}

		public deleteComment = (commentId: number, callback: () => void) => {
			
			$.ajax(this.urlMapper.mapRelative("/api/users/profileComments/" + commentId), { type: 'DELETE', success: callback });

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

		public getComments = (userId: number, callback) => {

			var url = this.urlMapper.mapRelative("/api/users/" + userId + "/profileComments");
			var data = {
				start: 0, getTotalCount: false, maxResults: 300,
				userId: userId
			};

			$.getJSON(url, data, result => callback(result.items));

		};

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
			fields: string,
			callback: (result: dc.PartialFindResultContract<dc.user.UserApiContract>) => void) => {

			var url = this.urlMapper.mapRelative("/api/users");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				query: query, nameMatchMode: 'Auto', sort: sort,
				includeDisabled: includeDisabled,
				onlyVerified: onlyVerified,
				groups: groups,
				fields: fields
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

		public getRatedSongsList = (
			userId: number,
			paging: dc.PagingProperties, lang: string, query: string,
			tag: string,
			artistId: number,
			childVoicebanks: boolean,
			rating: string,
			songListId: number,
			groupByRating: boolean,
			pvServices: string,
			fields: string,
			sort: string,
			callback: (result: dc.PartialFindResultContract<dc.RatedSongForUserForApiContract>) => void) => {

			var url = this.urlMapper.mapRelative("/api/users/" + userId + "/ratedSongs");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				query: query, tag: tag,
				artistId: artistId,
				childVoicebanks: childVoicebanks,
				rating: rating, songListId: songListId,
				groupByRating: groupByRating,
				pvServices: pvServices,
				fields: fields, lang: lang, nameMatchMode: 'Auto', sort: sort
			};

			$.getJSON(url, data, callback);

		}

		public getSongLists = (userId: number, callback) => {
	    
			var url = this.urlMapper.mapRelative("/api/users/" + userId + "/songLists");
			$.getJSON(url, callback);

		}

		// Gets a specific user's rating for a specific song.
		// userId: User ID. Can be null, in which case the logged user will be used (if any).
		// songId: ID of the song for which to get the rating. Cannot be null.
		// callback: Callback receiving the rating. If the user has not rated the song, or if the user is not logged in, this will be "Nothing".
		public getSongRating = (userId: number, songId: number, callback: (rating: string) => void) => {

			userId = userId || this.loggedUserId;

			if (!userId) {
				callback('Nothing');
				return;				
			}

			var url = this.urlMapper.mapRelative("/api/users/" + userId + "/ratedSongs/" + songId);
			$.getJSON(url, callback);

		}

        public getAlbumTagSelections = (albumId: number, callback: (tags: dc.tags.TagSelectionContract[]) => void) => {

			$.getJSON(this.urlMapper.mapRelative("/api/users/current/albumTags/" + albumId), callback);

        }

        public getArtistTagSelections = (artistId: number, callback: (tags: dc.tags.TagSelectionContract[]) => void) => {

			$.getJSON(this.urlMapper.mapRelative("/api/users/current/artistTags/" + artistId), callback);

        }

        public getSongTagSelections = (songId: number, callback: (tags: dc.tags.TagSelectionContract[]) => void) => {

			$.getJSON(this.urlMapper.mapRelative("/api/users/current/songTags/" + songId), callback);

        }

		public requestEmailVerification = (callback?: () => void) => {

			var url = this.mapUrl("/RequestEmailVerification");
			$.post(url, callback);

		};

        public updateAlbumTags = (albumId: number, tags: string[], callback: (usages: dc.tags.TagUsageForApiContract[]) => void) => {

			helpers.AjaxHelper.putJSON_Url(this.urlMapper.mapRelative("/api/users/current/albumTags/" + albumId), "tags", tags, callback);

        }

		// Updates artist subscription settings for an artist followed by a user.
		public updateArtistSubscription = (artistId: number, emailNotifications?: boolean, siteNotifications?: boolean) => {

			$.post(this.mapUrl("/UpdateArtistSubscription"), {
				artistId: artistId, emailNotifications: emailNotifications, siteNotifications: siteNotifications
			});

		};

        public updateArtistTags = (artistId: number, tags: string[], callback: (usages: dc.tags.TagUsageForApiContract[]) => void) => {

			helpers.AjaxHelper.putJSON_Url(this.urlMapper.mapRelative("/api/users/current/artistTags/" + artistId), "tags", tags, callback);

        }

		public updateComment = (commentId: number, contract: dc.CommentContract, callback?: () => void) => {

			$.post(this.urlMapper.mapRelative("/api/users/profileComments/" + commentId), contract, callback, 'json');

		}

        // Updates rating score for a song.
        // songId: Id of the song to be updated.
        // rating: Song rating.
        // callback: Callback function to be executed when the operation is complete.
        public updateSongRating: (songId: number, rating: vdb.models.SongVoteRating, callback: any) => void;

        public updateSongTags = (songId: number, tags: string[], callback: (usages: dc.tags.TagUsageForApiContract[]) => void) => {
	       
			helpers.AjaxHelper.putJSON_Url(this.urlMapper.mapRelative("/api/users/current/songTags/" + songId), "tags", tags, callback);
			 
        }

		// Updates user setting.
		// userId: user ID. Can be null in which case logged user ID (if any) will be used.
		// settingName: name of the setting to be updated, for example 'showChatBox'.
		// value: setting value, for example 'false'.
		public updateUserSetting = (userId: number, settingName: string, value: string, callback: () => void) => {
			
			var url = this.urlMapper.mapRelative("/api/users/" + (userId || this.loggedUserId) + "/settings/" + settingName);
			$.post(url, { '': value }, callback);

		}

        // Maps a relative URL to an absolute one.
        private mapUrl: (relative: string) => string;

        constructor(private urlMapper: vdb.UrlMapper, private loggedUserId?: number) {

            this.mapUrl = (relative: string) => {
                return urlMapper.mapRelative("/User") + relative;
            };

			this.updateSongRating = (songId: number, rating: cls.SongVoteRating, callback: any) => {

				$.post(this.mapUrl("/AddSongToFavorites"), { songId: songId, rating: vdb.models.SongVoteRating[rating] }, callback);

			};

        }

    }

}