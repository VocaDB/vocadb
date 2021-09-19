import ErrorNotFound from '@Components/Error/ErrorNotFound';
import React from 'react';
import { Routes } from 'react-router';
import { Route } from 'react-router-dom';

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
