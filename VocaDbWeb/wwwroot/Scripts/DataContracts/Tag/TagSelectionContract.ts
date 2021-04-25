import TagBaseContract from './TagBaseContract';

export default interface TagSelectionContract {
  selected?: boolean;

  tag: TagBaseContract;
}
