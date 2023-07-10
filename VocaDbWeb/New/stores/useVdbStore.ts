import { apiGet, authApiGet } from '@/Helpers/FetchApiHelper';
import { GlobalValues } from '@/types/GlobalValues';
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface VdbState {
	values?: GlobalValues;
	fetchValues(): Promise<void>;
}

export const useVdbStore = create<VdbState>()(
	persist(
		(set) => ({
			fetchValues: async () => {
				authApiGet<GlobalValues>('/api/globals/values')
					.then((values) => set({ values }))
					.catch(() => console.log('Invalid vdb return value'));
			},
		}),
		{
			name: 'vdb-storage',
		}
	)
);
