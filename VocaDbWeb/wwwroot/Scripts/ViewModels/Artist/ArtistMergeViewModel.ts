import { ArtistAutoCompleteParams } from '../../KnockoutExtensions/AutoCompleteParams';
import ArtistContract from '../../DataContracts/Artist/ArtistContract';
import ArtistRepository from '../../Repositories/ArtistRepository';
import BasicEntryLinkViewModel from '../BasicEntryLinkViewModel';
import EntryMergeValidationHelper from '../../Helpers/EntryMergeValidationHelper';

export default class ArtistMergeViewModel {
  constructor(repo: ArtistRepository, id: number) {
    this.target = new BasicEntryLinkViewModel(null, repo.getOne);

    this.targetSearchParams = {
      acceptSelection: this.target.id,
      ignoreId: id,
    };

    repo.getOne(id, (base) => {
      ko.computed(() => {
        var result = EntryMergeValidationHelper.validateEntry(
          base,
          this.target.entry(),
        );
        this.validationError_targetIsLessComplete(
          result.validationError_targetIsLessComplete,
        );
        this.validationError_targetIsNewer(
          result.validationError_targetIsNewer,
        );
      });
    });
  }

  public target: BasicEntryLinkViewModel<ArtistContract>;
  public targetSearchParams: ArtistAutoCompleteParams;

  public validationError_targetIsLessComplete = ko.observable(false);
  public validationError_targetIsNewer = ko.observable(false);
}
