import ErrorNotFound from '@Components/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const EventIndex = React.lazy(() => import('./EventIndex'));
const EventDetails = React.lazy(() => import('./EventDetails'));
const EventSeriesDetails = React.lazy(() => import('./EventSeriesDetails'));

const EventRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="" element={<EventIndex />} />
			<Route path="Details/:id" element={<EventDetails />} />
			<Route path="SeriesDetails/:id" element={<EventSeriesDetails />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default EventRoutes;
