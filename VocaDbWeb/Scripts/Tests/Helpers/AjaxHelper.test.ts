import AjaxHelper from '@Helpers/AjaxHelper';

var testCreateUrl = (
  params: { [key: string]: any[] },
  expected: string,
): void => {
  var actual = AjaxHelper.createUrl(params);
  expect(actual).toBe(expected);
};

test('single param', () => {
  testCreateUrl({ vocaloid: ['miku', 'luka'] }, 'vocaloid=miku&vocaloid=luka');
});

test('multiple params', () => {
  testCreateUrl(
    { vocaloid: ['miku'], song: ['Nebula'] },
    'vocaloid=miku&song=Nebula',
  );
});

test('single param with null', () => {
  testCreateUrl(
    { vocaloid: ['miku', null, 'luka'] },
    'vocaloid=miku&vocaloid=&vocaloid=luka',
  );
});

test('multiple params with null', () => {
  testCreateUrl(
    { vocaloid: ['miku', null, 'luka'], song: [null, 'Nebula', null] },
    'vocaloid=miku&vocaloid=&vocaloid=luka&song=&song=Nebula&song=',
  );
});
