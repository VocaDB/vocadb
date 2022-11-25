import { GlobalResources } from '@/Shared/GlobalResources';
import { GlobalValues } from '@/Shared/GlobalValues';

// TODO: Remove.
declare global {
	const vdb: {
		resources: GlobalResources;
		values: GlobalValues;
	};
}
