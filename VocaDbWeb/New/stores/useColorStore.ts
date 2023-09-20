import { MantineColor, MantineColorScheme } from '@mantine/core';
import { setCookie } from 'cookies-next';
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface ColorState {
	primaryColor: MantineColor;
	colorScheme: MantineColorScheme;
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
				set(() => ({ colorScheme: newColorScheme }));
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

