/// <reference path="../../Repositories/UserRepository.ts" />

import PartialFindResultContract from '../../DataContracts/PartialFindResultContract';
import SongVoteRating from '../../Models/SongVoteRating';
import UrlMapper from '../../Shared/UrlMapper';
import { UserInboxType } from '../../Repositories/UserRepository';
import UserMessageSummaryContract from '../../DataContracts/User/UserMessageSummaryContract';
import UserRepository from '../../Repositories/UserRepository';

//module vdb.tests.testSupport {

    export default class FakeUserRepository extends UserRepository {

        public message: UserMessageSummaryContract;
        public messages: UserMessageSummaryContract[];
        public songId: number;
        public rating: SongVoteRating;

        constructor() {

            super(new UrlMapper(""));        

            this.getMessage = (messageId, callback?) => {

                if (callback)
                    callback(this.message);

            };

            this.getMessageSummaries = (userId: number, inbox: UserInboxType, maxCount?, unread?, anotherUserId?, iconSize?,
				callback?: (result: PartialFindResultContract<UserMessageSummaryContract>) => void) => {

                if (callback)
                    callback({ items: this.messages, totalCount: (this.messages ? this.messages.length : 0) });

            };

            this.updateSongRating = (songId: number, rating: SongVoteRating, callback: Function) => {

                this.songId = songId;
                this.rating = rating;

                if (callback)
					callback();

				return $.Deferred().resolve();

            };

        }
    
    }

//}