import DateTimeHelper from '../Helpers/DateTimeHelper';

declare global {
  interface KnockoutFilters {
    formatLengthSeconds: (length: number) => string;
  }
}

ko.filters.formatLengthSeconds = (length): string => {
  return DateTimeHelper.formatFromSeconds(length);
};
