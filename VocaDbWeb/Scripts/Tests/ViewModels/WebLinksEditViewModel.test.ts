import TranslatedEnumField from '@DataContracts/TranslatedEnumField';
import WebLinksEditViewModel from '@ViewModels/WebLinksEditViewModel';

var categories: TranslatedEnumField[] = [
  { id: 'Official', name: 'Official' },
  { id: 'Commercial', name: 'Commercial' },
];
var webLinkData = {
  category: 'Official',
  description: 'Youtube Channel',
  id: 0,
  url: 'http://www.youtube.com/user/tripshots',
  disabled: false,
};

test('constructor', () => {
  var target = new WebLinksEditViewModel([webLinkData], categories);

  expect(target.items().length, 'webLinks.length').toBe(1);
  expect(target.categories!.length, 'categories.length').toBe(2);
});

test('add new', () => {
  var target = new WebLinksEditViewModel([]);

  target.add();

  expect(target.items().length, 'webLinks.length').toBe(1);
});

test('remove', () => {
  var target = new WebLinksEditViewModel([webLinkData]);

  target.remove(target.items()[0]);

  expect(target.items().length, 'webLinks.length').toBe(0);
});
