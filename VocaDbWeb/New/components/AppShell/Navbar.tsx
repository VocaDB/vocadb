import { Group, Navbar, ThemeIcon, UnstyledButton, Text, ScrollArea } from '@mantine/core';
import {
	IconBookmarks,
	IconCalendarEvent,
	IconDisc,
	IconHome,
	IconPlaylist,
	IconMessageCircle,
	IconMusic,
	IconUserCog,
	IconUserPlus,
	IconUsers,
	IconUsersGroup,
} from '@tabler/icons-react';
import { LinksGroupProps, NavbarLinksGroup } from './CollapsibleLinkGroup';
import Link from 'next/link';
import { PermissionToken } from '@/types/Models/LoginManager';
import { hasPermission } from '@/Helpers/PermissionsHelper';
import React from 'react';
import dynamic from 'next/dynamic';
import { useVdbStore } from '@/stores/useVdbStore';
import { useStyles } from './NavBar.styles';

const UserButton = dynamic(() => import('./UserButton').then((imp) => imp.UserButton));

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
				permission: PermissionToken.ManageDatabase,
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
				permission: PermissionToken.ManageDatabase,
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
			{
				label: 'Submit a song',
				link: '/Song/Create',
				permission: PermissionToken.ManageDatabase,
			},
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
		permission: PermissionToken.AccessManageMenu,
	},

	{
		label: 'Login / Register',
		icon: IconUserPlus,
		link: '/User/Login',
	},
];

interface CustomNavbarProps {
	opened: boolean;
}

const MainLink = ({
	icon: Icon,
	label,
	links,
	link,
	permission,
}: LinksGroupProps): React.ReactElement => {
	const { classes, theme } = useStyles();
	const [values] = useVdbStore((set) => [set.values]);

	if (links) {
		return <NavbarLinksGroup icon={Icon} label={label} links={links} link={link} />;
	}

	if (permission && !hasPermission(values, permission)) {
		return <></>;
	}

	return (
		<UnstyledButton
			// eslint-disable-next-line @typescript-eslint/explicit-function-return-type
			className={classes.mainLinkButton}
			component={Link}
			href={link}
		>
			<Group>
				<ThemeIcon color={theme.primaryColor} size={30}>
					<Icon size="1.1rem" />
				</ThemeIcon>
				<Text size="sm">{label}</Text>
			</Group>
		</UnstyledButton>
	);
};

const CustomNavbar = ({ opened }: CustomNavbarProps): React.ReactElement => {
	const { classes } = useStyles();
	const [values] = useVdbStore((set) => [set.values]);

	// Remove the login link, if the user is logged in
	const links = linkData.filter(
		({ link }) => !link.startsWith('/User/Login') || !values?.isLoggedIn
	);

	return (
		<Navbar p="md" hiddenBreakpoint="sm" hidden={!opened} width={{ sm: 220, lg: 300 }}>
			<Navbar.Section scrollHideDelay={100} component={ScrollArea} grow>
				{links.map((link) => (
					<MainLink {...link} key={link.label} />
				))}
			</Navbar.Section>
			{values?.isLoggedIn && (
				<Navbar.Section className={classes.navbarSection}>
					<UserButton
						name={values.loggedUser!.name}
						image={`${values.staticContentHost}/img/user/mainThumb/${
							values.loggedUser!.id
						}.jpg`}
						email={"Don't know what to put here yet"}
					/>
				</Navbar.Section>
			)}
		</Navbar>
	);
};

export default CustomNavbar;

