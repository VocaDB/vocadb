// https://ui.mantine.dev/component/comment-html
import { CommentContract } from '@/types/DataContracts/CommentContract';
import { Text, Avatar, Group, TypographyStylesProvider, Paper, Anchor } from '@mantine/core';
import Link from 'next/link';
import dayjs from 'dayjs';
import relativeTime from 'dayjs/plugin/relativeTime';
import { useStyles } from './Comment.styles';
import MarkdownRenderer from '../Markdown/MarkownRenderer';

dayjs.extend(relativeTime);

interface CommentProps {
	comment: CommentContract;
}

// Imports dayjs/plugin/relativeTime and should be lazy loaded
export default function Comment({ comment }: CommentProps) {
	const { classes } = useStyles();
	return (
		<Paper maw={700} withBorder radius="md" className={classes.comment}>
			<Group>
				<Avatar
					src={comment.author.mainPicture?.urlTinyThumb ?? '/unknown.webp'}
					alt={comment.authorName}
					radius="xl"
				/>
				<div>
					<Anchor fz="sm" component={Link} href={'/Profile/' + comment.author.name}>
						{comment.authorName}
					</Anchor>
					<Text fz="xs" c="dimmed">
						{dayjs(comment.created).fromNow()}
					</Text>
				</div>
			</Group>
			<TypographyStylesProvider className={classes.body}>
				<div className={classes.content}>
					<MarkdownRenderer>{comment.message}</MarkdownRenderer>
				</div>
			</TypographyStylesProvider>
		</Paper>
	);
}

