window.$ = window.jQuery = require('jquery');

require('bootstrap-sass');

require('jquery-ui/accordion');
require('jquery-ui/autocomplete');
require('jquery-ui/button');
require('jquery-ui/datepicker');
require('jquery-ui/dialog');
require('jquery-ui/draggable');
require('jquery-ui/droppable');
require('jquery-ui/menu');
require('jquery-ui/mouse');
require('jquery-ui/progressbar');
require('jquery-ui/resizable');
require('jquery-ui/selectable');
require('jquery-ui/slider');
require('jquery-ui/sortable');
require('jquery-ui/spinner');
require('jquery-ui/tabs');

window.ko = require('knockout');

require('knockout-punches');

window._ = require('lodash');

require('qtip2');

window.marked = require('marked');

$.postJSON = function (url, data, success, dataType) {
	return $.ajax({ url: url, type: "POST", dataType: dataType, contentType: "application/json", data: ko.toJSON(data), success: success });
};
