import {
	Group,
	MantineColor,
	Navbar,
	ThemeIcon,
	UnstyledButton,
	Text,
} from '@mantine/core';
import { IconMusic } from '@tabler/icons-react';

const linkData = [
	{ icon: <IconMusic size="1rem" />, color: 'teal', label: 'Songs' },
];

interface CustomNavbarProps {
	opened: boolean;
}

interface MainLinkProps {
	icon: React.ReactNode;
	color: MantineColor;
	label: string;
}

const MainLink = ({
	icon,
	color,
	label,
}: MainLinkProps): React.ReactElement => {
	return (
		<UnstyledButton
			// eslint-disable-next-line @typescript-eslint/explicit-function-return-type
			sx={(theme) => ({
				display: 'block',
				width: '100%',
				padding: theme.spacing.xs,
				borderRadius: theme.radius.sm,
				color:
					theme.colorScheme === 'dark' ? theme.colors.dark[0] : theme.black,
				'&:hover': {
					backgroundColor:
						theme.colorScheme === 'dark'
							? theme.colors.dark[6]
							: theme.colors.gray[0],
				},
			})}
		>
			<Group>
				<ThemeIcon color={color} variant="light">
					{icon}
				</ThemeIcon>
				<Text size="sm">{label}</Text>
			</Group>
		</UnstyledButton>
	);
};

const CustomNavbar = ({ opened }: CustomNavbarProps): React.ReactElement => {
	return (
		<Navbar
			p="md"
			hiddenBreakpoint="sm"
			hidden={!opened}
			width={{ sm: 200, lg: 300 }}
		>
			{linkData.map((link) => (
				<MainLink {...link} key={link.label} />
			))}
		</Navbar>
	);
};

export default CustomNavbar;
