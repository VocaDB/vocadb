import functions from '@Shared/GlobalFunctions';

test('mergeUrls bothWithSlash', () => {
  var result = functions.mergeUrls('/', '/Song');

  expect(result, 'result').toBe('/Song');
});

test('mergeUrls baseWithSlash', () => {
  var result = functions.mergeUrls('/', 'Song');

  expect(result, 'result').toBe('/Song');
});

test('mergeUrls relativeWithSlash', () => {
  var result = functions.mergeUrls('', '/Song');

  expect(result, 'result').toBe('/Song');
});

test('mergeUrls neitherWithSlash', () => {
  var result = functions.mergeUrls('', 'Song');

  expect(result, 'result').toBe('/Song');
});
