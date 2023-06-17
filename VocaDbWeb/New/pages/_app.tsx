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
import { ModalsProvider } from '@mantine/modals';

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
				'#edfcfc',
				'#ddf5f7',
				'#b5edef',
				'#8ce4e8',
				'#6ddbe1',
				'#5cd6dd',
				'#51d5dc',
				'#43bcc2',
				'#34a7ad',
				'#179197',
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
			gumi: [
				'#eeffe7',
				'#dffbd3',
				'#bef5a6',
				'#9bef76',
				'#7dea4e',
				'#6ae734',
				'#5fe525',
				'#4fcb18',
				'#43b40f',
				'#339c00',
			],
			solaria: [
				'#fff5e3',
				'#faead2',
				'#f0d2a8',
				'#e7ba7c',
				'#dfa556',
				'#da973d',
				'#d8912e',
				'#c07d20',
				'#ab6e19',
				'#955f0c',
			],
		},
		primaryColor,
		primaryShade: primaryColor === 'miku' || primaryColor === 'gumi' ? 7 : undefined,
	});
	const [colorScheme, setColorScheme] = useState<ColorScheme>(props.colorScheme);

	// TODO: This should be moved elsewhere
	const toggleColorScheme = (value?: ColorScheme) => {
		const nextColorScheme = value || (colorScheme === 'dark' ? 'light' : 'dark');
		setColorScheme(nextColorScheme);
		setCookie('mantine-color-scheme', nextColorScheme, { maxAge: 60 * 60 * 24 * 30 });
	};

	useEffect(() => {
		setCookie('vdb-values', values?.loggedUser);
	}, [values]);

	return (
		<>
			<Head>
				{/* TODO: Make this dynamic */}
				<title>VocaDB</title>
				<meta
					name="viewport"
					content="minimum-scale=1, initial-scale=1, width=device-width"
				/>
				<meta
					name="description"
					content="VocaDB is a Vocaloid music database with translated artists, albums and songs."
				/>
				<link rel="shortcut icon" href="/favicon.ico" />
			</Head>

			<ColorSchemeProvider colorScheme={colorScheme} toggleColorScheme={toggleColorScheme}>
				<MantineProvider
					theme={{ ...theme, colorScheme }}
					withGlobalStyles
					withNormalizeCSS
				>
					<VdbProvider initialValue={values}>
						<ThemeProvider theme={theme} setTheme={setTheme}>
							<ModalsProvider>
								<AppShell>
									<Component {...pageProps} />
								</AppShell>
							</ModalsProvider>
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
	let values;

	// TODO: Better check if session is lost
	if (!hasCookie('vdb-values', appContext.ctx)) {
		const res = await apiFetch('/api/globals/values', appContext.ctx.req);
		values = await res.json();
	}

	return {
		...appProps,
		colorScheme: getCookie('mantine-color-scheme', appContext.ctx) || 'light',
		primaryColor: getCookie('mantine-primary-color', appContext.ctx) || 'miku',
		values,
	};
};

