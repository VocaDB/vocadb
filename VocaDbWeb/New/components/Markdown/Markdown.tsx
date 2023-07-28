import dynamic from 'next/dynamic';

interface MarkdownProps {
	children: string;
}

const MarkdownRenderer = dynamic(() => import('./MarkownRenderer'));

export default function Markdown({ children }: MarkdownProps) {
	return <MarkdownRenderer>{children}</MarkdownRenderer>;
}

