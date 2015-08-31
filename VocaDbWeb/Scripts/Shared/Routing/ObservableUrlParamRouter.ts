
interface JQueryStatic {
	url: (param: string) => string;
}

module vdb.routing {

	// Uses HTML5 history API to update URL query string from a set of observables
	export class ObservableUrlParamRouter {
		
		constructor(routes: { [key: string]: KnockoutObservable<string | number>; }) {
			
			for (var route in routes) {
				if (routes.hasOwnProperty(route)) {
					var data = new ParamData(route, routes[route], $.url('?' + route));
					this.paramDatas.push(data);
				}
			}

			this.queryString = ko.computed(() => {

				return _
					.chain(this.paramDatas)
					.filter(p => p.hasValue())
					.map(p => encodeURIComponent(p.name) + "=" + encodeURIComponent(p.observable()))
					.value()
					.join("&");

			});

			this.queryString.subscribe(val => {
				window.history.pushState(null, null, "?" + val);
			});

		}

		private paramDatas: ParamData[] = [];

		private queryString: KnockoutComputed<string>;

	}

	class ParamData {

		constructor(public name: string, public observable: KnockoutObservable<any>, queryValue: string) {
			observable.subscribe(() => this.hasValue(true));
			if (queryValue)
				this.observable(queryValue !== "null" ? queryValue : null);
		}

		public hasValue = ko.observable(false);

	}

}