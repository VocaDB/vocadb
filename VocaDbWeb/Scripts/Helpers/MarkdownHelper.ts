export class MarkdownHelper {
	static createMarkdownLink = (url: string, name: string): string => {
		return `[${name}](${url})`;
	};
}
