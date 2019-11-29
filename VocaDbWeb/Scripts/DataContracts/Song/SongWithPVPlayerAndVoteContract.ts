
import SongWithPVAndVoteContract from './SongWithPVAndVoteContract';

//module vdb.dataContracts.songs {

    export default interface SongWithPVPlayerAndVoteContract {
        
        playerHtml: string;

		pvService: string;

		song: SongWithPVAndVoteContract;
    
    }

//}