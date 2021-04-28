import ContentLanguagePreference from '../Models/Globalization/ContentLanguagePreference';
import NameMatchMode from '../Models/NameMatchMode';

export default class BaseRepository {
  protected handleJqueryPromise<T>(jqueryPromise: JQueryXHR) {
    const promise = Promise.resolve(jqueryPromise);
    return promise as Promise<T>;
  }

  protected getDate(date?: Date) {
    return date ? date.toISOString() : undefined;
  }

  // todo: protected
  public languagePreferenceStr: string;

  constructor(
    public baseUrl: string,
    languagePreference = ContentLanguagePreference.Default,
  ) {
    this.languagePreferenceStr = ContentLanguagePreference[languagePreference];
  }
}

// Common parameters for entry queries (listings).
export interface CommonQueryParams {
  getTotalCount?: boolean;

  // Content language preference
  lang?: ContentLanguagePreference;

  maxResults?: number;

  nameMatchMode?: NameMatchMode;

  start?: number;

  query?: string;
}
