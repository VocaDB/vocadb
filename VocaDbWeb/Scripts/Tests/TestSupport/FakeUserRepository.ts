/// <reference path="../../Repositories/UserRepository.ts" />

module vdb.tests.testSupport {

    import cls = vdb.models;
    import dc = vdb.dataContracts;

    export class FakeUserRepository extends vdb.repositories.UserRepository {

        public messageBody: string;
        public messageSummaries: dc.UserMessagesContract;
        public songId: number;
        public rating: cls.SongVoteRating;

        constructor() {

            super(new vdb.UrlMapper(""));        

            this.getMessageBody = (messageId, callback?) => {

                if (callback)
                    callback(this.messageBody);

            };

            this.getMessageSummaries = (maxCount?, unread?, iconSize?, callback?) => {

                if (callback)
                    callback(this.messageSummaries);

            };

            this.updateSongRating = (songId: number, rating: cls.SongVoteRating, callback: Function) => {

                this.songId = songId;
                this.rating = rating;

                if (callback)
                    callback();

            };

        }
    
    }

}