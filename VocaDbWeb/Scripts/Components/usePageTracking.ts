import React from 'react';
import ReactGA from 'react-ga';
import { useLocation } from 'react-router-dom';

const usePageTracking = (): void => {
	const location = useLocation();

	React.useEffect(() => {
		ReactGA.pageview(`${location.pathname}${location.search}`);
	}, [location]);
};

export default usePageTracking;
