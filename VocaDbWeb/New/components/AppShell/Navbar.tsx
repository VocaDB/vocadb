import { Group, Navbar, ThemeIcon, UnstyledButton, Text, useMantineTheme } from '@mantine/core';
import { IconCalendarStats, IconMusic } from '@tabler/icons-react';
import { LinksGroupProps, NavbarLinksGroup } from './CollapsibleLinkGroup';

const linkData = [
	{ icon: IconMusic, color: 'teal', label: 'Songs' },

	{
		label: 'Releases',
		icon: IconCalendarStats,
		links: [
			{ label: 'Upcoming releases', link: '/' },
			{ label: 'Previous releases', link: '/' },
			{ label: 'Releases schedule', link: '/' },
		],
	},
];

interface CustomNavbarProps {
	opened: boolean;
}

const MainLink = ({
	icon: Icon,
	label,
	links,
	initiallyOpened,
}: LinksGroupProps): React.ReactElement => {
	const theme = useMantineTheme();

	if (links) {
		return (
			<NavbarLinksGroup
				icon={Icon}
				label={label}
				links={links}
				initiallyOpened={initiallyOpened}
			/>
		);
	}

	return (
		<UnstyledButton
			// eslint-disable-next-line @typescript-eslint/explicit-function-return-type
			sx={(theme) => ({
				display: 'block',
				width: '100%',
				padding: theme.spacing.xs,
				borderRadius: theme.radius.sm,
				color: theme.colorScheme === 'dark' ? theme.colors.dark[0] : theme.black,
				'&:hover': {
					backgroundColor:
						theme.colorScheme === 'dark' ? theme.colors.dark[6] : theme.colors.gray[0],
				},
			})}
		>
			<Group>
				<ThemeIcon color={theme.primaryColor} variant="light" size={30}>
					<Icon size="1.1rem" />
				</ThemeIcon>
				<Text size="sm">{label}</Text>
			</Group>
		</UnstyledButton>
	);
};

const CustomNavbar = ({ opened }: CustomNavbarProps): React.ReactElement => {
	return (
		<Navbar p="md" hiddenBreakpoint="sm" hidden={!opened} width={{ sm: 200, lg: 300 }}>
			{linkData.map((link) => (
				<MainLink {...link} key={link.label} />
			))}
		</Navbar>
	);
};

export default CustomNavbar;

