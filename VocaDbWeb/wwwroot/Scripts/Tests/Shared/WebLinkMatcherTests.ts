import WebLinkCategory from '@Models/WebLinkCategory';
import WebLinkMatcher from '@Shared/WebLinkMatcher';

QUnit.module('WebLinkMatcher');

QUnit.test('matchWebLink match', () => {
  var result = WebLinkMatcher.matchWebLink(
    'http://www.youtube.com/user/tripshots',
  );

  ok(result, 'result');
  equal(result.desc, 'YouTube Channel', 'desc');
  equal(result.cat, WebLinkCategory.Official, 'cat');
});

QUnit.test('matchWebLink no match', () => {
  var result = WebLinkMatcher.matchWebLink('http://www.google.com');

  equal(result, null, 'result');
});
