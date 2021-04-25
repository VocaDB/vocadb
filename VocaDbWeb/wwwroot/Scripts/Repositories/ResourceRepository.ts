import functions from '../Shared/GlobalFunctions';
import ResourcesContract from '../DataContracts/ResourcesContract';

export default class ResourceRepository {
  constructor(private baseUrl: string) {}

  public getList = (
    cultureCode: string,
    setNames: string[],
    success: (resources: ResourcesContract) => void,
  ) => {
    var url = functions.mergeUrls(
      this.baseUrl,
      '/api/resources/' + cultureCode + '/',
    );
    $.getJSON(url, { setNames: setNames }, success);
  };
}
