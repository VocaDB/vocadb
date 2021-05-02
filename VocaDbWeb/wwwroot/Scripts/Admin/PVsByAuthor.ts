import functions from '../Shared/GlobalFunctions';

const AdminPVsByAuthor = () => {
  $(document).ready(function () {
    $('#author').autocomplete({
      source: functions.mapAbsoluteUrl('/Admin/PVAuthorNames'),
    });
  });
};

export default AdminPVsByAuthor;
