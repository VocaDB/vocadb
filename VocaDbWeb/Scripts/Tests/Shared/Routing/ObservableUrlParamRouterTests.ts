
module vdb.tests.shared.routing {

	interface HistoryEntry {
		data: any;
		url: string;
	}
	
	var win: Window;
	var history: HistoryEntry[];
	var testObject: any;

    QUnit.module("ObservableUrlParamRouter", {
        setup: () => {

			testObject = {
				vocaloid: ko.observable("Luka"),
				song: ko.observable("Just-Be-Friends")
			};

			history = new Array<HistoryEntry>();

			win = <any>{
				history: {
					pushState: (statedata, title: string, url?: string) => history.push({ data: statedata, url: url }),
					go: (steps: number) => {
						for (var i = 0; i < -steps; i++) {
							var latest = history.pop();
							if (win.onpopstate)
								win.onpopstate(<any>{ state: latest.data });							
						}
					}
				}				
			};

			jQuery.url = () => null;

        }
    });

	QUnit.test("changing observable adds history entry", () => {

		new vdb.routing.ObservableUrlParamRouter(testObject, win);

		testObject.vocaloid("Miku");

		QUnit.equal(history.length, 1, "Added history entry");
		QUnit.equal(history[0].url, "?vocaloid=Miku", "history entry URL");

	});

	QUnit.test("changing two observables adds history entry for both", () => {

		new vdb.routing.ObservableUrlParamRouter(testObject, win);

		testObject.vocaloid("Miku");
		testObject.song("Nebula");

		QUnit.equal(history.length, 2, "Added history entries");
		QUnit.equal(history[1].url, "?vocaloid=Miku&song=Nebula", "history entry URL");

	});

	QUnit.test("going back changes observables", () => {

		new vdb.routing.ObservableUrlParamRouter(testObject, win);

		testObject.vocaloid("Miku");
		testObject.song("Nebula");
		win.history.go(-1);

		QUnit.equal(history.length, 1, "Removed history entry");
		QUnit.equal(history[0].url, "?vocaloid=Miku", "history entry URL");

	});

}