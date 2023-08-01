import { unified } from 'unified';
import remarkParse from 'remark-parse';
import remarkRehype from 'remark-rehype';
import rehypeStringify from 'rehype-stringify';
import remarkBreaks from 'remark-breaks';
import remarkMentions from 'remark-mentions';
import parse from '@/Helpers/markdown';
import { TypographyStylesProvider } from '@mantine/core';

interface MarkdownProps {
	children: string;
}

export default function MarkdownRenderer({ children }: MarkdownProps) {
	const processer = unified()
		.use(remarkParse)
		.use(remarkBreaks)
		.use(remarkMentions, { usernameLink: (username) => `/Profile/${username}` })
		.use(remarkRehype)
		.use(rehypeStringify);

	return (
		<TypographyStylesProvider fz="sm" className="description">
			{parse(String(processer.processSync(children)))}
		</TypographyStylesProvider>
	);
}

