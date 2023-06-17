// From https://ui.mantine.dev/component/navbar-links-group
import { PermissionToken } from '@/types/Models/LoginManager';
import { Group, Box, ThemeIcon, Text, UnstyledButton, createStyles, rem } from '@mantine/core';
import Link from 'next/link';
import { useVdb } from '../Context/VdbContext';

const useStyles = createStyles((theme) => ({
	control: {
		display: 'block',
		width: '100%',
		padding: `${theme.spacing.xs} `,
		color: theme.colorScheme === 'dark' ? theme.colors.dark[0] : theme.black,
		fontSize: theme.fontSizes.sm,

		'&:hover': {
			backgroundColor:
				theme.colorScheme === 'dark' ? theme.colors.dark[6] : theme.colors.gray[0],
		},
	},

	link: {
		display: 'block',
		padding: `${theme.spacing.xs}`,
		paddingLeft: rem(31),
		marginLeft: rem(30),
		fontSize: theme.fontSizes.sm,
		color: theme.colorScheme === 'dark' ? theme.colors.dark[0] : theme.colors.gray[7],
		borderLeft: `${rem(1)} solid ${
			theme.colorScheme === 'dark' ? theme.colors.dark[4] : theme.colors.gray[3]
		}`,

		'&:hover': {
			backgroundColor:
				theme.colorScheme === 'dark' ? theme.colors.dark[6] : theme.colors.gray[0],
		},
	},
}));

export interface LinksGroupProps {
	icon: React.FC<any>;
	label: string;
	link: string;
	permission?: PermissionToken;
	links?: { label: string; link: string; permission?: PermissionToken }[];
}

export function NavbarLinksGroup({ icon: Icon, label, links, link, permission }: LinksGroupProps) {
	const { classes, theme } = useStyles();
	const { loginManager } = useVdb();

	if (permission && !loginManager.hasPermission(permission)) {
		return <></>;
	}

	const hasLinks = Array.isArray(links);
	const items = (hasLinks ? links : [])
		.filter((link) => {
			if (link.permission) return loginManager.hasPermission(link.permission);
			return true;
		})
		.map((link) => (
			<Text component={Link} className={classes.link} href={link.link} key={link.label}>
				{link.label}
			</Text>
		));

	return (
		<>
			<UnstyledButton component={Link} href={link} className={classes.control}>
				<Group position="apart" spacing={0}>
					<Box sx={{ display: 'flex', alignItems: 'center' }}>
						<ThemeIcon color={theme.primaryColor} size={30}>
							<Icon size="1.1rem" />
						</ThemeIcon>
						<Box ml="md">{label}</Box>
					</Box>
				</Group>
			</UnstyledButton>
			{hasLinks ? <>{items}</> : null}
		</>
	);
}

