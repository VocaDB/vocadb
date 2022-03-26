import IStoreWithUpdateResults from './IStoreWithUpdateResults';
import ServerSidePagingStore from './ServerSidePagingStore';

export default interface IStoreWithPaging<T>
	extends IStoreWithUpdateResults<T> {
	paging: ServerSidePagingStore;
}
