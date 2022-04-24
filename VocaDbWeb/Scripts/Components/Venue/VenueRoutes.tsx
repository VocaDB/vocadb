import ErrorNotFound from '@Components/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const VenueDetails = React.lazy(() => import('./VenueDetails'));
const VenueVersions = React.lazy(() => import('./VenueVersions'));

const VenueRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="Details/:id" element={<VenueDetails />} />
			<Route path="Versions/:id" element={<VenueVersions />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default VenueRoutes;
