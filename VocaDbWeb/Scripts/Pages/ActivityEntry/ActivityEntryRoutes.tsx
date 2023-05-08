import ErrorNotFound from '@/Pages/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const ActivityEntryIndex = React.lazy(() => import('./ActivityEntryIndex'));
const FollowedArtistActivity = React.lazy(
	() => import('./FollowedArtistActivity'),
);

const ActivityEntryRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="" element={<ActivityEntryIndex />} />
			<Route
				path="FollowedArtistActivity"
				element={<FollowedArtistActivity />}
			/>
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default ActivityEntryRoutes;
