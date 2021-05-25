import $ from 'jquery';
import _ from 'lodash';

export default class AjaxHelper {
  public static createUrl = (params: {
    [key: string]: string[] | number[];
  }): string | null => {
    if (!params) return null;

    var par: string[] = [];

    _.forOwn(params, (val, key) => {
      par.push(
        key +
          '=' +
          _.map(val as string[], (v) => encodeURIComponent(v || '')).join(
            '&' + key + '=',
          ),
      );
    });

    var query = par.join('&');
    return query;
  };

  public static deleteJSON_Url = (
    url: string,
    dataParamName: string,
    data: any[],
    success?: any,
  ): void => {
    var dataParam =
      '?' + dataParamName + '=' + data.join('&' + dataParamName + '=');
    $.ajax(url + dataParam, { type: 'DELETE', success: success });
  };

  // Issues a PUT request with JSON-formatted body.
  public static putJSON = (
    url: string,
    data?: any,
    success?: any,
  ): JQueryXHR => {
    return $.ajax(url, {
      type: 'PUT',
      contentType: 'application/json; charset=utf-8',
      success: success,
      data: JSON.stringify(data),
    });
  };
}
