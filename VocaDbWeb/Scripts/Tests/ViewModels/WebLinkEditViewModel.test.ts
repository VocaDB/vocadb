import WebLinkCategory from '@/Models/WebLinkCategory';
import WebLinkEditViewModel from '@/ViewModels/WebLinkEditViewModel';

var webLinkData = {
	category: WebLinkCategory.Official,
	description: 'Youtube Channel',
	id: 0,
	url: 'http://www.youtube.com/user/tripshots',
	disabled: false,
};

test('constructor', () => {
	var target = new WebLinkEditViewModel(webLinkData);

	expect(target.category(), 'category').toBe('Official');
	expect(target.description(), 'description').toBe('Youtube Channel');
	expect(target.url(), 'url').toBe('http://www.youtube.com/user/tripshots');
});

test('editing url sets description', () => {
	var target = new WebLinkEditViewModel(null!);

	target.url('http://www.nicovideo.jp/mylist/');

	expect(target.category(), 'category').toBe('Official');
	expect(target.description(), 'description').toBe('NND MyList');
});
