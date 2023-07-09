import { useEffect } from 'react';
import NextApp, { AppProps, AppContext } from 'next/app';
import { getCookie, setCookie } from 'cookies-next';
import Head from 'next/head';
import { MantineProvider, ColorScheme, MantineThemeOverride } from '@mantine/core';
import { Notifications } from '@mantine/notifications';
import AppShell from '../components/AppShell/AppShell';
import { GlobalValues } from '@/types/GlobalValues';
import { VdbProvider } from '@/components/Context/VdbContext';
import { ModalsProvider } from '@mantine/modals';
import { useColorStore } from '@/stores/useColorStore';
import { colors } from '@/components/colors';

export default function App(
	props: AppProps & {
		colorScheme: ColorScheme;
		primaryColor: string;
		values: GlobalValues | undefined;
	}
) {
	const { Component, pageProps, values } = props;
	const [primaryColor, colorScheme] = useColorStore((state) => [
		state.primaryColor,
		state.colorScheme,
	]);
	const theme: MantineThemeOverride = {
		colors,
		primaryColor,
		colorScheme,
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
				<link rel="preconnect" href="https://vocadb.net" />
				<link rel="preconnect" href="https://wsrv.nl" />
			</Head>

			<MantineProvider theme={theme} withGlobalStyles withNormalizeCSS>
				<VdbProvider initialValue={values}>
					<ModalsProvider>
						<AppShell>
							<Component {...pageProps} />
						</AppShell>
					</ModalsProvider>
				</VdbProvider>
				<Notifications />
			</MantineProvider>
		</>
	);
}

App.getInitialProps = async (appContext: AppContext) => {
	const appProps = await NextApp.getInitialProps(appContext);

	return {
		...appProps,
		colorScheme: getCookie('mantine-color-scheme', appContext.ctx) || 'light',
		primaryColor: getCookie('mantine-primary-color', appContext.ctx) || 'miku',
	};
};

