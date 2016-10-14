/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../DataContracts/User/UserMessageSummaryContract.ts" />
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

		public deleteMessage = (messageId: number) => {

            var url = this.urlMapper.mapRelative("/User/DeleteMessage");
            $.post(url, { messageId: messageId });

		}

		public deleteMessages = (userId: number, messageIds: number[]) => {
			
            var url = this.urlMapper.mapRelative("/api/users/" + userId + "/messages");
			helpers.AjaxHelper.deleteJSON_Url(url, "messageId", messageIds);

		}

		public getAlbumCollectionList = (
			userId: number,
			paging: dc.PagingProperties, lang: string, query: string,
			tag: number,
			artistId: number,
			purchaseStatuses: string,
			releaseEventId: number,
			advancedFilters: viewModels.search.AdvancedSearchFilter[],
			sort: string,
			callback) => {

			var url = this.urlMapper.mapRelative("/api/users/" + userId + "/albums");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				query: query,
				tagId: tag,
				artistId: artistId,
				purchaseStatuses: purchaseStatuses,
				releaseEventId: releaseEventId || undefined,
				fields: "AdditionalNames,MainPicture",
				lang: lang,
				nameMatchMode: 'Auto',
				sort: sort,
				advancedFilters: advancedFilters
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
				fields: "AdditionalNames,MainPicture", lang: lang, nameMatchMode: 'Auto', artistType: artistType
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
			knowsLanguage: string,
			fields: string,
			callback: (result: dc.PartialFindResultContract<dc.user.UserApiContract>) => void) => {

			var url = this.urlMapper.mapRelative("/api/users");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				query: query, nameMatchMode: 'Auto', sort: sort,
				includeDisabled: includeDisabled,
				onlyVerified: onlyVerified,
				knowsLanguage: knowsLanguage,
				groups: groups,
				fields: fields
			};

			$.getJSON(url, data, callback);

		}

		public getOne = (id: number, callback: (result: dc.user.UserApiContract) => void) => {
			var url = this.urlMapper.mapRelative("/api/users/" + id);
			$.getJSON(url, callback);
		}

        public getMessage = (messageId: number, callback?: (result: dc.UserMessageSummaryContract) => void) => {

            var url = this.urlMapper.mapRelative("/api/users/messages/" + messageId);
            $.getJSON(url, callback);

        };

        public getMessageSummaries = (userId: number, inbox: UserInboxType, paging: dc.PagingProperties, unread: boolean = false, iconSize: number = 40,
			callback?: (result: dc.PartialFindResultContract<dc.UserMessageSummaryContract>) => void) => {

            var url = this.urlMapper.mapRelative("/api/users/" + (userId || this.loggedUserId) + "/messages");
            $.getJSON(url, { inbox: UserInboxType[inbox], start: paging.start, maxResults: paging.maxEntries, getTotalCount: paging.getTotalCount, unread: unread }, callback);

		};

		public getRatedSongsList = (
			userId: number,
			paging: dc.PagingProperties, lang: string, query: string,
			tagIds: number[],
			artistIds: number[],
			childVoicebanks: boolean,
			rating: string,
			songListId: number,
			advancedFilters: viewModels.search.AdvancedSearchFilter[],
			groupByRating: boolean,
			pvServices: string,
			fields: string,
			sort: string,
			callback: (result: dc.PartialFindResultContract<dc.RatedSongForUserForApiContract>) => void) => {

			var url = this.urlMapper.mapRelative("/api/users/" + userId + "/ratedSongs");
			var data = {
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				query: query, tagId: tagIds,
				artistId: artistIds,
				childVoicebanks: childVoicebanks,
				rating: rating,
				songListId: songListId,
				advancedFilters: advancedFilters,
				groupByRating: groupByRating,
				pvServices: pvServices,
				fields: fields, lang: lang, nameMatchMode: 'Auto', sort: sort
			};

			$.getJSON(url, data, callback);

		}

		public getRatingsByGenre = (userId: number, callback: (points: vdb.helpers.Tuple2<string, number>[]) => void) => {
	    
			var url = this.urlMapper.mapRelative('/api/users/' + userId + '/songs-per-genre/');
			$.getJSON(url, data => {
				callback(data);
			});

		}

		public getSongLists = (userId: number, query: string, paging: dc.PagingProperties, sort: string, fields: string,
			callback: (result: dc.PartialFindResultContract<dc.SongListContract>) => void) => {
	    
			var url = this.urlMapper.mapRelative("/api/users/" + userId + "/songLists");
			$.getJSON(url, {
				query: query,
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				sort: sort,
				fields: fields
			}, callback);

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

		public refreshEntryEdit = (entryType: models.EntryType, entryId: number) => {
			$.post(this.urlMapper.mapRelative("/api/users/current/refreshEntryEdit/?entryType=" + models.EntryType[entryType] + "&entryId=" + entryId));
		}

		public requestEmailVerification = (callback?: () => void) => {

			var url = this.mapUrl("/RequestEmailVerification");
			$.post(url, callback);

		};

        public updateAlbumTags = (albumId: number, tags: dc.TagBaseContract[], callback: (usages: dc.tags.TagUsageForApiContract[]) => void) => {

			helpers.AjaxHelper.putJSON(this.urlMapper.mapRelative("/api/users/current/albumTags/" + albumId), tags, callback);

        }

		// Updates artist subscription settings for an artist followed by a user.
		public updateArtistSubscription = (artistId: number, emailNotifications?: boolean, siteNotifications?: boolean) => {

			$.post(this.mapUrl("/UpdateArtistSubscription"), {
				artistId: artistId, emailNotifications: emailNotifications, siteNotifications: siteNotifications
			});

		};

        public updateArtistTags = (artistId: number, tags: dc.TagBaseContract[], callback: (usages: dc.tags.TagUsageForApiContract[]) => void) => {

			helpers.AjaxHelper.putJSON(this.urlMapper.mapRelative("/api/users/current/artistTags/" + artistId), tags, callback);

        }

		public updateComment = (commentId: number, contract: dc.CommentContract, callback?: () => void) => {

			$.post(this.urlMapper.mapRelative("/api/users/profileComments/" + commentId), contract, callback, 'json');

		}

        // Updates rating score for a song.
        // songId: Id of the song to be updated.
        // rating: Song rating.
        // callback: Callback function to be executed when the operation is complete.
        public updateSongRating = (songId: number, rating: vdb.models.SongVoteRating, callback: () => void) => {

			var url = this.urlMapper.mapRelative("/api/users/current/ratedSongs/" + songId);
			$.post(url, { rating: vdb.models.SongVoteRating[rating] }, callback);

        }

        public updateSongTags = (songId: number, tags: dc.TagBaseContract[], callback: (usages: dc.tags.TagUsageForApiContract[]) => void) => {

			helpers.AjaxHelper.putJSON(this.urlMapper.mapRelative("/api/users/current/songTags/" + songId), tags, callback);
			 
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

        }

    }
	
	export enum UserInboxType {
		Nothing,
		Received,
		Sent,
		Notifications			
	}

}