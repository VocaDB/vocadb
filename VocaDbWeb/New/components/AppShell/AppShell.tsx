import { AppShell, Box } from '@mantine/core';
import { useEffect, useState } from 'react';
import Navbar from './Navbar';
import Header from './Header';
import { useRouter } from 'next/router';
import Footer from './Footer';
import dynamic from 'next/dynamic';

const LyricsContainer = dynamic(() => import('@/nostalgic-darling/LyricsContainer'));
const PVPlayer = dynamic(() => import('@/nostalgic-darling/PVPlayer'), {
	loading: () => null,
});

interface CustomAppShellProps {
	children?: React.ReactElement;
}

const CustomAppShell = ({ children }: CustomAppShellProps): React.ReactElement => {
	const router = useRouter();
	const [opened, setOpened] = useState(false);

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
			<>
				<Box p="md" pos="relative" h="calc(100% - 65px)" w="100%">
					{children}
					<LyricsContainer />
					<PVPlayer />
				</Box>
				<Footer />
			</>
		</AppShell>
	);
};

export default CustomAppShell;

