import EntryType from '@Models/EntryType';

import EntryThumbContract from '../EntryThumbContract';
import EnglishTranslatedStringContract from '../Globalization/EnglishTranslatedStringContract';
import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import WebLinkContract from '../WebLinkContract';
import TagBaseContract from './TagBaseContract';

export default interface TagApiContract {
  additionalNames?: string;

  categoryName: string;

  defaultNameLanguage: string;

  description: string;

  id: number;

  mainPicture: EntryThumbContract;

  name: string;

  names: LocalizedStringWithIdContract[];

  parent: TagBaseContract;

  relatedTags?: TagBaseContract[];

  status: string;

  targets: EntryType;

  translatedDescription?: EnglishTranslatedStringContract;

  urlSlug?: string;

  usageCount: number;

  webLinks: WebLinkContract[];
}
