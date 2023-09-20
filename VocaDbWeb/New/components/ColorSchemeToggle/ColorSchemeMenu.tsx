import { useColorStore } from '@/stores/useColorStore';
import {
	Card,
	ActionIcon,
	Button,
	Text,
	Grid,
	Stack,
	ThemeIcon,
	useMantineTheme,
	useMantineColorScheme,
	useComputedColorScheme,
} from '@mantine/core';
import { IconInfoCircle, IconMoonStars, IconSun } from '@tabler/icons-react';
import Link from 'next/link';
import Image from 'next/image';
import { COLORSCHEMES, ColorScheme } from './colorschemes';

const ColorSchemeCard = ({ scheme }: { scheme: ColorScheme }) => {
	const theme = useMantineTheme();
	const colorScheme = useComputedColorScheme('light');
	const [setPrimaryColor] = useColorStore((state) => [state.setPrimaryColor]);

	const lightMode = !scheme.recommendedMode || scheme.recommendedMode === 'light';
	const darkMode = !scheme.recommendedMode || scheme.recommendedMode === 'dark';
	const iconColor = theme.colors.gray[colorScheme === 'dark' ? 2 : 6];

	return (
		<Card shadow="sm" radius="md">
			<Stack gap="xs" style={{ position: 'absolute', right: 10 }}>
				<ActionIcon
					component={Link}
					href={`/A/${scheme.id ?? 1}`}
					size="sm"
					variant="subtle"
					title={`${scheme.name}`}
				>
					<IconInfoCircle />
				</ActionIcon>
				<ThemeIcon
					style={!lightMode ? { visibility: 'hidden', pointerEvents: 'none' } : {}}
					title="Works well with light mode"
					variant="subtle"
					size="sm"
				>
					<IconSun color={iconColor} />
				</ThemeIcon>
				<ThemeIcon
					style={!darkMode ? { visibility: 'hidden', pointerEvents: 'none' } : {}}
					title="Works well with dark mode"
					size="sm"
					variant="subtle"
				>
					<IconMoonStars color={iconColor} />
				</ThemeIcon>
			</Stack>
			<Image
				src={scheme.picture}
				style={{ marginLeft: 'auto', marginRight: 'auto', display: 'block' }}
				height={120}
				alt=""
			/>

			<Text fw={400} ta="center" mt="sm">
				{scheme.name}
			</Text>

			<Text size="sm" c="dimmed">
				Maybe a description here?. Velit sint sint sit do esse incididunt aliquip eu aliquip
				veniam ad in. Culpa cillum sunt irure quis nostrud qui laborum.
			</Text>

			<Button
				onClick={() => setPrimaryColor(scheme.color)}
				mt="sm"
				color={scheme.color}
				radius="md"
				fullWidth
				title="Apply Color"
			>
				Apply Color
			</Button>
		</Card>
	);
};

export default function ColorSchemeMenu() {
	const colorSchemes = (
		<Grid>
			{COLORSCHEMES.map((scheme) => (
				<Grid.Col key={scheme.id} span={{ base: 12, xs: 6, sm: 4 }}>
					<ColorSchemeCard scheme={scheme} />
				</Grid.Col>
			))}
		</Grid>
	);

	return (
		<>
			<Text size="sm" mb="md">
				You can change your preferred color scheme here.
			</Text>

			{colorSchemes}
		</>
	);
}

