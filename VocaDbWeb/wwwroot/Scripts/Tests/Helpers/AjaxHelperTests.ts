import AjaxHelper from '../../Helpers/AjaxHelper';

QUnit.module('AjaxHelper');

var testCreateUrl = (
  params: { [key: string]: any[] },
  expected: string,
): void => {
  var actual = AjaxHelper.createUrl(params);
  QUnit.equal(actual, expected);
};

QUnit.test('single param', () => {
  testCreateUrl({ vocaloid: ['miku', 'luka'] }, 'vocaloid=miku&vocaloid=luka');
});

QUnit.test('multiple params', () => {
  testCreateUrl(
    { vocaloid: ['miku'], song: ['Nebula'] },
    'vocaloid=miku&song=Nebula',
  );
});

QUnit.test('single param with null', () => {
  testCreateUrl(
    { vocaloid: ['miku', null, 'luka'] },
    'vocaloid=miku&vocaloid=&vocaloid=luka',
  );
});

QUnit.test('multiple params with null', () => {
  testCreateUrl(
    { vocaloid: ['miku', null, 'luka'], song: [null, 'Nebula', null] },
    'vocaloid=miku&vocaloid=&vocaloid=luka&song=&song=Nebula&song=',
  );
});
