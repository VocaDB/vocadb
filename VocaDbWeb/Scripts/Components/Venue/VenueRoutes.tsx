import ErrorNotFound from '@Components/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const VenueDetails = React.lazy(() => import('./VenueDetails'));

const VenueRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="Details/:id" element={<VenueDetails />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default VenueRoutes;
