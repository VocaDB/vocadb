import ErrorNotFound from '@/Pages/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const SongListDetails = React.lazy(() => import('./SongListDetails'));
const SongListEdit = React.lazy(() => import('./SongListEdit'));
const SongListFeatured = React.lazy(() => import('./SongListFeatured'));
const SongListImport = React.lazy(() => import('./SongListImport'));
const SongListVersions = React.lazy(() => import('./SongListVersions'));

const SongListRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="Details/:id" element={<SongListDetails />} />
			<Route path="Edit" element={<SongListEdit />} />
			<Route path="Edit/:id" element={<SongListEdit />} />
			<Route path="Featured" element={<SongListFeatured />} />
			<Route path="Import" element={<SongListImport />} />
			<Route path="Versions/:id" element={<SongListVersions />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default SongListRoutes;
