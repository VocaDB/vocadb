import React from 'react';
import ReactMarkdown from 'react-markdown';
import remarkBreaks from 'remark-breaks';
import gfm from 'remark-gfm';

interface MarkdownProps {
	children: string;
}

const Markdown = ({ children }: MarkdownProps): React.ReactElement => {
	return (
		<ReactMarkdown remarkPlugins={[gfm, remarkBreaks]}>
			{children}
		</ReactMarkdown>
	);
};

export default Markdown;
