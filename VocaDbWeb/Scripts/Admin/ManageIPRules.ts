import AdminRepository from '@Repositories/AdminRepository';
import HttpClient from '@Shared/HttpClient';
import vdb from '@Shared/VdbStatic';
import ManageIPRulesViewModel, {
  IPRuleContract,
} from '@ViewModels/Admin/ManageIPRulesViewModel';
import $ from 'jquery';
import ko from 'knockout';
import moment from 'moment';

const AdminManageIPRules = (model: IPRuleContract[]): void => {
  $(function () {
    moment.locale(vdb.values.culture);
    ko.punches.enableAll();

    var rules = model;
    const httpClient = new HttpClient();
    var repo = new AdminRepository(httpClient);

    var viewModel = new ManageIPRulesViewModel(rules, repo);
    ko.applyBindings(viewModel);
  });
};

export default AdminManageIPRules;
