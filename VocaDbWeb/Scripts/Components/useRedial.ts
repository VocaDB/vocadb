import qs from 'qs';
import React from 'react';
import { useLocation, useNavigate } from 'react-router-dom';

const buildPath = <T>(pathname: string, from: T, to: T): string =>
	`${pathname}?${qs.stringify({ ...from, ...to })}`;

const useRedial = <T>(from: T): ((to: T) => void) => {
	const navigate = useNavigate();
	const { pathname } = useLocation();

	return React.useCallback((to: T) => navigate(buildPath(pathname, from, to)), [
		navigate,
		pathname,
		from,
	]);
};

export default useRedial;
