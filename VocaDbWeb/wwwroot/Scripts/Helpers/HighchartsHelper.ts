import CountPerDayContract from '@DataContracts/Aggregate/CountPerDayContract';

export interface Tuple2<T1, T2> {
  item1: T1;
  item2: T2;
}

export default class HighchartsHelper {
  public static dateLineChartWithAverage = (
    title: string,
    pointsTitle: string,
    yAxisTitle: string,
    points: CountPerDayContract[],
  ): HighchartsOptions => {
    var dataSeries: HighchartsSeriesOptions = {
      animation: false,
      type: 'area',
      name: pointsTitle,
      data: _.map(points, (p) => [
        Date.UTC(p.year, p.month - 1, p.day),
        p.count,
      ]), // Month numbers start from 0, wtf
      showInLegend: pointsTitle != null,
    } as HighchartsSeriesOptions;

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
    } as HighchartsOptions;
  };

  public static simplePieChart = (
    title: string,
    seriesName: string,
    points: Tuple2<string, number>[],
    backgroundColor: string = null!,
  ): HighchartsOptions => {
    var data: any[] = _.map(points, (p: Tuple2<string, number>) => [
      p.item1,
      p.item2,
    ]);

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
        <any>{
          animation: false,
          name: seriesName,
          data: data,
        },
      ],
    };
  };
}
