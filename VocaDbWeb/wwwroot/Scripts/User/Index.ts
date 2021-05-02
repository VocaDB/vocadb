import RepositoryFactory from '../Repositories/RepositoryFactory';
import UrlMapper from '../Shared/UrlMapper';
import ListUsersViewModel from '../ViewModels/User/ListUsersViewModel';

const UserIndex = (model: { filter: string; groupId: string }) => {
  $(function () {
    var cultureCode = vdb.values.culture;
    var uiCultureCode = vdb.values.uiCulture;
    moment.locale(cultureCode);

    var filter = model.filter;
    var groupId = model.groupId;
    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var repoFactory = new RepositoryFactory(urlMapper);
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
