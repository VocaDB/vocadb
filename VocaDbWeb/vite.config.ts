import faroUploader from '@grafana/faro-rollup-plugin';
import react from '@vitejs/plugin-react';
import { resolve } from 'path';
import { defineConfig, loadEnv } from 'vite';

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
	const env = loadEnv(mode, process.cwd());
	const VITE_FARO_API_KEY = `${env.VITE_FARO_API_KEY ?? ''}`;

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
			faroUploader({
				appName: 'vocadb_frontend',
				endpoint: 'https://faro-api-prod-eu-west-2.grafana.net/faro/api/v1',
				appId: 'vocadb_frontend',
				stackId: '1088236',
				// instructions on how to obtain your API key are in the documentation
				// https://grafana.com/docs/grafana-cloud/monitor-applications/frontend-observability/sourcemap-upload-plugins/#obtain-an-api-key
				apiKey: VITE_FARO_API_KEY,
				gzipContents: true,
			}),
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
	};
});
