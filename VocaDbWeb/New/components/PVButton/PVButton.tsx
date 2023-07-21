import { PVContract, PVContractBase } from '@/types/DataContracts/PVs/PVContract';
import { PVService } from '@/types/Models/PVs/PVService';
import { Button } from '@mantine/core';
import {
	IconBrandBandcamp,
	IconBrandBilibili,
	IconBrandVimeo,
	IconBrandYoutube,
	IconVideo,
} from '@tabler/icons-react';

interface PVButtonProps {
	pv: PVContract;
	onClick?: () => any;
}

// https://github.com/tabler/tabler-icons/issues/720
const NicoNicoIcon = () => {
	return (
		<svg
			fill="none"
			stroke="currentColor"
			stroke-width="2"
			stroke-linecap="round"
			stroke-linejoin="round"
			role="img"
			viewBox="0 0 24 24"
			width="20"
			height="20"
			xmlns="http://www.w3.org/2000/svg"
		>
			<title>niconico icon</title>
			<path d="M.4787 7.534v12.1279A2.0213 2.0213 0 0 0 2.5 21.6832h2.3888l1.323 2.0948a.4778.4778 0 0 0 .4043.2205.4778.4778 0 0 0 .441-.2205l1.323-2.0948h6.9828l1.323 2.0948a.4778.4778 0 0 0 .441.2205c.1838 0 .3308-.0735.4043-.2205l1.323-2.0948h2.6462a2.0213 2.0213 0 0 0 2.0213-2.0213V7.5339a2.0213 2.0213 0 0 0-2.0213-1.9845h-7.681l4.4468-4.4469L17.1637 0l-5.1452 5.1452L6.8 0 5.6973 1.1025l4.4102 4.4102H2.5367a2.0213 2.0213 0 0 0-2.058 2.058z" />
		</svg>
	);
};

const PiaproIcon = () => {
	return (
		<svg
			fill="none"
			stroke="currentColor"
			strokeWidth="2"
			strokeLinecap="round"
			strokeLinejoin="round"
			width="24"
			height="24"
			viewBox="0 0 20 20"
		>
			<defs id="defs58" />
			<title id="title36">logo_piapro</title>
			<g
				id="レイヤー_2"
				data-name="レイヤー 2"
				style={{ fill: '#e4007b', fillOpacity: 1 }}
				transform="translate(0,-0.04596567)"
			>
				<g
					id="基本ロゴ"
					style={{ fill: '#ffffff', fillOpacity: 1 }}
					transform="matrix(0.22183657,0,0,0.22183657,2.067521,-0.70762862)"
				>
					<path
						d="M 31.949,3.378 A 31.925,31.925 0 0 0 0.323,31 8.528,8.528 0 0 0 0,33.272 v 51.247 l 17.636,-11.5 v -37.688 0 a 14.345,14.345 0 1 1 6.9,12.222 V 66.391 A 31.943,31.943 0 1 0 31.949,3.378 Z"
						fill="#fff"
						id="path40"
						style={{ fill: '#ffffff', display: 'inline', fillOpacity: 1 }}
					/>
				</g>
			</g>
		</svg>
	);
};

export default function PVButton({ pv, onClick }: PVButtonProps) {
	let ServiceIcon = IconVideo;

	switch (pv.service) {
		case PVService.Youtube:
			ServiceIcon = IconBrandYoutube;
			break;
		case PVService.Bandcamp:
			ServiceIcon = IconBrandBandcamp;
			break;
		case PVService.Bilibili:
			ServiceIcon = IconBrandBilibili;
			break;
		case PVService.NicoNicoDouga:
			ServiceIcon = NicoNicoIcon;
			break;
		case PVService.Piapro:
			ServiceIcon = PiaproIcon;
			break;
		case PVService.Vimeo:
			ServiceIcon = IconBrandVimeo;
			break;
	}

	return (
		<Button maw="100%" onClick={onClick} variant="default" leftIcon={<ServiceIcon />}>
			{pv.name}
		</Button>
	);
}

