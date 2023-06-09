import { CountPerDayContract } from '@/types/DataContracts/Aggregate/CountPerDayContract';
import { Options, SeriesOptions } from 'highcharts';

export interface Tuple2<T1, T2> {
	item1: T1;
	item2: T2;
}

export class HighchartsHelper {
	static dateLineChartWithAverage = (
		title: string,
		pointsTitle: string,
		yAxisTitle: string,
		points: CountPerDayContract[]
	): Options => {
		var dataSeries: SeriesOptions = {
			animation: false,
			type: 'area',
			name: pointsTitle,
			data: points.map((p) => [Date.UTC(p.year, p.month - 1, p.day), p.count]), // Month numbers start from 0, wtf
			showInLegend: pointsTitle != null,
		} as SeriesOptions;

		return {
			chart: {
				animation: false,
				height: 300,
			},
			title: title,
			xAxis: { type: 'datetime' },
			yAxis: {
				title: yAxisTitle,
				min: 0,
				tickInterval: 1,
			},
			tooltip: {
				shared: true,
				crosshairs: true,
			},
			plotOptions: {
				series: {
					enableMouseTracking: false,
				},
				bar: {
					dataLabels: {
						enabled: true,
					},
				},
			},
			legend: {
				layout: 'vertical',
				align: 'left',
				x: 120,
				verticalAlign: 'top',
				y: 100,
				floating: true,
				backgroundColor: '#FFFFFF',
			},
			series: [dataSeries],
		} as Options;
	};

	static simplePieChart = (
		title: string,
		seriesName: string,
		points: Tuple2<string, number>[],
		backgroundColor: string = null!
	): Options => {
		var data: any[] = points.map((p: Tuple2<string, number>) => [p.item1, p.item2]);

		return {
			chart: {
				animation: false,
				type: 'pie',
				backgroundColor: backgroundColor,
			},
			title: {
				text: title,
			},
			xAxis: {
				title: {
					text: null!,
				},
			},
			yAxis: {
				title: {
					text: seriesName,
				},
			},
			plotOptions: {
				series: {
					enableMouseTracking: false,
				},
				pie: {
					dataLabels: {
						enabled: true,
						format: '<b>{point.name}</b>: {point.y}',
					},
				},
			},
			legend: {
				enabled: false,
			},
			series: [
				{
					animation: false,
					name: seriesName,
					data: data,
				} as any,
			],
		};
	};
}
