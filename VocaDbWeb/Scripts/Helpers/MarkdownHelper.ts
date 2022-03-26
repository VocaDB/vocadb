export class MarkdownHelper {
	public static createMarkdownLink = (url: string, name: string): string => {
		return `[${name}](${url})`;
	};
}
