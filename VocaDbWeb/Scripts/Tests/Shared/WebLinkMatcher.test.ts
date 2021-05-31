import WebLinkCategory from '@Models/WebLinkCategory';
import WebLinkMatcher from '@Shared/WebLinkMatcher';

test('matchWebLink match', () => {
	var result = WebLinkMatcher.matchWebLink(
		'http://www.youtube.com/user/tripshots',
	);

	expect(result, 'result').toBeTruthy();
	expect(result.desc, 'desc').toBe('YouTube Channel');
	expect(result.cat, 'cat').toBe(WebLinkCategory.Official);
});

test('matchWebLink no match', () => {
	var result = WebLinkMatcher.matchWebLink('http://www.google.com');

	expect(result, 'result').toBeUndefined();
});
