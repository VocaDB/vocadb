
import TagsEditViewModel from '../../../ViewModels/Tag/TagsEditViewModel';
import { TagSelectionViewModel } from '../../../ViewModels/Tag/TagsEditViewModel';

//namespace vdb.tests.viewModels.tags {
	
	var viewModel: TagsEditViewModel;

	QUnit.module("TagsEditViewModel", {
		setup: () => {

			viewModel = new TagsEditViewModel(null);

		}
	});

	QUnit.test("addTag - new tag", () => {

		viewModel.newTagName("Miku");

		viewModel.addTag();

		QUnit.equal(viewModel.selections().length, 1, "selections.length");

		var selection = viewModel.selections()[0];
		QUnit.equal(selection.tag.name, "Miku", "selection.tag.name");
		QUnit.equal(selection.selected(), true, "selection.selected");

		QUnit.equal(viewModel.newTagName(), "", "newTagName");

	});

	QUnit.test("addTag - already exists", () => {

		var selection = new TagSelectionViewModel({ tag: { name: 'Miku', id: 39 } });
		viewModel.selections.push(selection);
		QUnit.equal(selection.selected(), false, "selection.selected");
		viewModel.newTagName("Miku");

		viewModel.addTag();

		QUnit.equal(selection.selected(), true, "selection.selected");

	});

//} 