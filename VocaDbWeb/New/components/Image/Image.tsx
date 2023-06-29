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
				let base = `//wsrv.nl/?url=${loaderProps.src}&output=webp`;

				if (props.mode === 'crop') {
					base += '&fit=cover&a=attention';
				}
				if (props.width) {
					base += '&w=' + loaderProps.width;
				}

				return base;
			}}
		/>
	);

	return <Image {...props} src={base} unoptimized />;
}

