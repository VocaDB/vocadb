import RepositoryFactory from '../Repositories/RepositoryFactory';
import HttpClient from '../Shared/HttpClient';
import UrlMapper from '../Shared/UrlMapper';
import RequestVerificationViewModel from '../ViewModels/User/RequestVerificationViewModel';

const UserRequestVerification = (): void => {
  $(document).ready(function () {
    const httpClient = new HttpClient();
    var repoFactory = new RepositoryFactory(
      httpClient,
      new UrlMapper(vdb.values.baseAddress),
      vdb.values.languagePreference,
    );
    var artistRepo = repoFactory.artistRepository();
    ko.applyBindings(new RequestVerificationViewModel(artistRepo));
  });
};

export default UserRequestVerification;
