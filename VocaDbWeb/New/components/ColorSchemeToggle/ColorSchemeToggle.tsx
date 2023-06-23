import {
	ColorSwatch,
	Group,
	Switch,
	useMantineTheme,
	Text,
	Grid,
	Card,
	Button,
	Accordion,
} from '@mantine/core';
import { modals } from '@mantine/modals';
import { IconSun, IconMoonStars } from '@tabler/icons';
import Image, { StaticImageData } from 'next/image';
import miku from '@/public/characters/Hatsune Miku.png';
import luka from '@/public/characters/Megurine Luka.png';
import gumi from '@/public/characters/Gumi.png';
import solaria from '@/public/characters/Solaria.png';
import { useColorStore } from '@/stores/color';
import { groupBy } from '@/Helpers/Functions';

interface ColorScheme {
	color: string;
	picture: StaticImageData;
	name: string;
	description: string;
	category: string;
}

const COLORSCHEMES: ColorScheme[] = [
	{
		color: 'miku',
		picture: miku,
		name: 'Hatsune Miku',
		description: 'TODO',
		category: 'Vocaloid',
	},
	{
		color: 'rin',
		picture: miku,
		name: 'Kagamine Rin',
		description: 'TODO',
		category: 'Vocaloid',
	},
	{
		color: 'luka',
		picture: luka,
		name: 'Megurine Luka',
		description: 'TODO',
		category: 'Vocaloid',
	},
	{ color: 'gumi', picture: gumi, name: 'GUMI', description: 'TODO', category: 'Vocaloid' },
	{ color: 'ia', picture: gumi, name: 'IA', description: 'TODO', category: 'Vocaloid' },
	{
		color: 'yukari',
		picture: gumi,
		name: 'Yuzuki Yukari',
		description: 'TODO',
		category: 'Vocaloid',
	},
	{ color: 'teto', picture: gumi, name: 'Kasane Teto', description: 'TODO', category: 'UTAU' },
	{ color: 'kafu', picture: gumi, name: 'KAFU', description: 'TODO', category: 'CeVIO' },
	{ color: 'flower', picture: gumi, name: 'v flower', description: 'TODO', category: 'Vocaloid' },
	{ color: 'kaito', picture: gumi, name: 'KAITO', description: 'TODO', category: 'Vocaloid' },
	{ color: 'meiko', picture: gumi, name: 'MEIKO', description: 'TODO', category: 'Vocaloid' },
	{
		color: 'tianyi',
		picture: gumi,
		name: 'Luo Tianyi',
		description: 'TODO',
		category: 'Vocaloid',
	},
	{ color: 'uta', picture: gumi, name: 'Utane Uta', description: 'TODO', category: 'UTAU' },
	{
		color: 'sasara',
		picture: gumi,
		name: 'Satou Sasara',
		description: 'TODO',
		category: 'CeVIO',
	},
	{ color: 'yufu', picture: gumi, name: 'Sekka Yufu', description: 'TODO', category: 'UTAU' },
	{ color: 'fukase', picture: gumi, name: 'Fukase', description: 'TODO', category: 'Vocaloid' },
	{ color: 'seeu', picture: gumi, name: 'SeeU', description: 'TODO', category: 'Vocaloid' },
	{
		color: 'forte',
		picture: gumi,
		name: 'Eleanor Forte',
		description: 'TODO',
		category: 'Vocaloid',
	},
	{ color: 'avanna', picture: gumi, name: 'AVANNA', description: 'TODO', category: 'Vocaloid' },
	{
		color: 'nana',
		picture: gumi,
		name: 'Macne Nana',
		description: 'TODO',
		category: 'GarageBand',
	},
	{
		color: 'xingchen',
		picture: gumi,
		name: 'Xingchen',
		description: 'TODO',
		category: 'Vocaloid',
	},
	{ color: 'lumi', picture: gumi, name: 'LUMi', description: 'TODO', category: 'Vocaloid' },
	{
		color: 'nemu',
		picture: gumi,
		name: 'Yumemi Nemu',
		description: 'TODO',
		category: 'Vocaloid',
	},
	{ color: 'pouta', picture: gumi, name: 'Po-Uta', description: 'TODO', category: 'Vocaloid' },
	{
		color: 'una',
		picture: gumi,
		name: 'Otomachi Una',
		description: 'TODO',
		category: 'Vocaloid',
	},
	{ color: 'kzn', picture: gumi, name: '#kzn', description: 'TODO', category: 'Vocaloid' },
	{
		color: 'solaria',
		picture: solaria,
		name: 'SOLARIA',
		description: 'TODO',
		category: 'Synthesizer V',
	},
];

const GROUPED_COLORSCHEMES = groupBy(COLORSCHEMES, (val) => val.category);

const ColorSchemeCard = ({ scheme }: { scheme: ColorScheme }) => {
	const [setPrimaryColor] = useColorStore((state) => [state.setPrimaryColor]);

	return (
		<Card shadow="sm" radius="md">
			<Image
				src={scheme.picture}
				style={{ marginLeft: 'auto', marginRight: 'auto', display: 'block' }}
				height={120}
				alt="Hatsune Miku"
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
			>
				Apply Color
			</Button>
		</Card>
	);
};

export function ColorSchemeToggle() {
	const theme = useMantineTheme();
	const [colorScheme, toggleColorScheme] = useColorStore((state) => [
		state.colorScheme,
		state.toggleColorScheme,
	]);

	const colorSchemes = Object.keys(GROUPED_COLORSCHEMES).map((group) => (
		<Accordion.Item value={group}>
			<Accordion.Control>{group}</Accordion.Control>
			<Accordion.Panel>
				<Grid>
					{GROUPED_COLORSCHEMES[group].map((scheme) => (
						<Grid.Col md={4} sm={12}>
							<ColorSchemeCard scheme={scheme} />
						</Grid.Col>
					))}
				</Grid>
			</Accordion.Panel>
		</Accordion.Item>
	));

	const openModal = () =>
		modals.open({
			title: 'Change your color scheme',
			size: 'xl',
			children: (
				<>
					<Text size="sm">You can change your preferred color scheme here.</Text>

					<Accordion>{colorSchemes}</Accordion>
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

