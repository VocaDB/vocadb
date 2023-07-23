import EntryToolTip from '@/components/ToolTips/EntryToolTip';
import htmlParse, { Element, DOMNode, domToReact } from 'html-react-parser';
import Link from 'next/link';
import useSWR from 'swr';
import { apiGet } from './FetchApiHelper';
import { useState } from 'react';

// the workaround: https://github.com/remarkablemark/html-react-parser/issues/616
// the bug: https://github.com/remarkablemark/html-react-parser/issues/633
const isElement = (domNode: DOMNode): domNode is Element => {
	const isTag = ['tag', 'script'].includes(domNode.type);
	const hasAttributes = (domNode as Element).attribs !== undefined;

	return isTag && hasAttributes;
};

interface EntryLinkProps {
	itemUrl: string;
	children: JSX.Element | JSX.Element[] | string;
}

const EntryLink = ({ itemUrl, children }: EntryLinkProps) => {
	const [hovered, setHovered] = useState(false);
	const apiUrl = itemUrl.replace('/Ar/', '/artists/').replace('/Al/', '/albums/');
	const { data, isLoading } = useSWR(
		hovered ? `/api${apiUrl}?fields=AdditionalNames` : null,
		apiGet,
		{
			revalidateOnFocus: false,
			revalidateIfStale: false,
		}
	);
	const entry = itemUrl.includes('/Ar/') ? 'artist' : itemUrl.includes('/Al/') ? 'album' : 'tag';

	const child = (
		<Link
			onMouseOver={() => {
				if (!hovered) setHovered(true);
			}}
			href={itemUrl}
		>
			{children}
		</Link>
	);

	return isLoading || !hovered ? (
		child
	) : (
		//@ts-ignore
		<EntryToolTip artist={data} album={data} entry={entry}>
			{child}
		</EntryToolTip>
	);
};

export default function parse(html: string): string | JSX.Element | JSX.Element[] {
	return htmlParse(html, {
		replace: (domNode) => {
			if (isElement(domNode) && domNode.name === 'a' && domNode.attribs.href !== undefined) {
				const href = domNode.attribs.href.replace(/https?:\/\/vocadb.net/, '');
				if (href.startsWith('/Ar/') || href.startsWith('/Al/')) {
					return <EntryLink itemUrl={href}>{domToReact(domNode.childNodes)}</EntryLink>;
				}
				return <Link href={href}>{domToReact(domNode.childNodes)}</Link>;
			}
		},
	});
}

