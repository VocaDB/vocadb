import { ColorScheme, MantineColor } from '@mantine/core';
import { create } from 'zustand';

interface ColorState {
	primaryColor: MantineColor;
	colorScheme: ColorScheme;
	setPrimaryColor: (color: MantineColor) => void;
	toggleColorScheme: () => void;
}

export const useColorStore = create<ColorState>((set) => ({
	primaryColor: 'miku',
	colorScheme: 'light',
	setPrimaryColor: (color) => set({ primaryColor: color }),
	toggleColorScheme: () =>
		set((state) => ({ colorScheme: state.colorScheme === 'light' ? 'dark' : 'light' })),
}));

