import ErrorNotFound from '@Pages/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const VenueDetails = React.lazy(() => import('./VenueDetails'));
const VenueEdit = React.lazy(() => import('./VenueEdit'));
const VenueVersions = React.lazy(() => import('./VenueVersions'));

const VenueRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="Details/:id" element={<VenueDetails />} />
			<Route path="Edit" element={<VenueEdit />} />
			<Route path="Edit/:id" element={<VenueEdit />} />
			<Route path="Versions/:id" element={<VenueVersions />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default VenueRoutes;
