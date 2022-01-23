import $ from 'jquery';
import React from 'react';

interface CarouselProps {
	children: React.ReactNode;
}

const Carousel = ({ children }: CarouselProps): React.ReactElement => {
	const el = React.useRef<HTMLDivElement>(undefined!);

	React.useLayoutEffect(() => {
		const $el = $(el.current);
		$el.carousel({ interval: false });
	}, []);

	return (
		<div id="carousel" className="carousel slide" ref={el}>
			{children}
		</div>
	);
};

export default Carousel;
