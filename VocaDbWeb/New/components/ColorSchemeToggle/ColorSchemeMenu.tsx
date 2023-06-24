import { useColorStore } from '@/stores/color';
import { Card, ActionIcon, Button, Text, Grid } from '@mantine/core';
import { IconInfoCircle } from '@tabler/icons-react';
import Link from 'next/link';
import Image from 'next/image';
import { COLORSCHEMES, ColorScheme } from './colorschemes';

const ColorSchemeCard = ({ scheme }: { scheme: ColorScheme }) => {
	const [setPrimaryColor] = useColorStore((state) => [state.setPrimaryColor]);

	return (
		<Card shadow="sm" radius="md">
			<ActionIcon
				component={Link}
				href={`/A/${scheme.id ?? 1}`}
				size="sm"
				style={{ position: 'absolute', right: 5 }}
				variant="subtle"
				title={`${scheme.name}`}
			>
				<IconInfoCircle />
			</ActionIcon>
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

