import { Options } from 'highcharts';
import $ from 'jquery';
import ko from 'knockout';
import _ from 'lodash';

declare global {
	interface KnockoutBindingHandlers {
		highcharts: KnockoutBindingHandler;
	}
}

const setHighcharts = (element: HTMLElement, result: Options): void => {
	import('highcharts').then(() => {
		if (
			result &&
			result.series &&
			result.series.length &&
			(result.series[0] as any).data &&
			(result.series[0] as any).data.length
		) {
			$(element).show();
			$(element).highcharts(result as any);
		} else $(element).hide();
	});
};

ko.bindingHandlers.highcharts = {
	update: (element: HTMLElement, valueAccessor): void => {
		var unwrapped = ko.utils.unwrapObservable(valueAccessor());
		if (_.isFunction(unwrapped)) {
			var func = unwrapped;

			func((result: Options) => setHighcharts(element, result));
		} else {
			setHighcharts(element, unwrapped);
		}
	},
};
