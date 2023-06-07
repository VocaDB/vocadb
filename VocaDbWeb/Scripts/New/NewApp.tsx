import {
	AppShell,
	Navbar,
	Header,
	UnstyledButton,
	Group,
	ThemeIcon,
	Text,
	MantineColor,
	MantineProvider,
	MediaQuery,
	Burger,
	useMantineTheme,
} from '@mantine/core';
import { IconMusic } from '@tabler/icons-react';
import { useState } from 'react';
import { RouterProvider, createBrowserRouter } from 'react-router-dom';

import { routes } from './routes';

const linkData = [
	{ icon: <IconMusic size="1rem" />, color: 'teal', label: 'Songs' },
];

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

const NewApp = (): React.ReactElement => {
	const theme = useMantineTheme();
	const [opened, setOpened] = useState(false);

	const router = createBrowserRouter(routes);

	return (
		<MantineProvider
			withNormalizeCSS
			withGlobalStyles
			theme={{
				colors: {
					teal: [
						'#EDF8F7',
						'#CCEAE9',
						'#ACDDDB',
						'#8BD0CD',
						'#6AC3BF',
						'#4AB6B1',
						'#3B918E',
						'#2C6D6A',
						'#1D4947',
						'#0F2423',
					],
				},
				primaryColor: 'teal',
				colorScheme: 'dark',
			}}
		>
			<AppShell
				navbarOffsetBreakpoint="sm"
				navbar={
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
				}
				header={
					<Header
						height={{ base: 50, md: 70 }}
						style={{ display: 'flex' }}
						px="md"
					>
						<MediaQuery smallerThan="sm" styles={{ display: 'none' }}>
							<img
								style={{ maxHeight: '100%' }}
								src="/New/VocaDB_Logo_White_Transparent_No_Outline.png"
								alt=""
							/>
						</MediaQuery>
						<div
							style={{ display: 'flex', alignItems: 'center', height: '100%' }}
						>
							<MediaQuery largerThan="sm" styles={{ display: 'none' }}>
								<Burger
									opened={opened}
									onClick={(): void => setOpened((o) => !o)}
									size="sm"
									color={theme.colors.gray[6]}
									mr="xl"
								/>
							</MediaQuery>
						</div>
					</Header>
				}
				// eslint-disable-next-line @typescript-eslint/explicit-function-return-type
				styles={(theme) => ({
					main: {
						backgroundColor:
							theme.colorScheme === 'dark'
								? theme.colors.dark[8]
								: theme.colors.gray[0],
					},
				})}
			>
				<RouterProvider router={router} />
			</AppShell>
		</MantineProvider>
	);
};

export default NewApp;
