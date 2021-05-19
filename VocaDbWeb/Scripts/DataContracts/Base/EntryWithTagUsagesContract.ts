import TagUsageForApiContract from '../Tag/TagUsageForApiContract';

export default interface EntryWithTagUsagesContract {
  id: number;

  name: string;

  tags?: TagUsageForApiContract[];
}
