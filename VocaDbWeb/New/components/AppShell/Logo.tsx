import DarkLogo from '../../public/VocaDB_Logo_White_Transparent_No_Outline.png';
import LightLogo from '../../public/VocaDB_Logo_Black_Transparent_No_Outline.png';
import { useComputedColorScheme } from '@mantine/core';
import Image from 'next/image';

// TODO: Set the image src using css mixins and remove the next/dynamic restriction
export default function Logo() {
	const computedColorScheme = useComputedColorScheme('light', { getInitialValueInEffect: false });

	return (
		<Image
			style={{
				objectFit: 'contain',
				height: '100%',
			}}
			width={167}
			height={69}
			src={computedColorScheme === 'dark' ? DarkLogo : LightLogo}
			alt=""
		/>
	);
}

