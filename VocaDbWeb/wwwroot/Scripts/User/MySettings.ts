import UserKnownLanguageContract from '@DataContracts/User/UserKnownLanguageContract';
import WebLinkContract from '@DataContracts/WebLinkContract';
import UserRepository from '@Repositories/UserRepository';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import MySettingsViewModel from '@ViewModels/User/MySettingsViewModel';
import $ from 'jquery';

const UserMySettings = (model: {
  aboutMe: string;
  email: string;
  emailVerified: boolean;
  knownLanguages: UserKnownLanguageContract[];
  webLinks: WebLinkContract[];
}): void => {
  $(document).ready(function () {
    $('#tabs').tabs();

    const httpClient = new HttpClient();
    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var repository = new UserRepository(httpClient, urlMapper);

    var viewModel = new MySettingsViewModel(
      repository,
      model.aboutMe,
      model.email,
      model.emailVerified,
      model.webLinks,
      model.knownLanguages,
    );
    ko.applyBindings(viewModel);
  });
};

export default UserMySettings;
