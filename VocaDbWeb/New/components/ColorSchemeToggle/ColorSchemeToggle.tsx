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
import { useColorStore } from '@/stores/color';
import { groupBy } from '@/Helpers/Functions';
import miku from '@/public/characters/Hatsune Miku.png';
import luka from '@/public/characters/Megurine Luka.png';
import gumi from '@/public/characters/Gumi.png';
import solaria from '@/public/characters/Solaria.png';
import rin from '@/public/characters/Kagamine Rin.png';
import ia from '@/public/characters/IA.png';
import yukari from '@/public/characters/Yuzuki Yukari.png';
import teto from '@/public/characters/Kasane Teto.png';
import kafu from '@/public/characters/Kafu.png';
import flower from '@/public/characters/v flower.png';
import kaito from '@/public/characters/Kaito.png';
import meiko from '@/public/characters/Meiko.png';
import tianyi from '@/public/characters/Luo Tianyi.png';
import uta from '@/public/characters/Utane Uta.png';
import sasara from '@/public/characters/Sato Sasara.png';
import yufu from '@/public/characters/Sekka Yufu.png';
import fukase from '@/public/characters/Fukase.png';
import seeu from '@/public/characters/SeeU.png';
import avanna from '@/public/characters/AVANNA.png';
import nana from '@/public/characters/Macne Nana.png';
import xingchen from '@/public/characters/Xingchen.png';
import lumi from '@/public/characters/LUMi.png';
import nemu from '@/public/characters/Yumemi Nemu.png';
import pouta from '@/public/characters/Po-uta.png';
import una from '@/public/characters/Otomachi Una.png';
import kzn from '@/public/characters/#kzn.png';
import forte from '@/public/characters/Eleanor Forte.png';

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
		picture: rin,
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
	{ color: 'ia', picture: ia, name: 'IA', description: 'TODO', category: 'Vocaloid' },
	{
		color: 'yukari',
		picture: yukari,
		name: 'Yuzuki Yukari',
		description: 'TODO',
		category: 'Vocaloid',
	},
	{ color: 'teto', picture: teto, name: 'Kasane Teto', description: 'TODO', category: 'UTAU' },
	{ color: 'kafu', picture: kafu, name: 'KAFU', description: 'TODO', category: 'CeVIO' },
	{
		color: 'flower',
		picture: flower,
		name: 'v flower',
		description: 'TODO',
		category: 'Vocaloid',
	},
	{ color: 'kaito', picture: kaito, name: 'KAITO', description: 'TODO', category: 'Vocaloid' },
	{ color: 'meiko', picture: meiko, name: 'MEIKO', description: 'TODO', category: 'Vocaloid' },
	{
		color: 'tianyi',
		picture: tianyi,
		name: 'Luo Tianyi',
		description: 'TODO',
		category: 'Vocaloid',
	},
	{ color: 'uta', picture: uta, name: 'Utane Uta', description: 'TODO', category: 'UTAU' },
	{
		color: 'sasara',
		picture: sasara,
		name: 'Satou Sasara',
		description: 'TODO',
		category: 'CeVIO',
	},
	{ color: 'yufu', picture: yufu, name: 'Sekka Yufu', description: 'TODO', category: 'UTAU' },
	{ color: 'fukase', picture: fukase, name: 'Fukase', description: 'TODO', category: 'Vocaloid' },
	{ color: 'seeu', picture: seeu, name: 'SeeU', description: 'TODO', category: 'Vocaloid' },
	// TODO: Fix eleanor forte
	// {
	// 	color: 'forte',
	// 	picture: forte,
	// 	name: 'Eleanor Forte',
	// 	description: 'TODO',
	// 	category: 'Synthesizer V',
	// },
	{ color: 'avanna', picture: avanna, name: 'AVANNA', description: 'TODO', category: 'Vocaloid' },
	{
		color: 'nana',
		picture: nana,
		name: 'Macne Nana',
		description: 'TODO',
		category: 'GarageBand',
	},
	{
		color: 'xingchen',
		picture: xingchen,
		name: 'Xingchen',
		description: 'TODO',
		category: 'Vocaloid',
	},
	{ color: 'lumi', picture: lumi, name: 'LUMi', description: 'TODO', category: 'Vocaloid' },
	{
		color: 'nemu',
		picture: nemu,
		name: 'Yumemi Nemu',
		description: 'TODO',
		category: 'Vocaloid',
	},
	{ color: 'pouta', picture: pouta, name: 'Po-Uta', description: 'TODO', category: 'Vocaloid' },
	{
		color: 'una',
		picture: una,
		name: 'Otomachi Una',
		description: 'TODO',
		category: 'Vocaloid',
	},
	{ color: 'kzn', picture: kzn, name: '#kzn', description: 'TODO', category: 'Vocaloid' },
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

