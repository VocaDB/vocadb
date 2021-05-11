// eslint-disable-next-line @typescript-eslint/no-unused-vars
interface KnockoutFilters {
  formatDate: (date: Date, format: string) => string;
}

ko.filters.formatDate = (date, format): string => {
  if (!date) return '';

  return moment(date).format(format);
};
