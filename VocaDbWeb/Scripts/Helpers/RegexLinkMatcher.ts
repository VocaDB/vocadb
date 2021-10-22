import _ from 'lodash';

// Code from: https://stackoverflow.com/questions/610406/javascript-equivalent-to-printf-string-format/4673436#4673436
const format = (format: string, args: any): string => {
	return format.replace(/{(\d+)}/g, function (match, number) {
		return typeof args[number] != 'undefined' ? args[number] : match;
	});
};

// Corresponds to the RegexLinkMatcher class in C#.
/// <summary>
/// Captures values from (partial) URL using regex and then formats a full
/// URL using those captured values.
/// </summary>
export default class RegexLinkMatcher {
	private readonly regex: RegExp;

	public constructor(private readonly template: string, regexStr: string) {
		this.regex = new RegExp(regexStr);
	}

	public getLinkFromUrl = (
		url: string,
	): { success: boolean; formattedUrl?: string } => {
		const match = url.match(this.regex);

		if (match) {
			const values = _.slice(match, 1);
			return {
				formattedUrl: format(this.template, values),
				success: true,
			};
		} else {
			return {
				formattedUrl: undefined,
				success: false,
			};
		}
	};
}
