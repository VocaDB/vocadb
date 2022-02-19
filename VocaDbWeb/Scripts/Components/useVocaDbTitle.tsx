import { useTitle } from 'react-use';

import usePageTracking from './usePageTracking';

const useVocaDbTitle = (title: string | undefined, ready: boolean): void => {
	useTitle(
		title ? `${title} - ${vdb.values.siteTitle}` : `${vdb.values.siteTitle}`,
	);

	usePageTracking(ready);
};

export default useVocaDbTitle;
