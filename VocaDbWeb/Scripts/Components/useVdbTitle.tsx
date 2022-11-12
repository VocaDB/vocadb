import { usePageTracking } from '@/Components/usePageTracking';
import NProgress from 'nprogress';
import React from 'react';
import { useTitle } from 'react-use';

import './nprogress.css';

NProgress.configure({ showSpinner: false });

export const useVdbTitle = (
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
