
module vdb.helpers {
	
	interface Tuple2<T1, T2> {
		item1: T1;
		item2: T2;
	}

	export class HighchartsHelper {

		public static dateLineChartWithAverage = (title: string, pointsTitle: string, yAxisTitle: string, points: dataContracts.aggregate.CountPerDayContract[],
			average: boolean): HighchartsOptions => {
			
			var averages = points;
		//var averages = (average ? points.Select(p => Tuple.Create(p.Item1, Math.Floor(points.Where(p2 => p2.Item1 >= p.Item1 - TimeSpan.FromDays(182) && p2.Item1 <= p.Item1 + TimeSpan.FromDays(182)).Average(p3 => p3.Item2)))).ToArray() : new Tuple < DateTime, double > [0]);

			var dataSeries: HighchartsSeriesOptions = {
				type: 'area',
				name: pointsTitle,
				data: _.map(points, p => [Date.UTC(p.year, p.month, p.day), p.count]),
				showInLegend: pointsTitle != null
			};

		return {
			chart: {
				animation: false,
				height: 300
			},
			title: title,
				xAxis: { type: 'datetime'},
				yAxis: {
					title: yAxisTitle,
					min: 0
				},
				tooltip: {
					shared: true,
					crosshairs: true
				},
				plotOptions: {
					series: {
						enableMouseTracking: false
					},
					bar: {
						dataLabels : {
							enabled: true
						}
					}
				},
				legend: {
					layout: "vertical",
					align: "left",
					x: 120,
					verticalAlign: "top",
					y: 100,
					floating: true,
					backgroundColor: "#FFFFFF"
				},
				series: (average ? [
					dataSeries,
					<any>{
						type: 'spline',
						name: "Average",
						data: _.map(averages, p => [ Date.UTC(p.year, p.month, p.day), p.count ]),
						marker: {
							enabled: false
						},
						lineWidth: 4
				}] : [ dataSeries ])
			};

		}

		public static simplePieChart = (title: string, seriesName: string, points: Tuple2<string, number>[], backgroundColor: string = null): HighchartsOptions => {

			var data: any[] = _.map(points, (p: Tuple2<string, number>) => [ p.item1, p.item2 ]);

			return {
				chart: {
					animation: false,
					type: "pie",
					backgroundColor: backgroundColor,
				},
				title: {
					text: title
				},
				xAxis: {
					title: {
						text: null
					}
				},
				yAxis: {
					title: {
						text: seriesName
					}
				},
				plotOptions: {
					series: {
						enableMouseTracking: false
					},
					pie: {
						dataLabels: {
							enabled: true,
							format: "<b>{point.name}</b>: {point.y}"
						}
					}
				},
				legend: {
					enabled: false
				},
				series: [<any>{
					animation: false,
					name: seriesName,
					data: data
				}]			
			};

		}

	}

}