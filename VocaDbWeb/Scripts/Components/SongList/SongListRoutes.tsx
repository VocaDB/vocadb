import React from 'react';
import { Route, Routes } from 'react-router-dom';

const SongListFeatured = React.lazy(
	() => import('@Components/SongList/SongListFeatured'),
);

const SongListRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="/Featured" element={<SongListFeatured />} />
		</Routes>
	);
};

export default SongListRoutes;
