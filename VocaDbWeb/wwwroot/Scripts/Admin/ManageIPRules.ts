import AdminRepository from '../Repositories/AdminRepository';
import HttpClient from '../Shared/HttpClient';
import UrlMapper from '../Shared/UrlMapper';
import ManageIPRulesViewModel, {
  IPRuleContract,
} from '../ViewModels/Admin/ManageIPRulesViewModel';

const AdminManageIPRules = (model: IPRuleContract[]): void => {
  $(function () {
    moment.locale(vdb.values.culture);
    ko.punches.enableAll();

    var rules = model;
    const httpClient = new HttpClient();
    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var repo = new AdminRepository(httpClient, urlMapper);

    var viewModel = new ManageIPRulesViewModel(rules, repo);
    ko.applyBindings(viewModel);
  });
};

export default AdminManageIPRules;
