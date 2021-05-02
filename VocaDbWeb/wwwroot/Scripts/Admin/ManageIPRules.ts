import AdminRepository from '../Repositories/AdminRepository';
import UrlMapper from '../Shared/UrlMapper';
import ManageIPRulesViewModel, {
  IPRuleContract,
} from '../ViewModels/Admin/ManageIPRulesViewModel';

const AdminManageIPRules = (model: IPRuleContract[]) => {
  $(function () {
    moment.locale(vdb.values.culture);
    ko.punches.enableAll();

    var rules = model;
    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var repo = new AdminRepository(urlMapper);

    var viewModel = new ManageIPRulesViewModel(rules, repo);
    ko.applyBindings(viewModel);
  });
};

export default AdminManageIPRules;
