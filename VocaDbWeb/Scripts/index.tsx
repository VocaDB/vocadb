import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';

import App from './App';
import { VdbPlayerProvider } from './Components/VdbPlayer/VdbPlayerContext';
import VdbPlayerStore from './Stores/VdbPlayer/VdbPlayerStore';
import './i18n';

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
