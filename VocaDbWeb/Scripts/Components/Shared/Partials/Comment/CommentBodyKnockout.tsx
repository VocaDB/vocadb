import Markdown from '@/Components/KnockoutExtensions/Markdown';
import React from 'react';

interface CommentBodyKnockoutProps {
	message: string;
}

const CommentBodyKnockout = React.memo(
	({ message }: CommentBodyKnockoutProps): React.ReactElement => {
		return (
			<p>
				<Markdown>{message}</Markdown>
			</p>
		);
	},
);

export default CommentBodyKnockout;
