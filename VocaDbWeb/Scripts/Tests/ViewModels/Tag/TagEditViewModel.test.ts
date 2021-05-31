import TagApiContract from '@DataContracts/Tag/TagApiContract';
import EntryType from '@Models/EntryType';
import TagEditViewModel from '@ViewModels/TagEditViewModel';

import FakeUserRepository from '../../TestSupport/FakeUserRepository';

var viewModel: TagEditViewModel;

beforeEach(() => {
  viewModel = new TagEditViewModel(null!, new FakeUserRepository(), {
    targets: EntryType.Artist,
  } as TagApiContract);
});

test('hasTargetType - get - not set', () => {
  expect(viewModel.hasTargetType(EntryType.Album)(), 'hasTargetType').toBe(
    false,
  );
});

test('hasTargetType - get - is set', () => {
  expect(viewModel.hasTargetType(EntryType.Artist)(), 'hasTargetType').toBe(
    true,
  );
});

test('hasTargetType - set true', () => {
  viewModel.hasTargetType(EntryType.Album)(true);
  expect(viewModel.targets(), 'targets').toBe(
    EntryType.Album | EntryType.Artist,
  );
  expect(viewModel.hasTargetType(EntryType.Album)(), 'hasTargetType').toBe(
    true,
  );
});

test('hasTargetType - set false', () => {
  viewModel.hasTargetType(EntryType.Artist)(false);
  expect(viewModel.targets(), 'targets').toBe(EntryType.Undefined);
  expect(viewModel.hasTargetType(EntryType.Album)(), 'hasTargetType').toBe(
    false,
  );
});

test('hasTargetType - set true - all true', () => {
  viewModel.hasTargetType(EntryType.Album)(true);
  viewModel.hasTargetType(EntryType.Song)(true);
  viewModel.hasTargetType(EntryType.ReleaseEvent)(true);
  expect(viewModel.targets(), 'targets').toBe(TagEditViewModel.allEntryTypes); // When all entry types are selected, flags mask is set to all
});
