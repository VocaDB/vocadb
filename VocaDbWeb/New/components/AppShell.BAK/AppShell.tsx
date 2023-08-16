import { AppShell, Box, MediaQuery, Navbar as MantineNavbar, ScrollArea } from '@mantine/core';
import { useEffect, useState } from 'react';
import Header from './Header';
import { useRouter } from 'next/router';
import dynamic from 'next/dynamic';
import { useStyles } from './AppShell.styles';

const Navbar = dynamic(() => import('./Navbar'), {
	ssr: false,
	loading: () => (
		<MantineNavbar p="md" hiddenBreakpoint="sm" hidden={false} width={{ sm: 220, lg: 300 }}>
			<div style={{ height: '100%' }} />
		</MantineNavbar>
	),
});
const Footer = dynamic(() => import('./Footer'), { ssr: false });
const LyricsContainer = dynamic(() => import('@/nostalgic-darling/LyricsContainer'), {
	ssr: false,
});
const PVPlayer = dynamic(() => import('@/nostalgic-darling/PVPlayer'), {
	loading: () => null,
	ssr: false,
});

interface CustomAppShellProps {
	children?: React.ReactElement;
}

const CustomAppShell = ({ children }: CustomAppShellProps): React.ReactElement => {
	const router = useRouter();
	const [opened, setOpened] = useState(false);
	const { classes } = useStyles();

	// Close burger menu on navigation
	useEffect(() => {
		router.events.on('routeChangeStart', () => {
			setOpened(false);
		});
	}, [router]);

	return (
		<AppShell
			navbarOffsetBreakpoint="sm"
			navbar={<Navbar opened={opened} />}
			header={<Header opened={opened} setOpened={setOpened} />}
			// eslint-disable-next-line @typescript-eslint/explicit-function-return-type
			styles={(theme) => ({
				main: {
					backgroundColor:
						theme.colorScheme === 'dark' ? theme.colors.dark[8] : theme.colors.gray[0],
					width: 0, // TODO: Remoe this hack (prevents chrome viewport being too large)
				},
			})}
			padding={0}
		>
			<Box id="main-content" className={classes.box}>
				{children}
				<LyricsContainer />
				<PVPlayer />
			</Box>
			<Footer />
		</AppShell>
	);
};

export default CustomAppShell;

