import RepositoryFactory from '../Repositories/RepositoryFactory';
import HttpClient from '../Shared/HttpClient';
import UrlMapper from '../Shared/UrlMapper';
import ListUsersViewModel from '../ViewModels/User/ListUsersViewModel';

const UserIndex = (model: { filter: string; groupId: string }): void => {
  $(function () {
    var cultureCode = vdb.values.culture;
    var uiCultureCode = vdb.values.uiCulture;
    moment.locale(cultureCode);

    var filter = model.filter;
    var groupId = model.groupId;
    const httpClient = new HttpClient();
    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var repoFactory = new RepositoryFactory(httpClient, urlMapper);
    var repo = repoFactory.userRepository();
    var resourceRepo = repoFactory.resourceRepository();
    var viewModel = new ListUsersViewModel(
      repo,
      resourceRepo,
      uiCultureCode,
      filter,
      groupId,
    );
    ko.applyBindings(viewModel);
  });
};

export default UserIndex;
