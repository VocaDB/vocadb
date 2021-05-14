import $ from 'jquery';
import _ from 'lodash';

declare global {
  interface KnockoutBindingHandlers {
    highcharts: KnockoutBindingHandler;
  }
}

const setHighcharts = (
  element: HTMLElement,
  result: HighchartsOptions,
): void => {
  import('highcharts').then(() => {
    if (
      result &&
      result.series &&
      result.series.length &&
      result.series[0].data &&
      result.series[0].data.length
    ) {
      $(element).show();
      $(element).highcharts(result);
    } else $(element).hide();
  });
};

ko.bindingHandlers.highcharts = {
  update: (element: HTMLElement, valueAccessor): void => {
    var unwrapped = ko.utils.unwrapObservable(valueAccessor());
    if (_.isFunction(unwrapped)) {
      var func = unwrapped;

      func((result: HighchartsOptions) => setHighcharts(element, result));
    } else {
      setHighcharts(element, unwrapped);
    }
  },
};
