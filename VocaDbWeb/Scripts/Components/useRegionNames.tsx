import { useVdb } from '@/VdbContext';
import React from 'react';

export const useRegionNames = (): Intl.DisplayNames => {
	const vdb = useVdb();
	const regionNames = React.useMemo(
		() =>
			new Intl.DisplayNames([vdb.values.uiCulture], {
				type: 'region',
			}),
		[vdb],
	);
	return regionNames;
};
