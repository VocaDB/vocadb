import $ from 'jquery';
import _ from 'lodash';

declare global {
  interface JQueryStatic {
    url: (param: string) => string;
  }
}

// Uses HTML5 history API to update URL query string from a set of observables
export default class ObservableUrlParamRouter {
  constructor(
    routes: { [key: string]: KnockoutObservable<string | number> },
    private win: Window = window,
  ) {
    for (var route in routes) {
      if (routes.hasOwnProperty(route)) {
        var data = new ParamData(route, routes[route], $.url('?' + route));
        this.paramDatas.push(data);
      }
    }

    this.queryString = ko.computed(() => {
      return _.chain(this.paramDatas)
        .filter((p) => p.hasValue())
        .map(
          (p) =>
            encodeURIComponent(p.name) +
            '=' +
            encodeURIComponent(p.observable()),
        )
        .value()
        .join('&');
    });

    this.queryString.subscribe((val) => {
      if (!this.popState) win.history.pushState(val, null!, '?' + val);
    });

    win.onpopstate = (event): void => {
      if (!event) return;

      // History state includes the query string as key/value pairs separated by "&"
      var datas: string = event.state || '';
      var params = _.map(datas.split('&'), (z) =>
        z.split('=').map((v) => decodeURIComponent(v)),
      );
      var dict = _.fromPairs(params);

      _.each(this.paramDatas, (paramData) => {
        this.popState = true;

        // Set observable value to either value from the route or initial value if the value is not present
        paramData.observable(dict[paramData.name] || paramData.initialValue);

        this.popState = false;
      });
    };
  }

  // Whether currently processing popstate. This is to prevent adding the previous state to history.
  private popState = false;

  private paramDatas: ParamData[] = [];

  private queryString: KnockoutComputed<string>;
}

class ParamData {
  constructor(
    public name: string,
    public observable: KnockoutObservable<any>,
    queryValue: string,
  ) {
    this.initialValue = this.observable();
    observable.subscribe((val) => this.hasValue(val !== this.initialValue));
    if (queryValue) this.observable(queryValue !== 'null' ? queryValue : null);
  }

  // Whether the value has changed from the initial value.
  public hasValue = ko.observable(false);

  // Initial (default) value.
  public initialValue: any;
}
