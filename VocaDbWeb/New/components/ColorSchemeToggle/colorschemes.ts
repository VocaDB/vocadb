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
import { StaticImageData } from 'next/image';

export interface ColorScheme {
	color: string;
	picture: StaticImageData;
	name: string;
	description: string;
	category: string;
	id?: number;
	recommendedMode?: 'light' | 'dark';
}
export const COLORSCHEMES: ColorScheme[] = [
	{
		color: 'teto',
		picture: teto,
		name: 'Kasane Teto',
		description: 'TODO',
		category: 'UTAU',
		id: 116,
	},
	{
		color: 'meiko',
		picture: meiko,
		name: 'MEIKO',
		description: 'TODO',
		category: 'Vocaloid',
		id: 176,
	},
	{
		color: 'fukase',
		picture: fukase,
		name: 'Fukase',
		description: 'TODO',
		category: 'Vocaloid',
		id: 40866,
	},
	{
		color: 'seeu',
		picture: seeu,
		name: 'SeeU',
		description: 'TODO',
		category: 'Vocaloid',
		id: 113168,
	},
	{
		color: 'solaria',
		picture: solaria,
		name: 'SOLARIA',
		description: 'TODO',
		category: 'Synthesizer V',
		id: 76317,
	},
	{
		color: 'rin',
		picture: rin,
		name: 'Kagamine Rin',
		description: 'TODO',
		category: 'Vocaloid',
		id: 14,
		recommendedMode: 'dark',
	},
	{
		color: 'nana',
		picture: nana,
		name: 'Macne Nana',
		description: 'TODO',
		category: 'GarageBand',
		id: 36099,
		recommendedMode: 'dark',
	},
	{
		color: 'una',
		picture: una,
		name: 'Otomachi Una',
		description: 'TODO',
		category: 'Vocaloid',
		id: 120224,
		recommendedMode: 'dark',
	},
	{
		color: 'gumi',
		picture: gumi,
		name: 'GUMI',
		description: 'TODO',
		category: 'Vocaloid',
		id: 3,
	},
	{
		color: 'avanna',
		picture: avanna,
		name: 'AVANNA',
		description: 'TODO',
		category: 'Vocaloid',
		id: 2803,
		recommendedMode: 'light',
	},
	{
		color: 'nemu',
		picture: nemu,
		name: 'Yumemi Nemu',
		description: 'TODO',
		category: 'Vocaloid',
		id: 56153,
	},
	{
		color: 'miku',
		picture: miku,
		name: 'Hatsune Miku',
		description: 'TODO',
		category: 'Vocaloid',
		id: 1,
	},
	{
		color: 'lumi',
		picture: lumi,
		name: 'LUMi',
		description: 'TODO',
		category: 'Vocaloid',
		id: 58416,
		recommendedMode: 'dark',
	},
	{
		color: 'tianyi',
		picture: tianyi,
		name: 'Luo Tianyi',
		description: 'TODO',
		category: 'Vocaloid',
		id: 1778,
	},
	{
		color: 'kafu',
		picture: kafu,
		name: 'KAFU',
		description: 'TODO',
		category: 'CeVIO',
		id: 83928,
	},
	{
		color: 'kaito',
		picture: kaito,
		name: 'KAITO',
		description: 'TODO',
		category: 'Vocaloid',
		id: 71,
		recommendedMode: 'light',
	},
	{
		color: 'xingchen',
		picture: xingchen,
		name: 'Xingchen',
		description: 'TODO',
		category: 'Vocaloid',
		id: 35966,
		recommendedMode: 'light',
	},
	{
		color: 'yukari',
		picture: yukari,
		name: 'Yuzuki Yukari',
		description: 'TODO',
		category: 'Vocaloid',
		id: 623,
	},
	{
		color: 'uta',
		picture: uta,
		name: 'Utane Uta',
		description: 'TODO',
		category: 'UTAU',
		id: 803,
		recommendedMode: 'light',
	},
	{
		color: 'kzn',
		picture: kzn,
		name: '#kzn',
		description: 'TODO',
		category: 'CeVIO',
		id: 100054,
	},
	{
		color: 'luka',
		picture: luka,
		name: 'Megurine Luka',
		description: 'TODO',
		category: 'Vocaloid',
		id: 2,
	},
	{ color: 'ia', picture: ia, name: 'IA', description: 'TODO', category: 'Vocaloid', id: 504 },
	{
		color: 'sasara',
		picture: sasara,
		name: 'Satou Sasara',
		description: 'TODO',
		category: 'CeVIO',
		id: 9874,
	},
	{
		color: 'pouta',
		picture: pouta,
		name: 'Po-Uta',
		description: 'TODO',
		category: 'Vocaloid',
		id: 117049,
		recommendedMode: 'light',
	},
	{
		color: 'yufu',
		picture: yufu,
		name: 'Sekka Yufu',
		description: 'TODO',
		category: 'UTAU',
		id: 809,
	},
	{
		color: 'flower',
		picture: flower,
		name: 'v flower',
		description: 'TODO',
		category: 'Vocaloid',
		id: 21165,
		recommendedMode: 'light',
	},
	// TODO: Fix eleanor forte
	// {
	// 	color: 'forte',
	// 	picture: forte,
	// 	name: 'Eleanor Forte',
	// 	description: 'TODO',
	// 	category: 'Synthesizer V',
	// id: 66906
	// },
];

