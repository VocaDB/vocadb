// https://github.com/mantinedev/ui.mantine.dev/blob/master/components/UserButton/UserButton.tsx
import { UnstyledButton, UnstyledButtonProps, Group, Avatar, Text, Menu } from '@mantine/core';
import {
	IconAdjustments,
	IconChevronRight,
	IconDisc,
	IconLogout,
	IconMusic,
	IconUser,
} from '@tabler/icons-react';
import { useStyles } from './UserButton.styles';

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

