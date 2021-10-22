import UrlHelper from '@Helpers/UrlHelper';

test('MakeLink_Empty', () => {
	const result = UrlHelper.makeLink('');

	expect(result, 'result').toBe('');
});

test('MakeLink_WithHttp', () => {
	const result = UrlHelper.makeLink('http://vocadb.net');

	expect(result, 'result').toBe('http://vocadb.net');
});

test('MakeLink_WithoutHttp', () => {
	const result = UrlHelper.makeLink('vocadb.net');

	expect(result, 'result').toBe('http://vocadb.net');
});

test('MakeLink_Mailto', () => {
	const result = UrlHelper.makeLink('mailto:miku@vocadb.net');

	expect(result, 'result').toBe('mailto:miku@vocadb.net');
});

test('UpgradeToHttps', () => {
	expect(
		UrlHelper.upgradeToHttps('http://tn.smilevideo.jp/smile?i=6888548'),
		'http://tn.smilevideo.jp was upgraded',
	).toBe('https://tn.smilevideo.jp/smile?i=6888548');
	expect(
		UrlHelper.upgradeToHttps('http://tn-skr1.smilevideo.jp/smile?i=6888548'),
		'http://tn-skr1.smilevideo.jp was upgraded',
	).toBe('https://tn.smilevideo.jp/smile?i=6888548');
	expect(
		UrlHelper.upgradeToHttps('https://tn.smilevideo.jp/smile?i=6888548'),
		'https://tn.smilevideo.jp already HTTPS',
	).toBe('https://tn.smilevideo.jp/smile?i=6888548');
	expect(
		UrlHelper.upgradeToHttps('http://tn.smilevideo.jp/smile?i=34172016.39165'),
		'URL with dot was upgraded',
	).toBe('https://tn.smilevideo.jp/smile?i=34172016.39165');
});
