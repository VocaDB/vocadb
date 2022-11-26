import react from '@vitejs/plugin-react';
import { resolve } from 'path';
import { defineConfig } from 'vite';

// https://vitejs.dev/config/
export default defineConfig({
	publicDir: 'wwwroot',
	resolve: {
		alias: {
			'@': resolve(__dirname, './Scripts'),
		},
	},
	plugins: [
		react({
			// https://dev.to/ajitsinghkamal/using-emotionjs-with-vite-2ndj#comment-1nif3
			jsxImportSource: '@emotion/react',
		}),
	],
	server: {
		proxy: {
			'/api': {
				target: 'https://localhost:44398',
				changeOrigin: true,
				// https://stackoverflow.com/questions/74033733/vite-self-signed-certificate-error-when-calling-local-api/74033815#74033815
				secure: false,
			},
		},
	},
});
