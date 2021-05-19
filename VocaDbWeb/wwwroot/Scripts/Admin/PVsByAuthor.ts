import functions from '@Shared/GlobalFunctions';
import $ from 'jquery';

const AdminPVsByAuthor = (): void => {
  $(document).ready(function () {
    $('#author').autocomplete({
      source: functions.mapAbsoluteUrl('/Admin/PVAuthorNames'),
    });
  });
};

export default AdminPVsByAuthor;
