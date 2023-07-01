import Image, { ImageProps } from 'next/image';
import React from 'react';

interface CustomImageProps extends ImageProps {
	mode?: 'crop';
}

export default function CustomImage(props: CustomImageProps) {
	let base = `//wsrv.nl/?url=${props.src}&output=webp`;

	if (props.mode === 'crop') {
		base += '&fit=cover&a=attention';
	}
	if (props.width) {
		base += '&w=' + props.width;
	}

	if (props.height) {
		base += '&h=' + props.height;
	}

	if (typeof window !== 'undefined' && window.devicePixelRatio) {
		base += '&dpr=' + window.devicePixelRatio;
	}

	return <Image {...props} src={base} unoptimized />;
}

