import ResourcesContract from '@DataContracts/ResourcesContract';
import HttpClient from '@Shared/HttpClient';

export default class ResourceRepository {
  constructor(private readonly httpClient: HttpClient) {}

  public getList = (
    cultureCode: string,
    setNames: string[],
  ): Promise<ResourcesContract> => {
    return this.httpClient.get<ResourcesContract>(
      `/api/resources/${cultureCode}/`,
      { setNames: setNames },
    );
  };
}
