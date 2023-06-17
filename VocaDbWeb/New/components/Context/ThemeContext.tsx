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
	const setPrimaryColor = (color: string): any => {
		setTheme({
			...theme,
			primaryColor: color,
			primaryShade: color === 'miku' || color === 'gumi' ? 7 : undefined,
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

