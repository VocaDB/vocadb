import RepositoryFactory from '../Repositories/RepositoryFactory';
import UrlMapper from '../Shared/UrlMapper';
import RequestVerificationViewModel from '../ViewModels/User/RequestVerificationViewModel';

const UserRequestVerification = () => {
  $(document).ready(function () {
    var repoFactory = new RepositoryFactory(
      new UrlMapper(vdb.values.baseAddress),
      vdb.values.languagePreference,
    );
    var artistRepo = repoFactory.artistRepository();
    ko.applyBindings(new RequestVerificationViewModel(artistRepo));
  });
};

export default UserRequestVerification;
