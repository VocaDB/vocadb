/// <reference path="../typings/highcharts/highcharts.d.ts" />

declare global {
	interface KnockoutBindingHandlers {
		highcharts: KnockoutBindingHandler;
	}
}

//module vdb.knockoutExtensions.highcharts {

	export function setHighcharts(element: HTMLElement, result: HighchartsOptions) {
	
		if (result && result.series && result.series.length && result.series[0].data && result.series[0].data.length) {
			$(element).show();
			$(element).highcharts(result);
		} else
			$(element).hide();
			
	}

//}

ko.bindingHandlers.highcharts = {
	update: (element: HTMLElement, valueAccessor) => {

		var unwrapped = ko.utils.unwrapObservable(valueAccessor());
		if (_.isFunction(unwrapped)) {

			var func = unwrapped;

			func((result: HighchartsOptions) => setHighcharts(element, result));
			
		} else {
			
			setHighcharts(element, unwrapped);

		}

	}
};
