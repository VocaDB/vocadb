import ErrorNotFound from '@Components/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const EventIndex = React.lazy(() => import('./EventIndex'));
const EventDetails = React.lazy(() => import('./EventDetails'));
const EventEdit = React.lazy(() => import('./EventEdit'));
const EventSeriesDetails = React.lazy(() => import('./EventSeriesDetails'));
const EventSeriesVersions = React.lazy(() => import('./EventSeriesVersions'));
const EventVersions = React.lazy(() => import('./EventVersions'));

const EventRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="" element={<EventIndex />} />
			<Route path="Details/:id" element={<EventDetails />} />
			<Route path="Edit" element={<EventEdit />} />
			<Route path="Edit/:id" element={<EventEdit />} />
			<Route path="SeriesDetails/:id" element={<EventSeriesDetails />} />
			<Route path="SeriesVersions/:id" element={<EventSeriesVersions />} />
			<Route path="Versions/:id" element={<EventVersions />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default EventRoutes;
