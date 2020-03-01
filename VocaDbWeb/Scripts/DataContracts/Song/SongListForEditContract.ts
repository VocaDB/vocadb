import SongListContract from './SongListContract';
import SongInListEditContract from './SongInListEditContract';

	export default interface SongListForEditContract extends SongListContract {

		songLinks: SongInListEditContract[];

	}