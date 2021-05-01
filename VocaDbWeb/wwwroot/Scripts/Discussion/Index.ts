import RepositoryFactory from '../Repositories/RepositoryFactory';
import UrlMapper from '../Shared/UrlMapper';
import DiscussionIndexViewModel from '../ViewModels/Discussion/DiscussionIndexViewModel';

const DiscussionIndex = (canDeleteAllComments: boolean) => {
  $(function () {
    moment.locale(vdb.values.culture);

    ko.punches.enableAll();

    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var repoFactory = new RepositoryFactory(
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
