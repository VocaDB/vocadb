import { useEffect, useState } from 'react';
import NextApp, { AppProps, AppContext } from 'next/app';
import { getCookie, hasCookie, setCookie } from 'cookies-next';
import Head from 'next/head';
import { MantineProvider, ColorScheme, ColorSchemeProvider } from '@mantine/core';
import { Notifications } from '@mantine/notifications';
import AppShell from '../components/AppShell/AppShell';
import { GlobalValues } from '@/types/GlobalValues';
import { VdbProvider } from '@/components/Context/VdbContext';
import { apiFetch } from '@/Helpers/FetchApiHelper';

export default function App(
	props: AppProps & { colorScheme: ColorScheme; values: GlobalValues | undefined }
) {
	const { Component, pageProps, values } = props;
	const [colorScheme, setColorScheme] = useState<ColorScheme>(props.colorScheme);

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
				<MantineProvider theme={{ colorScheme }} withGlobalStyles withNormalizeCSS>
					<VdbProvider initialValue={values}>
						<AppShell>
							<Component {...pageProps} />
						</AppShell>
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
		values,
	};
};

