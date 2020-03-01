import EntryTypeAndSubTypeContract from "../EntryTypeAndSubTypeContract";
import TagBaseContract from "./TagBaseContract";

//namespace vdb.dataContracts.tags {

	export default interface EntryTagMappingContract {
		entryType: EntryTypeAndSubTypeContract;
		tag: TagBaseContract;
	}

//}