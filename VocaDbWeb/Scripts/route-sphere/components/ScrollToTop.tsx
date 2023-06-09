import React from 'react';
import { useLocation } from 'react-router-dom';

export const ScrollToTop = (): null => {
	const location = useLocation();

	React.useLayoutEffect(() => {
		window.scrollTo(0, 0);
	}, [location]);

	return null;
};
