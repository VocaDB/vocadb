import ErrorNotFound from '@Components/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const ActivityEntryIndex = React.lazy(() => import('./ActivityEntryIndex'));

const ActivityEntryRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="" element={<ActivityEntryIndex />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default ActivityEntryRoutes;
