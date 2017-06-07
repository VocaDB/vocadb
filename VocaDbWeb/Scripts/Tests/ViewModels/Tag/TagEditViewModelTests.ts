
namespace vdb.tests.viewModels.tags {

	import cls = vdb.models;
	import vm = vdb.viewModels;

	var viewModel: vm.tags.TagEditViewModel;

	QUnit.module("TagEditViewModel", {
		setup: () => {

			viewModel = new vm.tags.TagEditViewModel(null, new testSupport.FakeUserRepository(), { targets: cls.EntryType.Artist } as dc.TagApiContract);

		}
	});

	QUnit.test("hasTargetType - get - not set", () => {
		QUnit.equal(viewModel.hasTargetType(cls.EntryType.Album)(), false, "hasTargetType");
	});

	QUnit.test("hasTargetType - get - is set", () => {
		QUnit.equal(viewModel.hasTargetType(cls.EntryType.Artist)(), true, "hasTargetType");
	});

	QUnit.test("hasTargetType - set true", () => {
		viewModel.hasTargetType(cls.EntryType.Album)(true);
		QUnit.equal(viewModel.targets(), cls.EntryType.Album | cls.EntryType.Artist, "targets");
		QUnit.equal(viewModel.hasTargetType(cls.EntryType.Album)(), true, "hasTargetType");
	});

	QUnit.test("hasTargetType - set false", () => {
		viewModel.hasTargetType(cls.EntryType.Artist)(false);
		QUnit.equal(viewModel.targets(), cls.EntryType.Undefined, "targets");
		QUnit.equal(viewModel.hasTargetType(cls.EntryType.Album)(), false, "hasTargetType");
	});

	QUnit.test("hasTargetType - set true - all true", () => {
		viewModel.hasTargetType(cls.EntryType.Album)(true);
		viewModel.hasTargetType(cls.EntryType.Song)(true);
		viewModel.hasTargetType(cls.EntryType.ReleaseEvent)(true);
		QUnit.equal(viewModel.targets(), vm.tags.TagEditViewModel.allEntryTypes, "targets"); // When all entry types are selected, flags mask is set to all
	});

}