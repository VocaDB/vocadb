import EntryLink from '@/components/ToolTips/EntryLink';
import htmlParse, { Element, DOMNode, domToReact } from 'html-react-parser';
import Link from 'next/link';

// the workaround: https://github.com/remarkablemark/html-react-parser/issues/616
// the bug: https://github.com/remarkablemark/html-react-parser/issues/633
const isElement = (domNode: DOMNode): domNode is Element => {
	const isTag = ['tag', 'script'].includes(domNode.type);
	const hasAttributes = (domNode as Element).attribs !== undefined;

	return isTag && hasAttributes;
};

export default function parse(html: string): string | JSX.Element | JSX.Element[] {
	return htmlParse(html, {
		replace: (domNode) => {
			if (isElement(domNode) && domNode.name === 'a' && domNode.attribs.href !== undefined) {
				const href = domNode.attribs.href.replace(/https?:\/\/vocadb.net/, '');
				if (href.startsWith('/Ar/') || href.startsWith('/Al/') || href.startsWith('/S/')) {
					return <EntryLink itemUrl={href}>{domToReact(domNode.childNodes)}</EntryLink>;
				}
				return <Link href={href}>{domToReact(domNode.childNodes)}</Link>;
			}
		},
	});
}

// https://github.com/danestves/markdown-to-text/blob/main/index.ts
export const unmark = (markdown: string | undefined): string => {
	if (markdown === undefined) return '';

	return markdown
		.replace(/^(-\s*?|\*\s*?|_\s*?){3,}\s*$/gm, '')
		.replace(/\n={2,}/g, '\n')
		.replace(/~{3}.*\n/g, '')
		.replace(/~~/g, '')
		.replace(/`{3}.*\n/g, '')
		.replace(/<[^>]*>/g, '')
		.replace(/^[=\-]{2,}\s*$/g, '')
		.replace(/\[\^.+?\](\: .*?$)?/g, '')
		.replace(/\s{0,2}\[.*?\]: .*?$/g, '')
		.replace(/\!\[(.*?)\][\[\(].*?[\]\)]/g, '$1')
		.replace(/\[(.*?)\][\[\(].*?[\]\)]/g, '$1')
		.replace(/^\s{0,3}>\s?/g, '')
		.replace(/(^|\n)\s{0,3}>\s?/g, '\n\n')
		.replace(/^\s{1,2}\[(.*?)\]: (\S+)( ".*?")?\s*$/g, '')
		.replace(/^(\n)?\s{0,}#{1,6}\s+| {0,}(\n)?\s{0,}#{0,} {0,}(\n)?\s{0,}$/gm, '$1$2$3')
		.replace(/([\*_]{1,3})(\S.*?\S{0,1})\1/g, '$2')
		.replace(/([\*_]{1,3})(\S.*?\S{0,1})\1/g, '$2')
		.replace(/(`{3,})(.*?)\1/gm, '$2')
		.replace(/`(.+?)`/g, '$1')
		.replace(/\n{2,}/g, '\n\n');
};

