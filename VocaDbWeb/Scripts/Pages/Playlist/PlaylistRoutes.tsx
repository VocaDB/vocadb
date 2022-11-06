import ErrorNotFound from '@/Pages/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const PlaylistIndex = React.lazy(() => import('./PlaylistIndex'));

const PlaylistRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="" element={<PlaylistIndex />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default PlaylistRoutes;
