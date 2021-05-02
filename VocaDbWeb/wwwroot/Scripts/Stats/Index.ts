import StatsViewModel from '../ViewModels/StatsViewModel';

const StatsIndex = () => {
  $(function () {
    var vm = new StatsViewModel();
    ko.applyBindings(vm);
  });
};

export default StatsIndex;
