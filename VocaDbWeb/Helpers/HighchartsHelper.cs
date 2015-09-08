using System;
using System.Collections.Generic;
using System.Linq;

namespace VocaDb.Web.Helpers {

	public static class HighchartsHelper {

		public static double ToEpochTime(DateTime date) {
			return (date - new DateTime(1970, 1, 1)).TotalMilliseconds;
		}

		public static object DateLineChartWithAverage(string title, string pointsTitle, string yAxisTitle, ICollection<Tuple<DateTime, int>> points,
			bool average = true) {
			
			var averages = (average ? points.Select(p => Tuple.Create(p.Item1, Math.Floor(points.Where(p2 => p2.Item1 >= p.Item1 - TimeSpan.FromDays(182) && p2.Item1 <= p.Item1 + TimeSpan.FromDays(182)).Average(p3 => p3.Item2)))).ToArray() : new Tuple<DateTime, double>[0]);

			var dataSeries = new {
				type = "area",
				name = pointsTitle,
				data = points.Select(p => new object[] { ToEpochTime(p.Item1), p.Item2 }).ToArray()
			};

			return new {
				chart = new {
					height = 600
				},
				title = new {
					text = title
				},
				xAxis = new {
					type = "datetime",
					title = new {
						text = (string)null
					},
				},
				yAxis = new {
					title = new {
						text = yAxisTitle
					},
					min = 0,
				},
				tooltip = new {
					shared = true,
					crosshairs = true
				},
				plotOptions = new {
					bar = new {
						dataLabels = new {
							enabled = true
						}
					}
				},
				legend = new {
						layout = "vertical",
						align = "left",
						x = 120,
						verticalAlign = "top",
						y = 100,
						floating = true,
						backgroundColor = "#FFFFFF"
				},
				series = (average ? new Object[] {
					dataSeries,
					new {
						type = "spline",
						name = "Average",
						data = averages.Select(p => new object[] { ToEpochTime(p.Item1), p.Item2 }).ToArray(),
						marker = new {
							enabled = false
						},
						lineWidth = 4
					}
				}
				: new Object[] { dataSeries })				
			};

		}

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