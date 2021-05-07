import functions from './GlobalFunctions';

export default class UrlMapper {
  public static buildUrl = (...args: string[]): string => {
    return _.reduce(args, (list: string, item: string) =>
      UrlMapper.mergeUrls(list, item),
    );
  };

  public static mergeUrls = (base: string, relative: string): string => {
    if (base.charAt(base.length - 1) == '/' && relative.charAt(0) == '/')
      return base + relative.substr(1);

    if (base.charAt(base.length - 1) == '/' && relative.charAt(0) != '/')
      return base + relative;

    if (base.charAt(base.length - 1) != '/' && relative.charAt(0) == '/')
      return base + relative;

    return base + '/' + relative;
  };

  constructor(public baseUrl: string) {}

  public mapRelative(relative: string): string {
    return functions.mergeUrls(this.baseUrl, relative);
  }
}
