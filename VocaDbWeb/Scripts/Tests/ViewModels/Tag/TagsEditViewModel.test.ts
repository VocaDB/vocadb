import TagsEditViewModel from '@ViewModels/Tag/TagsEditViewModel';
import { TagSelectionViewModel } from '@ViewModels/Tag/TagsEditViewModel';

var viewModel: TagsEditViewModel;

beforeEach(() => {
	viewModel = new TagsEditViewModel(null!);
});

test('addTag - new tag', () => {
	viewModel.newTagName('Miku');

	viewModel.addTag();

	expect(viewModel.selections().length, 'selections.length').toBe(1);

	var selection = viewModel.selections()[0];
	expect(selection.tag.name, 'selection.tag.name').toBe('Miku');
	expect(selection.selected(), 'selection.selected').toBe(true);

	expect(viewModel.newTagName(), 'newTagName').toBe('');
});

test('addTag - already exists', () => {
	var selection = new TagSelectionViewModel({ tag: { name: 'Miku', id: 39 } });
	viewModel.selections.push(selection);
	expect(selection.selected(), 'selection.selected').toBe(false);
	viewModel.newTagName('Miku');

	viewModel.addTag();

	expect(selection.selected(), 'selection.selected').toBe(true);
});
