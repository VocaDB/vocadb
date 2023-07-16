import NextApp, { AppProps, AppContext } from 'next/app';
import { getCookie } from 'cookies-next';
import Head from 'next/head';
import { MantineProvider, ColorScheme, MantineThemeOverride } from '@mantine/core';
import { Notifications } from '@mantine/notifications';
import AppShell from '../components/AppShell/AppShell';
import { GlobalValues } from '@/types/GlobalValues';
import { ModalsProvider } from '@mantine/modals';
import { useColorStore } from '@/stores/useColorStore';
import { colors } from '@/components/colors';
import { useEffect, useState } from 'react';
import { authApiGet } from '@/Helpers/FetchApiHelper';
import { useVdbStore } from '@/stores/useVdbStore';

export default function App(
	props: AppProps & {
		colorScheme: ColorScheme;
		primaryColor: string;
		values: GlobalValues | undefined;
	}
) {
	const {
		Component,
		pageProps,
		colorScheme: cookieColorScheme,
		primaryColor: cookiePrimaryColor,
	} = props;
	const [storedPrimaryColor, storedColorScheme] = useColorStore((state) => [
		state.primaryColor,
		state.colorScheme,
	]);
	const setValues = useVdbStore((set) => set.setValues);

	const [theme, setTheme] = useState<MantineThemeOverride>({
		colors,
		primaryColor: cookiePrimaryColor ? cookiePrimaryColor : 'miku',
		colorScheme: cookieColorScheme ? cookieColorScheme : 'light',
	});

	useEffect(() => {
		if (
			(storedPrimaryColor !== cookiePrimaryColor ||
				storedColorScheme !== cookieColorScheme) &&
			(storedPrimaryColor !== theme.primaryColor || storedColorScheme !== theme.colorScheme)
		) {
			setTheme({
				colors,
				primaryColor: storedPrimaryColor,
				//@ts-ignore
				colorScheme: storedColorScheme,
			});
		}
	}, [storedColorScheme, storedPrimaryColor]);

	useEffect(() => {
		authApiGet<GlobalValues>('/api/globals/values')
			.then((values) => setValues(values))
			.catch((e) => console.log(e));
	}, [setValues]);

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
				<ModalsProvider>
					<AppShell>
						<Component {...pageProps} />
					</AppShell>
				</ModalsProvider>
				<Notifications />
			</MantineProvider>
		</>
	);
}

App.getInitialProps = async (appContext: AppContext) => {
	const appProps = await NextApp.getInitialProps(appContext);

	return {
		...appProps,
		colorScheme: getCookie('mantine-color-scheme', appContext.ctx),
		primaryColor: getCookie('mantine-primary-color', appContext.ctx),
	};
};

