import React from 'react';
import { Route, Routes } from 'react-router-dom';

import ErrorIndex from './ErrorIndex';

const ErrorForbidden = React.lazy(() => import('./ErrorForbidden'));
const ErrorNotFound = React.lazy(() => import('./ErrorNotFound'));
const ErrorIPForbidden = React.lazy(() => import('./ErrorIPForbidden'));
const ErrorBadRequest = React.lazy(() => import('./ErrorBadRequest'));

const ErrorRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="" element={<ErrorIndex />} />
			<Route path="Forbidden" element={<ErrorForbidden />} />
			<Route path="IPForbidden" element={<ErrorIPForbidden />} />
			<Route path="BadRequest" element={<ErrorBadRequest />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default ErrorRoutes;
