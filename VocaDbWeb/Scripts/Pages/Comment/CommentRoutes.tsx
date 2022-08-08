import ErrorNotFound from '@/Pages/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const CommentIndex = React.lazy(() => import('./CommentIndex'));

const CommentRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="" element={<CommentIndex />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default CommentRoutes;
