// https://github.com/mantinedev/ui.mantine.dev/blob/master/components/UserButton/UserButton.tsx
import {
	UnstyledButton,
	UnstyledButtonProps,
	Group,
	Avatar,
	Text,
	createStyles,
	Menu,
} from '@mantine/core';
import {
	IconAdjustments,
	IconChevronRight,
	IconDisc,
	IconLogout,
	IconMusic,
	IconUser,
} from '@tabler/icons-react';

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
}

export function UserButton({ image, name, email, ...others }: UserButtonProps) {
	const { classes } = useStyles();

	return (
		<Menu position="right" shadow="md" offset={25} width={200}>
			<Menu.Target>
				<UnstyledButton className={classes.user} {...others}>
					<Group>
						{/* TODO: Find out why this looks bad * }
						{/* <Image
							alt={name}
							src={image}
							style={{ borderRadius: '50%', objectFit: 'cover' }}
							width={38}
							height={38}
						/> */}
						<Avatar alt={name} src={image} radius="xl" />

						<div style={{ flex: 1 }}>
							<Text size="sm" weight={500}>
								{name}
							</Text>

							<Text color="dimmed" size="xs">
								{email}
							</Text>
						</div>

						<IconChevronRight size="0.9rem" stroke={1.5} />
					</Group>
				</UnstyledButton>
			</Menu.Target>
			<Menu.Dropdown>
				<Menu.Label>My Items</Menu.Label>
				<Menu.Item icon={<IconUser size={14} />}>Profile</Menu.Item>
				<Menu.Item icon={<IconDisc size={14} />}>My albums</Menu.Item>
				<Menu.Item icon={<IconMusic size={14} />}>My songs</Menu.Item>

				<Menu.Divider />

				<Menu.Label>My Settings</Menu.Label>
				<Menu.Item icon={<IconAdjustments size={14} />}>Settings</Menu.Item>
				<Menu.Item icon={<IconLogout size={14} />}>Log out</Menu.Item>
			</Menu.Dropdown>
		</Menu>
	);
}

