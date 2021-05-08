import ObservableUrlParamRouter from '../../../Shared/Routing/ObservableUrlParamRouter';

interface HistoryEntry {
  data: any;
  url: string;
}

var win: Window;
var history: HistoryEntry[];
var testObject: any;

QUnit.module('ObservableUrlParamRouter', {
  setup: () => {
    testObject = {
      vocaloid: ko.observable('Luka'),
      song: ko.observable('magnet'),
    };

    history = new Array<HistoryEntry>();

    win = <any>{
      history: {
        pushState: (statedata: any, title: string, url?: string) =>
          history.push({ data: statedata, url: url! }),
        go: (steps: number) => {
          for (var i = 0; i < -steps; i++) {
            history.pop();
            var latest = history.length ? history[history.length - 1] : null;
            if (win.onpopstate) win.onpopstate(<any>{ state: latest!.data });
          }
        },
      },
    };

    jQuery.url = () => null!;
  },
});

var initRouter = (): void => {
  new ObservableUrlParamRouter(testObject, win);
};

QUnit.test('changing observable adds history entry', () => {
  initRouter();

  testObject.vocaloid('Miku');

  QUnit.equal(history.length, 1, 'Added history entry');
  QUnit.equal(history[0].url, '?vocaloid=Miku', 'history entry URL');
});

QUnit.test('changing two observables adds history entry for both', () => {
  initRouter();

  testObject.vocaloid('Miku');
  testObject.song('Nebula');

  QUnit.equal(history.length, 2, 'Added history entries');
  QUnit.equal(
    history[1].url,
    '?vocaloid=Miku&song=Nebula',
    'history entry URL',
  );
});

QUnit.test('query parameters get URL encoded', () => {
  initRouter();

  testObject.vocaloid('Hatsune Miku');

  QUnit.equal(history.length, 1, 'Added history entry');
  QUnit.equal(history[0].url, '?vocaloid=Hatsune%20Miku', 'history entry URL');
});

QUnit.test('going back changes observables', () => {
  initRouter();

  testObject.vocaloid('Miku');
  testObject.song('SPIKE');
  win.history.go(-1);

  QUnit.equal(history.length, 1, 'Removed history entry');
  QUnit.equal(history[0].url, '?vocaloid=Miku', 'history entry URL');
  QUnit.equal(testObject.song(), 'magnet', 'testObject.song');
});

QUnit.test('going back handles URL-encoding', () => {
  initRouter();

  testObject.song('Becoming Round');
  testObject.song('Nebula');
  win.history.go(-1);

  QUnit.equal(testObject.song(), 'Becoming Round', 'testObject.song');
});
