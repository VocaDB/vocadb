using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Web.Code.Highcharts;

namespace VocaDb.Web.Helpers
{

	public static class HighchartsHelper
	{

		public static double ToEpochTime(DateTime date)
		{
			return (date - new DateTime(1970, 1, 1)).TotalMilliseconds;
		}

		public static Highchart DateLineChartWithAverage(string title, string pointsTitle, string yAxisTitle, ICollection<Tuple<DateTime, int>> points,
			bool average = true)
		{

			var averages = (average ? points.Select(p => Tuple.Create(p.Item1, Math.Floor(points.Where(p2 => p2.Item1 >= p.Item1 - TimeSpan.FromDays(182) && p2.Item1 <= p.Item1 + TimeSpan.FromDays(182)).Average(p3 => p3.Item2)))).ToArray() : new Tuple<DateTime, double>[0]);

			var dataSeries = new Series
			{
				Type = SeriesType.Area,
				Name = pointsTitle,
				Data = points.Select(p => new object[] { ToEpochTime(p.Item1), p.Item2 }).ToArray()
			};

			var series = (average ? new[] {
					dataSeries,
					new Series {
						Type = SeriesType.Spline,
						Name = "Average",
						Data = averages.Select(p => new object[] { ToEpochTime(p.Item1), p.Item2 }).ToArray(),
						Marker = new {
							enabled = false
						},
						LineWidth = 4
					}
				}
				: new[] { dataSeries });

			return DateLineChart(title, pointsTitle, yAxisTitle, series);

		}

		public static Highchart DateLineChart(string title, string pointsTitle, string yAxisTitle, ICollection<Series> series)
		{

			return new Highchart
			{
				Chart = new Chart
				{
					Height = 600
				},
				Title = title,
				XAxis = new Axis(AxisType.Datetime, new Title()),
				YAxis = new Axis
				{
					Title = yAxisTitle,
					Min = 0,
				},
				Tooltip = new
				{
					Shared = true,
					Crosshairs = true
				},
				PlotOptions = new PlotOptions
				{
					Bar = new
					{
						DataLabels = new
						{
							Enabled = true
						}
					}
				},
				Legend = new
				{
					Layout = "vertical",
					Align = "left",
					X = 120,
					VerticalAlign = "top",
					Y = 100,
					Floating = true,
					BackgroundColor = "#FFFFFF"
				},
				Series = series
			};

		}

		public static Highchart SimplePieChart(string title, string seriesName, ICollection<Tuple<string, int>> points, bool transparentBackground)
		{

			return new Highchart
			{
				Chart = new Chart
				{
					Type = ChartType.Pie,
					BackgroundColor = (transparentBackground ? (string)null : "#FFFFFF"),
					//height = 200
				},
				Title = title,
				XAxis = new Axis
				{
					Title = new Title()
				},
				YAxis = new Axis
				{
					Title = seriesName
				},
				PlotOptions = new PlotOptions
				{
					Pie = new
					{
						DataLabels = new
						{
							Enabled = true,
							Format = "<b>{point.name}</b>: {point.y}"
						}
					}
				},
				Legend = new
				{
					Enabled = false
				},
				Series = new[] {
					new Series {
						Name = seriesName,
						Data = points.Select(p => new object[] { p.Item1, p.Item2 }).ToArray()
					}
				}

			};

		}

	}

}