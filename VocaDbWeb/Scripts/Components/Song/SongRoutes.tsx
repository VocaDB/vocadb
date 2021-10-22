import ErrorNotFound from '@Components/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router';

const SongRankings = React.lazy(() => import('./SongRankings'));

const SongRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="Rankings" element={<SongRankings />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default SongRoutes;
