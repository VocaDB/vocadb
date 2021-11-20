import ErrorNotFound from '@Components/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const EventIndex = React.lazy(() => import('./EventIndex'));

const EventRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="" element={<EventIndex />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default EventRoutes;
