import RepositoryFactory from '@Repositories/RepositoryFactory';
import HttpClient from '@Shared/HttpClient';
import RequestVerificationViewModel from '@ViewModels/User/RequestVerificationViewModel';
import $ from 'jquery';
import ko from 'knockout';

const UserRequestVerification = (): void => {
  $(document).ready(function () {
    const httpClient = new HttpClient();
    var repoFactory = new RepositoryFactory(httpClient);
    var artistRepo = repoFactory.artistRepository();
    ko.applyBindings(new RequestVerificationViewModel(artistRepo));
  });
};

export default UserRequestVerification;
