import ActivateNewFrontend from '@/ActivateNewFrontend';
import { RouteObject } from 'react-router-dom';

export const routes: RouteObject[] = [
	{
		path: '/',
		element: <div>Hello, World!</div>,
	},
	{
		path: '/beta',
		element: <ActivateNewFrontend />,
	},
];
