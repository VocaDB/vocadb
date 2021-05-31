import ContentLanguageSelection from '@Models/Globalization/ContentLanguageSelection';
import LocalizedStringWithIdEditViewModel from '@ViewModels/Globalization/LocalizedStringWithIdEditViewModel';
import NamesEditViewModel from '@ViewModels/Globalization/NamesEditViewModel';

test('primary name and aliases', () => {
  var names = [
    new LocalizedStringWithIdEditViewModel(
      ContentLanguageSelection.English,
      'English name',
      1,
    ),
    new LocalizedStringWithIdEditViewModel(
      ContentLanguageSelection.Unspecified,
      'Alias',
      2,
    ),
  ];

  var viewModel = new NamesEditViewModel(names);

  expect(viewModel.englishName.value(), 'englishName').toBe('English name');
  expect(viewModel.originalName.value(), 'originalName').toBe('');
  expect(viewModel.aliases().length, 'aliases').toBe(1);
  expect(viewModel.aliases()[0].value(), 'aliases').toBe('Alias');
  expect(viewModel.hasPrimaryName(), 'hasPrimaryName').toBe(true);
});

test('only primary name', () => {
  var names = [
    new LocalizedStringWithIdEditViewModel(
      ContentLanguageSelection.Japanese,
      'Japanese name',
      1,
    ),
  ];

  var viewModel = new NamesEditViewModel(names);

  expect(viewModel.englishName.value(), 'englishName').toBe('');
  expect(viewModel.originalName.value(), 'originalName').toBe('Japanese name');
  expect(viewModel.aliases().length, 'aliases').toBe(0);
  expect(viewModel.hasPrimaryName(), 'hasPrimaryName').toBe(true);
});

test('only aliases', () => {
  var names = [
    new LocalizedStringWithIdEditViewModel(
      ContentLanguageSelection.Unspecified,
      'Alias',
      1,
    ),
  ];

  var viewModel = new NamesEditViewModel(names);

  expect(viewModel.englishName.value(), 'englishName').toBe('');
  expect(viewModel.originalName.value(), 'originalName').toBe('');
  expect(viewModel.aliases().length, 'aliases').toBe(1);
  expect(viewModel.aliases()[0].value(), 'aliases').toBe('Alias');
  expect(viewModel.hasPrimaryName(), 'hasPrimaryName').toBe(false);
});
