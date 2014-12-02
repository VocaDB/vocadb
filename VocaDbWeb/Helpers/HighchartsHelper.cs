using System;
using System.Collections.Generic;
using System.Linq;

namespace VocaDb.Web.Helpers {

	public static class HighchartsHelper {

		public static object SimplePieChart(string title, string seriesName, ICollection<Tuple<string, int>> points, bool transparentBackground) {
			
			return new {
				chart = new {
					type = "pie",
					backgroundColor = (transparentBackground ? (string)null : "#FFFFFF"),
					//height = 200
				},
				title = new {
					text = title
				},
				xAxis = new {
					title = new {
						text = (string)null
					}
				},
				yAxis = new {
					title = new {
						text = seriesName
					}
				},
				plotOptions = new {
					pie = new {
						dataLabels = new {
							enabled = true,
							format = "<b>{point.name}</b>: {point.y}"
						}
					}
				},
				legend = new {
					enabled = false
				},
				series = new Object[] {
					new {
						name = seriesName,
						data = points.Select(p => new object[] { p.Item1, p.Item2 }).ToArray()
					}
				}
				
			};

		}


	}

}