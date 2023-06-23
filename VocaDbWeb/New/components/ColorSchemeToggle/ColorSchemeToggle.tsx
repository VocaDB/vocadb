import {
	ColorSwatch,
	Group,
	Switch,
	useMantineTheme,
	Text,
	Grid,
	Card,
	Button,
} from '@mantine/core';
import { modals } from '@mantine/modals';
import { IconSun, IconMoonStars } from '@tabler/icons';
import Image from 'next/image';
import miku from '@/public/characters/Hatsune Miku.png';
import luka from '@/public/characters/Megurine Luka.png';
import gumi from '@/public/characters/Gumi.png';
import solaria from '@/public/characters/Solaria.png';
import { useColorStore } from '@/stores/color';

const COLORSCHEMES = [
	{ color: 'miku', picture: miku, name: 'Hatsune Miku', description: 'TODO' },
	{ color: 'luka', picture: luka, name: 'Megurine Luka', description: 'TODO' },
	{ color: 'gumi', picture: gumi, name: 'GUMI', description: 'TODO' },
	{ color: 'solaria', picture: solaria, name: 'Solaria', description: 'TODO' },
];

export function ColorSchemeToggle() {
	const theme = useMantineTheme();
	const [setPrimaryColor, colorScheme, toggleColorScheme] = useColorStore((state) => [
		state.setPrimaryColor,
		state.colorScheme,
		state.toggleColorScheme,
	]);

	const colorSchemes = COLORSCHEMES.map((scheme) => (
		<Grid.Col md={6} sm={12} key={scheme.color}>
			<Card shadow="sm" radius="md">
				<Image
					src={scheme.picture}
					style={{ marginLeft: 'auto', marginRight: 'auto', display: 'block' }}
					height={120}
					alt="Hatsune Miku"
				/>

				<Text mt="sm" size="sm" color="dimmed">
					Maybe a description here?. Velit sint sint sit do esse incididunt aliquip eu
					aliquip veniam ad in. Culpa cillum sunt irure quis nostrud qui laborum.
				</Text>

				<Button
					onClick={() => setPrimaryColor(scheme.color)}
					mt="sm"
					color={scheme.color}
					radius="md"
					fullWidth
				>
					Apply Color
				</Button>
			</Card>
		</Grid.Col>
	));

	const openModal = () =>
		modals.open({
			title: 'Change your color scheme',
			children: (
				<>
					<Text size="sm">You can change your preferred color scheme here.</Text>
					<Grid>{colorSchemes}</Grid>
				</>
			),
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

