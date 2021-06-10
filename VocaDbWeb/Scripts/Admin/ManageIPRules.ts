import RepositoryFactory from '@Repositories/RepositoryFactory';
import vdb from '@Shared/VdbStatic';
import { container } from '@Shared/inversify.config';
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
		var repoFactory = container.get(RepositoryFactory);
		var repo = repoFactory.adminRepository();

		var viewModel = new ManageIPRulesViewModel(rules, repo);
		ko.applyBindings(viewModel);
	});
};

export default AdminManageIPRules;
