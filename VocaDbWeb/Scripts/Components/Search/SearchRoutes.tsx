import React from 'react';
import { Routes } from 'react-router';
import { Route } from 'react-router-dom';

const SearchIndex = React.lazy(() => import('./SearchIndex'));

const SearchRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="/" element={<SearchIndex />} />
		</Routes>
	);
};

export default SearchRoutes;
