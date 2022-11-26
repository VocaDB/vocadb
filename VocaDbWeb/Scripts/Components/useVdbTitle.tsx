import { useVdb } from '@/VdbContext';
import NProgress from 'nprogress';
import { useTitle } from 'react-use';

import './nprogress.css';

NProgress.configure({ showSpinner: false });

export const useVdbTitle = (title: string | undefined): void => {
	const vdb = useVdb();

	useTitle(
		title ? `${title} - ${vdb.values.siteTitle}` : `${vdb.values.siteTitle}`,
	);
};
