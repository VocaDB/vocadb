import ErrorNotFound from '@/Pages/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const HomeIndex = React.lazy(() => import('./HomeIndex'));
const HomeChat = React.lazy(() => import('./HomeChat'));
const HomeWiki = React.lazy(() => import('./HomeWiki'));

const HomeRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="" element={<HomeIndex />} />
			<Route path="Home/Chat" element={<HomeChat />} />
			<Route path="Home/Wiki" element={<HomeWiki />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default HomeRoutes;
