'use client';

import { MantineThemeOverride } from '@mantine/core';
import { setCookie } from 'cookies-next';
import React, { useEffect } from 'react';

interface ThemeContext {
	theme: MantineThemeOverride;
	setTheme: React.Dispatch<React.SetStateAction<MantineThemeOverride>>;
	setPrimaryColor: (color: string) => void;
}

const ThemeContext = React.createContext<ThemeContext>(undefined!);

interface ThemeProviderProps {
	theme: MantineThemeOverride;
	setTheme: React.Dispatch<React.SetStateAction<MantineThemeOverride>>;
	children?: React.ReactNode;
}

export const ThemeProvider = ({ children, theme, setTheme }: ThemeProviderProps) => {
	const calculatePrimaryShade = (
		color: string
	): 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | undefined => {
		if (theme.colorScheme === 'dark') {
			return undefined;
		}

		if (color === 'miku' || color === 'gumi') {
			return 8;
		}

		if (color === 'solaria') {
			return 7;
		}

		return undefined;
	};

	const setPrimaryColor = (color: string): any => {
		setTheme({
			...theme,
			primaryColor: color,
			primaryShade: calculatePrimaryShade(color),
		});
	};

	useEffect(() => {
		setCookie('mantine-primary-color', theme.primaryColor, { maxAge: 60 * 60 * 24 * 30 });
	}, [theme.primaryColor]);

	return (
		<ThemeContext.Provider value={{ theme, setTheme, setPrimaryColor }}>
			{children}
		</ThemeContext.Provider>
	);
};

export const useThemeOverride = (): ThemeContext => {
	return React.useContext(ThemeContext);
};

