import $ from 'jquery';

const UserStats = (model: { id: number }, type: string): void => {
  $(function () {
    var userId = model.id;

    $.getJSON('/User/Stats_' + type + '/' + userId, function (data) {
      import('highcharts').then(() => {
        $('#chartContainer').highcharts(data);
      });
    });
  });
};

export default UserStats;
