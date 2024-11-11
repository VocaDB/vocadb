import {
	createReactRouterV6Options,
	getWebInstrumentations,
	initializeFaro,
	ReactIntegration,
} from '@grafana/faro-react';
import { TracingInstrumentation } from '@grafana/faro-web-tracing';
import {
	createRoutesFromChildren,
	matchRoutes,
	Routes,
	useLocation,
	useNavigationType,
} from 'react-router-dom';

initializeFaro({
	url: 'https://faro-collector-prod-eu-west-2.grafana.net/collect/1df9324819a9b9c19f9d79c725c72ed2',
	app: {
		name: 'vocadb_frontend',
		version: '1.0.0',
		environment: 'production',
	},
	sessionTracking: {
		samplingRate: 0.3,
	},
	instrumentations: [
		// Mandatory, omits default instrumentations otherwise.
		...getWebInstrumentations(),

		// Tracing package to get end-to-end visibility for HTTP requests.
		new TracingInstrumentation(),

		// React integration for React applications.
		new ReactIntegration({
			router: createReactRouterV6Options({
				createRoutesFromChildren,
				matchRoutes,
				Routes,
				useLocation,
				useNavigationType,
			}),
		}),
	],
});
