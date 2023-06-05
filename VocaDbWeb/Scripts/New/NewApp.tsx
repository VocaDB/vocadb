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
} from '@mantine/core';
import { IconMusic } from '@tabler/icons-react';

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
	return (
		<MantineProvider
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
			}}
		>
			<AppShell
				navbarOffsetBreakpoint="sm"
				navbar={
					<Navbar p="md" hiddenBreakpoint="sm" width={{ sm: 200, lg: 300 }}>
						{linkData.map((link) => (
							<MainLink {...link} key={link.label} />
						))}
					</Navbar>
				}
				header={
					<Header height={{ base: 50, md: 70 }} p="md">
						{/* Header content */}
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
				{/* Your application here */}
			</AppShell>
		</MantineProvider>
	);
};

export default NewApp;
