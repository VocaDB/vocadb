import ObservableUrlParamRouter from '@Shared/Routing/ObservableUrlParamRouter';
import jQuery from 'jquery';
import ko from 'knockout';

interface HistoryEntry {
	data: any;
	url: string;
}

var win: Window;
var history: HistoryEntry[];
var testObject: any;

beforeEach(() => {
	testObject = {
		vocaloid: ko.observable('Luka'),
		song: ko.observable('magnet'),
	};

	history = new Array<HistoryEntry>();

	win = {
		history: {
			pushState: (statedata: any, title: string, url?: string) =>
				history.push({ data: statedata, url: url! }),
			go: (steps: number) => {
				for (var i = 0; i < -steps; i++) {
					history.pop();
					var latest = history.length ? history[history.length - 1] : null;
					if (win.onpopstate) win.onpopstate({ state: latest!.data } as any);
				}
			},
		},
	} as any;

	jQuery.url = (): string => null!;
});

var initRouter = (): void => {
	new ObservableUrlParamRouter(testObject, win);
};

test('changing observable adds history entry', () => {
	initRouter();

	testObject.vocaloid('Miku');

	expect(history.length, 'Added history entry').toBe(1);
	expect(history[0].url, 'history entry URL').toBe('?vocaloid=Miku');
});

test('changing two observables adds history entry for both', () => {
	initRouter();

	testObject.vocaloid('Miku');
	testObject.song('Nebula');

	expect(history.length, 'Added history entries').toBe(2);
	expect(history[1].url, 'history entry URL').toBe(
		'?vocaloid=Miku&song=Nebula',
	);
});

test('query parameters get URL encoded', () => {
	initRouter();

	testObject.vocaloid('Hatsune Miku');

	expect(history.length, 'Added history entry').toBe(1);
	expect(history[0].url, 'history entry URL').toBe('?vocaloid=Hatsune%20Miku');
});

test('going back changes observables', () => {
	initRouter();

	testObject.vocaloid('Miku');
	testObject.song('SPIKE');
	win.history.go(-1);

	expect(history.length, 'Removed history entry').toBe(1);
	expect(history[0].url, 'history entry URL').toBe('?vocaloid=Miku');
	expect(testObject.song(), 'testObject.song').toBe('magnet');
});

test('going back handles URL-encoding', () => {
	initRouter();

	testObject.song('Becoming Round');
	testObject.song('Nebula');
	win.history.go(-1);

	expect(testObject.song(), 'testObject.song').toBe('Becoming Round');
});
