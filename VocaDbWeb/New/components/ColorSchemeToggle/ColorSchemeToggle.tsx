import { ColorSwatch, Group, Paper, Switch, useMantineTheme } from '@mantine/core';
import { modals } from '@mantine/modals';
import { IconSun, IconMoonStars } from '@tabler/icons-react';
import { useColorStore } from '@/stores/useColorStore';
import React from 'react';
import dynamic from 'next/dynamic';

const ColorSchemeMenu = dynamic(() => import('./ColorSchemeMenu'), {
	loading: () => <Paper h="1000px" />,
});

export default function ColorSchemeToggle() {
	const theme = useMantineTheme();
	const [colorScheme, toggleColorScheme] = useColorStore((state) => [
		state.colorScheme,
		state.toggleColorScheme,
	]);

	const openModal = () =>
		modals.open({
			title: 'Change your color scheme',
			size: 'xl',
			children: <ColorSchemeMenu />,
		});

	return (
		<Group position="center" mr={10}>
			<ColorSwatch<'button'>
				component="button"
				onClick={openModal}
				color={theme.colors[theme.primaryColor][6]}
				title="Open color scheme menu"
			/>
			<Switch
				checked={colorScheme === 'dark'}
				onChange={() => toggleColorScheme()}
				size="md"
				onLabel={<IconSun color={theme.white} size="1.25rem" stroke={1.5} />}
				offLabel={
					<IconMoonStars color={theme.colors.gray[6]} size="1.25rem" stroke={1.5} />
				}
				aria-label="Toggle color scheme"
			/>
		</Group>
	);
}

