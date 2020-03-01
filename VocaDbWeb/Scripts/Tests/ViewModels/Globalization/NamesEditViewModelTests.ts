import ContentLanguageSelection from '../../../Models/Globalization/ContentLanguageSelection';
import LocalizedStringWithIdEditViewModel from '../../../ViewModels/Globalization/LocalizedStringWithIdEditViewModel';
import NamesEditViewModel from '../../../ViewModels/Globalization/NamesEditViewModel';

//module vdb.tests.viewModels.globalization {

	QUnit.module("NamesEditViewModelTests");

	QUnit.test("primary name and aliases", assert => {

		var names = [
			new LocalizedStringWithIdEditViewModel(ContentLanguageSelection.English, "English name", 1),
			new LocalizedStringWithIdEditViewModel(ContentLanguageSelection.Unspecified, "Alias", 2)
		];

		var viewModel = new NamesEditViewModel(names);

		assert.equal(viewModel.englishName.value(), "English name", "englishName");
		assert.equal(viewModel.originalName.value(), "", "originalName");
		assert.equal(viewModel.aliases().length, 1, "aliases");
		assert.equal(viewModel.aliases()[0].value(), "Alias", "aliases");
		assert.equal(viewModel.hasPrimaryName(), true, "hasPrimaryName");

	});

	QUnit.test("only primary name", assert => {

		var names = [
			new LocalizedStringWithIdEditViewModel(ContentLanguageSelection.Japanese, "Japanese name", 1)
		];

		var viewModel = new NamesEditViewModel(names);

		assert.equal(viewModel.englishName.value(), "", "englishName");
		assert.equal(viewModel.originalName.value(), "Japanese name", "originalName");
		assert.equal(viewModel.aliases().length, 0, "aliases");
		assert.equal(viewModel.hasPrimaryName(), true, "hasPrimaryName");

	});

	QUnit.test("only aliases", assert => {

		var names = [
			new LocalizedStringWithIdEditViewModel(ContentLanguageSelection.Unspecified, "Alias", 1)
		];

		var viewModel = new NamesEditViewModel(names);

		assert.equal(viewModel.englishName.value(), "", "englishName");
		assert.equal(viewModel.originalName.value(), "", "originalName");
		assert.equal(viewModel.aliases().length, 1, "aliases");
		assert.equal(viewModel.aliases()[0].value(), "Alias", "aliases");
		assert.equal(viewModel.hasPrimaryName(), false, "hasPrimaryName");

	});

//} 