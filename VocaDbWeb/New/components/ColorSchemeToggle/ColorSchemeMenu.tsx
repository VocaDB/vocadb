import { useColorStore } from '@/stores/color';
import {
	Card,
	ActionIcon,
	Button,
	Text,
	Grid,
	Stack,
	ThemeIcon,
	useMantineTheme,
} from '@mantine/core';
import { IconInfoCircle, IconMoonStars, IconSun } from '@tabler/icons-react';
import Link from 'next/link';
import Image from 'next/image';
import { COLORSCHEMES, ColorScheme } from './colorschemes';

const ColorSchemeCard = ({ scheme }: { scheme: ColorScheme }) => {
	const theme = useMantineTheme();
	const [setPrimaryColor] = useColorStore((state) => [state.setPrimaryColor]);

	const lightMode = !scheme.recommendedMode || scheme.recommendedMode === 'light';
	const darkMode = !scheme.recommendedMode || scheme.recommendedMode === 'dark';
	const iconColor = theme.colors.gray[theme.colorScheme === 'dark' ? 2 : 6];

	return (
		<Card shadow="sm" radius="md">
			<Stack spacing="xs" style={{ position: 'absolute', right: 10 }}>
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

			<Text weight={400} align="center" mt="sm">
				{scheme.name}
			</Text>

			<Text size="sm" color="dimmed">
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
				<Grid.Col key={scheme.id} md={4} sm={12}>
					<ColorSchemeCard scheme={scheme} />
				</Grid.Col>
			))}
		</Grid>
	);

	return (
		<>
			<Text size="sm">You can change your preferred color scheme here.</Text>

			{colorSchemes}
		</>
	);
}

