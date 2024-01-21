import ErrorNotFound from '@/Pages/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const RewindIndex = React.lazy(() => import('./Rewind'));

const RewindRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="" element={<RewindIndex />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default RewindRoutes;
