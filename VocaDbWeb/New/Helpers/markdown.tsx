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
				return <Link href={href}>{domToReact(domNode.childNodes)}</Link>;
			}
		},
	});
}

