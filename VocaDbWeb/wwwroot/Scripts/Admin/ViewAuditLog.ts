import functions from '../Shared/GlobalFunctions';
import ViewAuditLogViewModel from '../ViewModels/Admin/ViewAuditLogViewModel';

const AdminViewAuditLog = (model): void => {
  $(document).ready(function () {
    var json = model;
    var vm = new ViewAuditLogViewModel(json);
    ko.applyBindings(vm);

    $('#loadMoreLink').click(function () {
      var params = $.extend({ start: $('#start').val() }, json);

      $.post('AuditLogEntries', params, function (result) {
        $('#logEntries').append(result);
        $('#start').val(`${parseInt($('#start').val()) + 200}`);
      });
      return false;
    });

    // HACK: replacement for Html.RenderAction.
    // TODO: replace with view components
    // Code from: https://stackoverflow.com/questions/41353874/equivalent-of-html-renderaction-in-asp-net-core/45494932#45494932
    $.get(
      functions.mapAbsoluteUrl('/Admin/AuditLogEntries'),
      model,
      function (content) {
        $('#logEntries').html(content);
      },
    );
  });
};

export default AdminViewAuditLog;
