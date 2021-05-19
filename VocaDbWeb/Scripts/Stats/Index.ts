import StatsViewModel from '@ViewModels/StatsViewModel';
import $ from 'jquery';

const StatsIndex = (): void => {
  $(function () {
    var vm = new StatsViewModel();
    ko.applyBindings(vm);
  });
};

export default StatsIndex;
