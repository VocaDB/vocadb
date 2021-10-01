import React from 'react';
import ReactGA from 'react-ga';
import { useLocation } from 'react-router-dom';

// Captures pageviews as a new page is loaded.
// The call to usePageTracking must go after useTitle.
// Set ready to false while translations are not yet loaded.
// This prevents Google Analytics from using an incomplete page title (e.g. `Index.Discussions - Vocaloid Database`).
const usePageTracking = (ready: boolean): void => {
	const location = useLocation();

	React.useEffect(() => {
		if (ready) ReactGA.pageview(`${location.pathname}${location.search}`);
	}, [ready, location]);
};

export default usePageTracking;
