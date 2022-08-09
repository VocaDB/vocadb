import App from '@/App';
import { VdbPlayerProvider } from '@/Components/VdbPlayer/VdbPlayerContext';
import { VdbPlayerStore } from '@/Stores/VdbPlayer/VdbPlayerStore';
import '@/i18n';
import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';

const app = document.getElementById('app');

ReactDOM.render(
	<React.StrictMode>
		<BrowserRouter>
			<VdbPlayerProvider value={new VdbPlayerStore()}>
				<App />
			</VdbPlayerProvider>
		</BrowserRouter>
	</React.StrictMode>,
	app,
);
