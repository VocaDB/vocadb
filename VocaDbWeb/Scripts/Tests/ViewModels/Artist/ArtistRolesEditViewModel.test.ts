import { AlbumArtistRolesEditViewModel } from '@ViewModels/Artist/ArtistRolesEditViewModel';
import { IEditableArtistWithSupport } from '@ViewModels/ArtistForAlbumEditViewModel';
import ko from 'knockout';

var roleNames: { [key: string]: string } = {
  Arranger: 'Arranger',
  Composer: 'Composer',
  VoiceManipulator: 'Voice manipulator',
};
var viewModel: AlbumArtistRolesEditViewModel;
var artist: IEditableArtistWithSupport = {
  rolesArray: ko.observableArray<string>(['Arranger']),
};

beforeEach(() => {
  viewModel = new AlbumArtistRolesEditViewModel(roleNames);
});

test('constructor', () => {
  expect(viewModel.roleSelections, 'roleSelections').toBeTruthy();
  expect(viewModel.roleSelections.length, 'roleSelections.length').toBe(3);

  var voiceManipulatorRole = viewModel.roleSelections[2];
  expect(voiceManipulatorRole.id, 'voiceManipulatorRole.id').toBe(
    'VoiceManipulator',
  );
  expect(voiceManipulatorRole.name, 'voiceManipulatorRole.name').toBe(
    'Voice manipulator',
  );
});

test('show', () => {
  viewModel.show(artist);

  expect(viewModel.selectedArtist(), 'selectedArtist').toBe(artist);

  var arrangerRole = viewModel.roleSelections[0];
  expect(arrangerRole.id, 'arrangerRole.id').toBe('Arranger');
  expect(arrangerRole.selected(), 'arrangerRole.selected').toBe(true);
});
