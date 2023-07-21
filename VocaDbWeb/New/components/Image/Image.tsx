import Image, { ImageProps } from 'next/image';
import React from 'react';

interface CustomImageProps extends Omit<ImageProps, 'src'> {
	src: string | undefined;
	mode?: 'crop';
}

export default function CustomImage(props: CustomImageProps) {
	return (
		<Image
			{...props}
			src={props.src ?? '/unknown.webp'}
			width={props.width}
			height={props.height}
			loader={(loaderProps) => {
				if (loaderProps.src === '/unknown.png' || props.src === undefined) {
					return '/unknown.webp';
				}

				let base = `//wsrv.nl/?url=${
					loaderProps.src.startsWith('/') ? process.env.NEXT_PUBLIC_API_URL : ''
				}${loaderProps.src}&output=webp`;

				if (props.mode === 'crop') {
					base += '&fit=cover&a=attention';
				}
				if (props.width) {
					base += '&w=' + props.width;
					base += '&h=' + props.height;
				}

				return base;
			}}
		/>
	);
}

