// From https://ui.mantine.dev/component/navbar-links-group
import { PermissionToken } from '@/types/Models/LoginManager';
import { Group, Box, ThemeIcon, Text, UnstyledButton } from '@mantine/core';
import Link from 'next/link';
import { hasPermission } from '@/Helpers/PermissionsHelper';
import { useVdbStore } from '@/stores/useVdbStore';
import { useStyles } from './CollapsibleLinkGroup.styles';

export interface LinksGroupProps {
	icon: React.FC<any>;
	label: string;
	link: string;
	permission?: PermissionToken;
	links?: { label: string; link: string; permission?: PermissionToken }[];
}

export function NavbarLinksGroup({ icon: Icon, label, links, link, permission }: LinksGroupProps) {
	const { classes, theme } = useStyles();
	const [values] = useVdbStore((set) => [set.values]);

	if (permission && !hasPermission(values, permission)) {
		return <></>;
	}

	const hasLinks = Array.isArray(links);
	const items = (hasLinks ? links : [])
		.filter((link) => {
			if (link.permission) return hasPermission(values, link.permission);
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
					<Box className={classes.iconWrapper}>
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

