import '@mantine/core/styles.css';

import { MantineProvider, ColorSchemeScript } from '@mantine/core';
import CustomAppShell from '@/components/AppShell/AppShell';
import { ModalsProvider } from '@mantine/modals';
import { colors } from '@/components/colors';
import { cookies } from 'next/headers';

export const metadata = {
	title: 'VocaDB',
	description: '',
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
	const cookieStore = cookies();
	const primaryColor = cookieStore.get('mantine-primary-color')?.value ?? 'miku';

	return (
		<html lang="en">
			<head>
				<ColorSchemeScript defaultColorScheme="light" />
			</head>
			<body>
				<MantineProvider theme={{ colors: colors, primaryColor: primaryColor }}>
					<ModalsProvider>
						<CustomAppShell>{children}</CustomAppShell>
					</ModalsProvider>
				</MantineProvider>
			</body>
		</html>
	);
}

