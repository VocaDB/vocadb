import { ColorScheme, MantineColor } from '@mantine/core';
import { setCookie } from 'cookies-next';
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface ColorState {
	primaryColor: MantineColor;
	colorScheme: ColorScheme;
	setPrimaryColor: (color: MantineColor) => void;
	toggleColorScheme: () => void;
}

export const useColorStore = create<ColorState>()(
	persist(
		(set, get) => ({
			primaryColor: 'miku',
			colorScheme: 'light',
			setPrimaryColor: (color) => {
				setCookie('mantine-primary-color', color);
				set({ primaryColor: color });
			},
			toggleColorScheme() {
				const newColorScheme = get().colorScheme === 'light' ? 'dark' : 'light';
				setCookie('mantine-color-scheme', newColorScheme);
				set((state) => ({ colorScheme: newColorScheme }));
			},
		}),
		{
			name: 'color-storage',
			partialize: (state) => ({
				primaryColor: state.primaryColor,
				colorScheme: state.colorScheme,
			}),
		}
	)
);

