import PVContract from '@DataContracts/PVs/PVContract';
import HttpClient from '@Shared/HttpClient';

export default class PVRepository {
  constructor(private readonly httpClient: HttpClient) {}

  public getPVByUrl = (pvUrl: string, type: string): Promise<PVContract> => {
    return this.httpClient.get<PVContract>('/api/pvs', {
      pvUrl: pvUrl,
      type: type,
    });
  };
}
