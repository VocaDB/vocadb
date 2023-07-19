import Image, { ImageProps } from 'next/image';
import React from 'react';

interface CustomImageProps extends ImageProps {
	mode?: 'crop';
}

export default function CustomImage(props: CustomImageProps) {
	return (
		<Image
			{...props}
			src={props.src}
			width={props.width}
			height={props.height}
			loader={(loaderProps) => {
				if (loaderProps.src === '/unknown.png') {
					return '/unknown.webp';
				}
				let base = `//wsrv.nl/?url=${loaderProps.src}&output=webp`;

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

