import PVService from '@Models/PVs/PVService';
import _ from 'lodash';

export default class PVHelper {
  public static pvServicesArrayFromString = (
    pvServices: string,
  ): PVService[] => {
    if (!pvServices) return [];

    var values = pvServices.split(',');
    var services: PVService[] = _.map(
      values,
      (val) => PVService[val.trim() as keyof typeof PVService],
    );

    return services;
  };
}
