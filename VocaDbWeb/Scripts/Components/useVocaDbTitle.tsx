import { usePageTracking } from '@/Components/usePageTracking';
import { useTitle } from 'react-use';

export const useVocaDbTitle = (
	title: string | undefined,
	ready: boolean,
): void => {
	useTitle(
		title ? `${title} - ${vdb.values.siteTitle}` : `${vdb.values.siteTitle}`,
	);

	usePageTracking(ready);
};
