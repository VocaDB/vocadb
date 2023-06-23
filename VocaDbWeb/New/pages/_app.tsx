import { useEffect } from 'react';
import NextApp, { AppProps, AppContext } from 'next/app';
import { getCookie, hasCookie, setCookie } from 'cookies-next';
import Head from 'next/head';
import { MantineProvider, ColorScheme, MantineThemeOverride } from '@mantine/core';
import { Notifications } from '@mantine/notifications';
import AppShell from '../components/AppShell/AppShell';
import { GlobalValues } from '@/types/GlobalValues';
import { VdbProvider } from '@/components/Context/VdbContext';
import { apiFetch } from '@/Helpers/FetchApiHelper';
import { ModalsProvider } from '@mantine/modals';
import { useColorStore } from '@/stores/color';

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
		colors: {
			miku: [
				'#dbfeff',
				'#b4f5f9',
				'#8aedf3',
				'#5fe6ed',
				'#38dfe7',
				'#23c5cd',
				'#1299a0',
				'#036e72',
				'#004346',
				'#00181a',
			],
			luka: [
				'#F1E7EC',
				'#E4C9D8',
				'#DCAAC5',
				'#CA95B2',
				'#B884A0',
				'#A67690',
				'#966A82',
				'#836374',
				'#735C69',
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
			una: [
				'#f5ffdc',
				'#e7feaf',
				'#d8fc7f',
				'#cafb4e',
				'#bbfa1f',
				'#a1e005',
				'#7caf00',
				'#597d00',
				'#344b00',
				'#101a00',
			],
		},
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

