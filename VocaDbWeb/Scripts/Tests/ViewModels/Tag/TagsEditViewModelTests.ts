
module vdb.tests.viewModels.tags {
	
	import vm = vdb.viewModels;

	var viewModel: vm.tags.TagsEditViewModel;

	QUnit.module("TagsEditViewModel", {
		setup: () => {

			viewModel = new vm.tags.TagsEditViewModel(null);

		}
	});

	QUnit.test("addTag - new tag", () => {

		viewModel.newTagName("Miku");

		viewModel.addTag();

		QUnit.equal(viewModel.selections().length, 1, "selections.length");

		var selection = viewModel.selections()[0];
		QUnit.equal(selection.tagName, "Miku", "selection.tagName");
		QUnit.equal(selection.selected(), true, "selection.selected");

		QUnit.equal(viewModel.newTagName(), "", "newTagName");

	});

	QUnit.test("addTag - already exists", () => {

		var selection = new vm.tags.TagSelectionViewModel({ tag: { name: 'Miku', id: 39 } });
		viewModel.selections.push(selection);
		QUnit.equal(selection.selected(), false, "selection.selected");
		viewModel.newTagName("Miku");

		viewModel.addTag();

		QUnit.equal(selection.selected(), true, "selection.selected");

	});

} 