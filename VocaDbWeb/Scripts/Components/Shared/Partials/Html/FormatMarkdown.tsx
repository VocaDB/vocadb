import Markdown from '@Components/KnockoutExtensions/Markdown';
import React from 'react';

interface FormatMarkdownProps {
	text: string;
}

const FormatMarkdown = ({ text }: FormatMarkdownProps): React.ReactElement => {
	return <Markdown children={text} />;
};

export default FormatMarkdown;
