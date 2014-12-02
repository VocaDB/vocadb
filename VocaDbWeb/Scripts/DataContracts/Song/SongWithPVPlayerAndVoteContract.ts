/// <reference path="../SongContract.ts" />

module vdb.dataContracts.songs {

    export interface SongWithPVPlayerAndVoteContract {
        
        playerHtml: string;

		pvService: string;

		song: SongWithPVAndVoteContract;
    
    }

}