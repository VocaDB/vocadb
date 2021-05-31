import SongInListEditContract from './SongInListEditContract';
import SongListContract from './SongListContract';

export default interface SongListForEditContract extends SongListContract {
	songLinks: SongInListEditContract[];
}
