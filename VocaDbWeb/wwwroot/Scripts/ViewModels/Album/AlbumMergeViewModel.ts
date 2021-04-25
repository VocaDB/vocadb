import AlbumContract from '../../DataContracts/Album/AlbumContract';
import AlbumRepository from '../../Repositories/AlbumRepository';
import { ArtistAutoCompleteParams } from '../../KnockoutExtensions/AutoCompleteParams';
import BasicEntryLinkViewModel from '../BasicEntryLinkViewModel';
import EntryMergeValidationHelper from '../../Helpers/EntryMergeValidationHelper';

export default class AlbumMergeViewModel {
  constructor(repo: AlbumRepository, id: number) {
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

  public target: BasicEntryLinkViewModel<AlbumContract>;
  public targetSearchParams: ArtistAutoCompleteParams;

  public validationError_targetIsLessComplete = ko.observable(false);
  public validationError_targetIsNewer = ko.observable(false);
}
