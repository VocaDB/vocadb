import { AffiliateLinkGenerator } from '@/Helpers/UrlHelper';

const generator = new AffiliateLinkGenerator({
	playAsiaAffiliateId: '852809',
});

test('PlayAsia', () => {
	const input =
		'http://www.play-asia.com/0005-a-galaxy-odyssey-paOS-13-49-en-70-7sjp.html';
	const expected =
		'http://www.play-asia.com/0005-a-galaxy-odyssey-paOS-13-49-en-70-7sjp.html?affiliate_id=852809';

	const result = generator.generateAffiliateLink(input);

	expect(result, 'Play-asia affiliate link matches').toBe(expected);
});

test('PlayAsia_ReplaceAffId', () => {
	const input =
		'http://www.play-asia.com/0005-a-galaxy-odyssey-paOS-13-49-en-70-7sjp.html?affiliate_id=12345';
	const expected =
		'http://www.play-asia.com/0005-a-galaxy-odyssey-paOS-13-49-en-70-7sjp.html?affiliate_id=852809';

	const result = generator.generateAffiliateLink(input);

	expect(result, 'Play-asia affiliate link matches').toBe(expected);
});
