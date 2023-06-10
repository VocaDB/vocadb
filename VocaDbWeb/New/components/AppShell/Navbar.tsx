import {
	Group,
	Navbar,
	ThemeIcon,
	UnstyledButton,
	Text,
	useMantineTheme,
	ScrollArea,
} from '@mantine/core';
import {
	IconBookmarks,
	IconCalendarEvent,
	IconDisc,
	IconHome,
	IconMessageCircle,
	IconMusic,
	IconUserCog,
	IconUsers,
	IconUsersGroup,
} from '@tabler/icons-react';
import { LinksGroupProps, NavbarLinksGroup } from './CollapsibleLinkGroup';
import Link from 'next/link';
import { IconPlaylist } from '@tabler/icons';
import { UserButton } from './UserButton';

const linkData = [
	{ icon: IconHome, label: 'Home', link: '/' },
	{
		icon: IconUsersGroup,
		label: 'Artists',
		link: '/Search?searchType=Artist',
		links: [
			{
				label: 'Add an artist',
				link: '/Artist/Create',
			},
		],
	},
	{
		label: 'Albums',
		icon: IconDisc,
		link: '/Search?searchType=Album',
		links: [
			{
				label: 'Submit an album',
				link: '/Album/Create',
			},
			{
				label: 'Top rated albums',
				link: '/Search?searchType=Album&sort=RatingAverage',
			},
			{
				label: 'New albums',
				link: '/Search?searchType=Album&sort=ReleaseDate',
			},
		],
	},
	{
		label: 'Songs',
		icon: IconMusic,
		link: '/Search?searchType=Song',
		links: [
			{ label: 'Submit a song', link: '/Song/Create' },
			{ label: 'Top rated songs', link: '/Song/Rankings?durationHours=168' },
			{
				label: 'Recent PVs',
				link: '/Search?searchType=Song&sort=AdditionDate&onlyWithPVs=true',
			},
		],
	},
	{
		label: 'Events',
		icon: IconCalendarEvent,
		link: '/Search?searchType=ReleaseEvent',
		links: [
			{
				label: 'Upcoming events',
				link: '/Event',
			},
		],
	},
	{
		label: 'Songlists',
		icon: IconPlaylist,
		link: '/SongList/Featured',
	},
	{
		label: 'Tags / genres',
		icon: IconBookmarks,
		link: '/Tag',
	},
	{
		label: 'Users',
		icon: IconUsers,
		link: '/User',
	},
	{
		label: 'Discussions',
		icon: IconMessageCircle,
		link: '/discussion',
	},
	{
		label: 'Manage',
		icon: IconUserCog,
		link: '/Admin',
	},
];

interface CustomNavbarProps {
	opened: boolean;
}

const MainLink = ({ icon: Icon, label, links, link }: LinksGroupProps): React.ReactElement => {
	const theme = useMantineTheme();

	if (links) {
		return <NavbarLinksGroup icon={Icon} label={label} links={links} link={link} />;
	}

	return (
		<UnstyledButton
			// eslint-disable-next-line @typescript-eslint/explicit-function-return-type
			sx={(theme) => ({
				display: 'block',
				width: '100%',
				padding: theme.spacing.xs,
				fontSize: theme.fontSizes.sm,
				borderRadius: theme.radius.sm,
				color: theme.colorScheme === 'dark' ? theme.colors.dark[0] : theme.black,
				'&:hover': {
					backgroundColor:
						theme.colorScheme === 'dark' ? theme.colors.dark[6] : theme.colors.gray[0],
				},
			})}
			component={Link}
			href={link}
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
			<ScrollArea>
				{linkData.map((link) => (
					<MainLink {...link} key={link.label} />
				))}
			</ScrollArea>
			<Navbar.Section>
				<UserButton
					name="Placeholder"
					image="https://static.vocadb.net/img/user/mainThumb/14922.jpg?s=120"
					email="Email here?"
				/>
			</Navbar.Section>
		</Navbar>
	);
};

export default CustomNavbar;

