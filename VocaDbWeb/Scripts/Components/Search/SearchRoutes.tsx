import ErrorNotFound from '@Components/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const SearchIndex = React.lazy(() => import('./SearchIndex'));

const SearchRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="" element={<SearchIndex />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default SearchRoutes;
