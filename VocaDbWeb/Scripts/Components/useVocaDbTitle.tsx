import { usePageTracking } from '@/Components/usePageTracking';
import NProgress from 'nprogress';
import 'nprogress/nprogress.css';
import React from 'react';
import { useTitle } from 'react-use';

export const useVocaDbTitle = (
	title: string | undefined,
	ready: boolean,
): void => {
	useTitle(
		title ? `${title} - ${vdb.values.siteTitle}` : `${vdb.values.siteTitle}`,
	);

	usePageTracking(ready);

	React.useEffect(() => {
		NProgress.done();

		return (): void => {
			NProgress.start();
		};
	}, [title]);
};
