import React from 'react';
import { Route, Routes } from 'react-router';

const SongRankings = React.lazy(() => import('./SongRankings'));

const SongRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="/Rankings" element={<SongRankings />} />
		</Routes>
	);
};

export default SongRoutes;
