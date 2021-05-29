import StatsViewModel from '@ViewModels/StatsViewModel';
import $ from 'jquery';
import ko from 'knockout';

const StatsIndex = (): void => {
  $(function () {
    var vm = new StatsViewModel();
    ko.applyBindings(vm);
  });
};

export default StatsIndex;
