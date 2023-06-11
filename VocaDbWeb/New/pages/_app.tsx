import { useEffect, useState } from 'react';
import NextApp, { AppProps, AppContext } from 'next/app';
import { getCookie, hasCookie, setCookie } from 'cookies-next';
import Head from 'next/head';
import {
	MantineProvider,
	ColorScheme,
	ColorSchemeProvider,
	MantineThemeOverride,
} from '@mantine/core';
import { Notifications } from '@mantine/notifications';
import AppShell from '../components/AppShell/AppShell';
import { GlobalValues } from '@/types/GlobalValues';
import { VdbProvider } from '@/components/Context/VdbContext';
import { apiFetch } from '@/Helpers/FetchApiHelper';
import { ThemeProvider } from '@/components/Context/ThemeContext';

export default function App(
	props: AppProps & {
		colorScheme: ColorScheme;
		primaryColor: string;
		values: GlobalValues | undefined;
	}
) {
	const { Component, pageProps, primaryColor, values } = props;
	const [theme, setTheme] = useState<MantineThemeOverride>({
		colors: {
			miku: [
				'#e4fffe',
				'#d0fffb',
				'#a1fef6',
				'#6ffef1',
				'#4dfeed',
				'#3dfeea',
				'#30fee9',
				'#21e2ce',
				'#00c9b7',
				'#00ae9e',
			],
			luka: [
				'#ffe8f8',
				'#ffcfe9',
				'#ff9bce',
				'#ff64b4',
				'#fe389d',
				'#fe1c8e',
				'#ff0987',
				'#e40074',
				'#cc0067',
				'#b30059',
			],
		},
		primaryColor,
	});
	const [colorScheme, setColorScheme] = useState<ColorScheme>(props.colorScheme);

	// TODO: This should be moved elsewhere
	const toggleColorScheme = (value?: ColorScheme) => {
		const nextColorScheme = value || (colorScheme === 'dark' ? 'light' : 'dark');
		setColorScheme(nextColorScheme);
		setCookie('mantine-color-scheme', nextColorScheme, { maxAge: 60 * 60 * 24 * 30 });
	};

	useEffect(() => {
		setCookie('vdb-values', values);
	}, [values]);

	return (
		<>
			<Head>
				<title>VocaDB</title>
				<meta
					name="viewport"
					content="minimum-scale=1, initial-scale=1, width=device-width"
				/>
				<link rel="shortcut icon" href="/new/favicon.ico" />
			</Head>

			<ColorSchemeProvider colorScheme={colorScheme} toggleColorScheme={toggleColorScheme}>
				<MantineProvider
					theme={{ ...theme, colorScheme }}
					withGlobalStyles
					withNormalizeCSS
				>
					<VdbProvider initialValue={values}>
						<ThemeProvider theme={theme} setTheme={setTheme}>
							<AppShell>
								<Component {...pageProps} />
							</AppShell>
						</ThemeProvider>
					</VdbProvider>
					<Notifications />
				</MantineProvider>
			</ColorSchemeProvider>
		</>
	);
}

App.getInitialProps = async (appContext: AppContext) => {
	const appProps = await NextApp.getInitialProps(appContext);
	const isServer = !!appContext.ctx.req;
	let values;

	// TODO: Better check if session is lost
	if (!hasCookie('vdb-values', appContext.ctx) && isServer) {
		const res = await apiFetch('/api/globals/values', appContext.ctx.req);
		values = await res.json();
	}

	return {
		...appProps,
		colorScheme: getCookie('mantine-color-scheme', appContext.ctx) || 'dark',
		primaryColor: getCookie('mantine-primary-color', appContext.ctx) || 'miku',
		values,
	};
};

