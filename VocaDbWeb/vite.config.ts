import react from '@vitejs/plugin-react';
import { resolve } from 'path';
import { visualizer } from 'rollup-plugin-visualizer';
import { PluginOption, defineConfig } from 'vite';

// https://vitejs.dev/config/
export default defineConfig({
	build: {
		outDir: 'wwwroot',
	},
	publicDir: 'public',
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
		visualizer({
			template: 'treemap',
			open: true,
			filename: 'analyse.html',
		}) as PluginOption,
	],
	server: {
		proxy: {
			'/api': {
				target: 'https://localhost:5001',
				changeOrigin: true,
				// https://stackoverflow.com/questions/74033733/vite-self-signed-certificate-error-when-calling-local-api/74033815#74033815
				secure: false,
			},
			'^/stats/.*': {
				target: 'https://localhost:5001',
				changeOrigin: true,
				// https://stackoverflow.com/questions/74033733/vite-self-signed-certificate-error-when-calling-local-api/74033815#74033815
				secure: false,
			},
		},
	},
});
