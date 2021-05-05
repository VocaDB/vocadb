import RepositoryFactory from '../Repositories/RepositoryFactory';
import HttpClient from '../Shared/HttpClient';
import UrlMapper from '../Shared/UrlMapper';
import DiscussionIndexViewModel from '../ViewModels/Discussion/DiscussionIndexViewModel';

const DiscussionIndex = (canDeleteAllComments: boolean) => {
  $(function () {
    moment.locale(vdb.values.culture);

    ko.punches.enableAll();

    const httpClient = new HttpClient();
    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var repoFactory = new RepositoryFactory(
      httpClient,
      urlMapper,
      vdb.values.languagePreference,
    );
    var repo = repoFactory.discussionRepository();
    ko.applyBindings(
      new DiscussionIndexViewModel(
        repo,
        urlMapper,
        canDeleteAllComments,
        vdb.values.loggedUserId,
      ),
    );
  });
};

export default DiscussionIndex;
