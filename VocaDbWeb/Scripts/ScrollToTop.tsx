import React from 'react';
import { useLocation } from 'react-router-dom';

const ScrollToTop = (): React.ReactElement => {
	const { pathname } = useLocation();

	React.useLayoutEffect(() => {
		window.scrollTo(0, 0);
	}, [pathname]);

	return <></>;
};

export default ScrollToTop;
