import { useDarkMode } from 'storybook-dark-mode';
import { MantineProvider, ColorSchemeProvider } from '@mantine/core';
import { Notifications } from '@mantine/notifications';
import React from 'react';

export const parameters = { layout: 'fullscreen' };

function ThemeWrapper(props: { children: React.ReactNode }) {
	return (
		<ColorSchemeProvider colorScheme="light" toggleColorScheme={() => {}}>
			<MantineProvider
				theme={{ colorScheme: useDarkMode() ? 'dark' : 'light' }}
				withGlobalStyles
				withNormalizeCSS
			>
				{props.children}
				<Notifications />
			</MantineProvider>
		</ColorSchemeProvider>
	);
}

export const decorators = [(renderStory: Function) => <ThemeWrapper>{renderStory()}</ThemeWrapper>];

// https://github.com/vercel/next.js/issues/18393#issuecomment-955577890
import * as NextImage from 'next/image';

const OriginalNextImage = NextImage.default;

// eslint-disable-next-line no-import-assign
Object.defineProperty(NextImage, 'default', {
	configurable: true,
	value: (/** @type {import('next/image').ImageProps} */ props) => {
		if (typeof props.src === 'string') {
			return <OriginalNextImage {...props} unoptimized blurDataURL={props.src} />;
		} else {
			// don't need blurDataURL here since it is already defined on the StaticImport type
			return <OriginalNextImage {...props} unoptimized />;
		}
	},
});

