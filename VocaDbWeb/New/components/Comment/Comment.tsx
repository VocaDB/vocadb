// https://ui.mantine.dev/component/comment-html
import { CommentContract } from '@/types/DataContracts/CommentContract';
import {
	createStyles,
	Text,
	Avatar,
	Group,
	TypographyStylesProvider,
	Paper,
	rem,
	Anchor,
} from '@mantine/core';
import Link from 'next/link';
import dayjs from 'dayjs';
import relativeTime from 'dayjs/plugin/relativeTime';

dayjs.extend(relativeTime);

interface CommentProps {
	comment: CommentContract;
}

const useStyles = createStyles((theme) => ({
	comment: {
		padding: `${theme.spacing.lg} ${theme.spacing.xl}`,
	},

	body: {
		paddingLeft: rem(54),
		paddingTop: theme.spacing.sm,
		fontSize: theme.fontSizes.sm,
	},

	content: {
		'& > p:last-child': {
			marginBottom: 0,
		},
	},
}));

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
				{/* TODO: Markdown rendering */}
				<div className={classes.content}>{comment.message}</div>
			</TypographyStylesProvider>
		</Paper>
	);
}

