import { AffiliateLinkGenerator } from '@Helpers/UrlHelper';
import GlobalValues from '@Shared/GlobalValues';

const generator = new AffiliateLinkGenerator({
	amazonJpAffiliateId: 'vocadb',
	playAsiaAffiliateId: '852809',
} as GlobalValues);

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

test('Amazon', () => {
	const input = 'http://www.amazon.co.jp/dp/B00K1IV8FM';
	const expected = 'http://www.amazon.co.jp/dp/B00K1IV8FM?tag=vocadb';

	const result = generator.generateAffiliateLink(input);

	expect(result, 'Amazon affiliate link matches').toBe(expected);
});

test('Amazon_ReplaceAffId', () => {
	const input = 'http://www.amazon.co.jp/dp/B00K1IV8FM?tag=another';
	const expected = 'http://www.amazon.co.jp/dp/B00K1IV8FM?tag=vocadb';

	const result = generator.generateAffiliateLink(input);

	expect(result, 'Amazon affiliate link matches').toBe(expected);
});
