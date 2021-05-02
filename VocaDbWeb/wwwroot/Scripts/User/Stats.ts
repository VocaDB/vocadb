const UserStats = (model: { id: number }, type: string) => {
  $(function () {
    var userId = model.id;

    $.getJSON('/User/Stats_' + type + '/' + userId, function (data) {
      $('#chartContainer').highcharts(data);
    });
  });
};

export default UserStats;
