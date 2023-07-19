import { GlobalValues } from '@/types/GlobalValues';
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface VdbState {
	values?: GlobalValues;
	setValues(values: GlobalValues): void;
}

export const useVdbStore = create<VdbState>()(
	persist(
		(set) => ({
			setValues: (values) => {
				set({ values });
			},
		}),
		{
			name: 'vdb-storage',
			partialize: (state) => ({ values: state.values }),
		}
	)
);

