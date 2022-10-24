import App from '@/App';
import '@/ArrayExtensions';
import { VdbPlayerProvider } from '@/Components/VdbPlayer/VdbPlayerContext';
import '@/i18n';
import { NostalgicDivaProvider } from '@vocadb/nostalgic-diva';
import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';

const app = document.getElementById('app');

ReactDOM.render(
	<React.StrictMode>
		<BrowserRouter>
			<NostalgicDivaProvider>
				<VdbPlayerProvider>
					<App />
				</VdbPlayerProvider>
			</NostalgicDivaProvider>
		</BrowserRouter>
	</React.StrictMode>,
	app,
);
