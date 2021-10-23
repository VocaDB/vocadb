import ErrorNotFound from '@Components/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const SongListDetails = React.lazy(() => import('./SongListDetails'));
const SongListFeatured = React.lazy(() => import('./SongListFeatured'));

const SongListRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="Details/:id" element={<SongListDetails />} />
			<Route path="Featured" element={<SongListFeatured />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default SongListRoutes;
