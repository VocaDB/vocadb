import { StatsViewModel } from '@/ViewModels/StatsViewModel';
import $ from 'jquery';
import ko from 'knockout';

export const StatsIndex = (): void => {
	$(function () {
		var vm = new StatsViewModel();
		ko.applyBindings(vm);
	});
};
