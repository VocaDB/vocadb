/// <reference path="../typings/highcharts/highcharts.d.ts" />

interface KnockoutBindingHandlers {
	highcharts: KnockoutBindingHandler;
}

ko.bindingHandlers.highcharts = {
	init: (element: HTMLElement, valueAccessor) => {

		var func = ko.utils.unwrapObservable<Function>(valueAccessor());

		func((result: HighchartsOptions) => {
			if (result && result.series && result.series.length && result.series[0].data && result.series[0].data.length)
				$(element).highcharts(result);
			else
				$(element).hide();
		});

	}
};
