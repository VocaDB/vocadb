// https://github.com/mantinedev/ui.mantine.dev/blob/master/components/UserButton/UserButton.tsx
import {
	UnstyledButton,
	UnstyledButtonProps,
	Group,
	Avatar,
	Text,
	createStyles,
} from '@mantine/core';
import { IconChevronRight } from '@tabler/icons-react';

const useStyles = createStyles((theme) => ({
	user: {
		display: 'block',
		width: '100%',
		padding: theme.spacing.md,
		color: theme.colorScheme === 'dark' ? theme.colors.dark[0] : theme.black,

		'&:hover': {
			backgroundColor:
				theme.colorScheme === 'dark' ? theme.colors.dark[6] : theme.colors.gray[0],
		},
	},
}));

interface UserButtonProps extends UnstyledButtonProps {
	image: string;
	name: string;
	email: string;
	icon?: React.ReactNode;
}

export function UserButton({ image, name, email, icon, ...others }: UserButtonProps) {
	const { classes } = useStyles();

	return (
		<UnstyledButton className={classes.user} {...others}>
			<Group>
				<Avatar src={image} radius="xl" />

				<div style={{ flex: 1 }}>
					<Text size="sm" weight={500}>
						{name}
					</Text>

					<Text color="dimmed" size="xs">
						{email}
					</Text>
				</div>

				{icon || <IconChevronRight size="0.9rem" stroke={1.5} />}
			</Group>
		</UnstyledButton>
	);
}

