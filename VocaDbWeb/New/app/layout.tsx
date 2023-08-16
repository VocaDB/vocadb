import '@mantine/core/styles.css';

import { MantineProvider, ColorSchemeScript } from '@mantine/core';
import CustomAppShell from '@/components/AppShell/AppShell';

export const metadata = {
	title: 'My Mantine app',
	description: 'I have followed setup instructions carefully',
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
	return (
		<html lang="en">
			<head>
				<ColorSchemeScript defaultColorScheme="light" />
			</head>
			<body>
				<MantineProvider>
					<CustomAppShell>{children}</CustomAppShell>
				</MantineProvider>
			</body>
		</html>
	);
}

