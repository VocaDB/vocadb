import _ from 'lodash';

declare global {
  interface KnockoutPunchesFilters {
    truncate: (source: string, length: number) => string;
  }
}

ko.filters.truncate = (source, length): string => {
  return _.truncate(source, { length: length });
};
