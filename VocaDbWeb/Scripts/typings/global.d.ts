import { GlobalResources } from '@/Shared/GlobalResources';
import { GlobalValues } from '@/Shared/GlobalValues';

declare global {
	const vdb: {
		resources: GlobalResources;
		values: GlobalValues;
	};
}
