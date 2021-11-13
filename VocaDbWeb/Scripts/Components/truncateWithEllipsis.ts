const truncateWithEllipsis = (str: string, length: number): string =>
	str.length > length ? `${str.slice(0, length)}...` : str;

export default truncateWithEllipsis;
