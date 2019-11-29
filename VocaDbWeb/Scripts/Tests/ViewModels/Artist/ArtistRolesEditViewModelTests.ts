
//module vdb.tests.viewModels.artists {
	
	import vm = vdb.viewModels;

	var roleNames: { [key: string]: string; } = { 'Arranger': 'Arranger', 'Composer': 'Composer', 'VoiceManipulator': 'Voice manipulator' };
	var viewModel: vm.artists.AlbumArtistRolesEditViewModel;
	var artist: vm.IEditableArtistWithSupport = { rolesArray: ko.observableArray<string>(['Arranger']) };

	QUnit.module("ArtistRolesEditViewModel", {
		setup: () => {

			viewModel = new vm.artists.AlbumArtistRolesEditViewModel(roleNames);

		}
	});


	QUnit.test("constructor", () => {

		QUnit.ok(viewModel.roleSelections, "roleSelections");
		QUnit.equal(viewModel.roleSelections.length, 3, "roleSelections.length");
		
		var voiceManipulatorRole = viewModel.roleSelections[2];
		QUnit.equal(voiceManipulatorRole.id, "VoiceManipulator", "voiceManipulatorRole.id");
		QUnit.equal(voiceManipulatorRole.name, "Voice manipulator", "voiceManipulatorRole.name");

	});

	QUnit.test("show", () => {

		viewModel.show(artist);

		QUnit.equal(viewModel.selectedArtist(), artist, "selectedArtist");
		
		var arrangerRole = viewModel.roleSelections[0];
		QUnit.equal(arrangerRole.id, "Arranger", "arrangerRole.id");
		QUnit.equal(arrangerRole.selected(), true, "arrangerRole.selected");

	});

//} 