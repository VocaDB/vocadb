import react from '@vitejs/plugin-react';
import { resolve } from 'path';
import { defineConfig } from 'vite';

// https://vitejs.dev/config/
export default defineConfig(() => {
	const betaMode = process.env.mode === 'beta';
	console.log('Using https://beta.vocadb.net/ :', betaMode);

	return {
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
		],
		server: {
			proxy: {
				'/api': {
					target: betaMode ? 'https://beta.vocadb.net' : 'https://vocadb.net',
					changeOrigin: true,
					// https://stackoverflow.com/questions/74033733/vite-self-signed-certificate-error-when-calling-local-api/74033815#74033815
					secure: false,
				},
				'^/stats/.*': {
					target: betaMode
						? 'https://beta.vocadb.net'
						: 'https://localhost:5001',
					changeOrigin: true,
					// https://stackoverflow.com/questions/74033733/vite-self-signed-certificate-error-when-calling-local-api/74033815#74033815
					secure: false,
				},
			},
		},
	};
});
