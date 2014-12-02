
module vdb.helpers {
	
	interface Tuple2<T1, T2> {
		item1: T1;
		item2: T2;
	}

	export class HighchartsHelper {
		
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
				series: [{
					animation: false,
					name: seriesName,
					data: data
				}]			
			};

		}

	}

}