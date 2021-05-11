import EntryType from '@Models/EntryType';
import FakeUserRepository from '../../TestSupport/FakeUserRepository';
import TagApiContract from '@DataContracts/Tag/TagApiContract';
import TagEditViewModel from '@ViewModels/TagEditViewModel';

var viewModel: TagEditViewModel;

QUnit.module('TagEditViewModel', {
  setup: () => {
    viewModel = new TagEditViewModel(null!, new FakeUserRepository(), {
      targets: EntryType.Artist,
    } as TagApiContract);
  },
});

QUnit.test('hasTargetType - get - not set', () => {
  QUnit.equal(
    viewModel.hasTargetType(EntryType.Album)(),
    false,
    'hasTargetType',
  );
});

QUnit.test('hasTargetType - get - is set', () => {
  QUnit.equal(
    viewModel.hasTargetType(EntryType.Artist)(),
    true,
    'hasTargetType',
  );
});

QUnit.test('hasTargetType - set true', () => {
  viewModel.hasTargetType(EntryType.Album)(true);
  QUnit.equal(
    viewModel.targets(),
    EntryType.Album | EntryType.Artist,
    'targets',
  );
  QUnit.equal(
    viewModel.hasTargetType(EntryType.Album)(),
    true,
    'hasTargetType',
  );
});

QUnit.test('hasTargetType - set false', () => {
  viewModel.hasTargetType(EntryType.Artist)(false);
  QUnit.equal(viewModel.targets(), EntryType.Undefined, 'targets');
  QUnit.equal(
    viewModel.hasTargetType(EntryType.Album)(),
    false,
    'hasTargetType',
  );
});

QUnit.test('hasTargetType - set true - all true', () => {
  viewModel.hasTargetType(EntryType.Album)(true);
  viewModel.hasTargetType(EntryType.Song)(true);
  viewModel.hasTargetType(EntryType.ReleaseEvent)(true);
  QUnit.equal(viewModel.targets(), TagEditViewModel.allEntryTypes, 'targets'); // When all entry types are selected, flags mask is set to all
});
