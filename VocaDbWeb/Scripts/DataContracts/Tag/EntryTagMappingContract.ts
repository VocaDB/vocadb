import EntryTypeAndSubTypeContract from '../EntryTypeAndSubTypeContract';
import TagBaseContract from './TagBaseContract';

export default interface EntryTagMappingContract {
  entryType: EntryTypeAndSubTypeContract;
  tag: TagBaseContract;
}
