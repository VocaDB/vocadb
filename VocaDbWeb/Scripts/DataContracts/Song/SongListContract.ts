/// <reference path="../SongListBaseContract.ts" />
/// <reference path="../User/UserBaseContract.ts" />

module vdb.dataContracts {

    export interface SongListContract extends SongListBaseContract {
        
        author: UserBaseContract;

        description: string;

        featuredCategory: string;
    
    }

}