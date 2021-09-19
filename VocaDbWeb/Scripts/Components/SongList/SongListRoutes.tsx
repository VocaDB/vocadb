import ErrorNotFound from '@Components/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const SongListFeatured = React.lazy(
	() => import('@Components/SongList/SongListFeatured'),
);

const SongListRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="Featured" element={<SongListFeatured />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default SongListRoutes;
