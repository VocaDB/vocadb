import App from '@/App';
import '@/ArrayExtensions';
import { VdbPlayerProvider } from '@/Components/VdbPlayer/VdbPlayerContext';
import { MutedUsersProvider } from '@/MutedUsersContext';
import '@/i18n';
import { NostalgicDivaProvider } from '@vocadb/nostalgic-diva';
import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';

const app = document.getElementById('root');

ReactDOM.render(
	<React.StrictMode>
		<BrowserRouter>
			<NostalgicDivaProvider>
				<VdbPlayerProvider>
					<MutedUsersProvider>
						<App />
					</MutedUsersProvider>
				</VdbPlayerProvider>
			</NostalgicDivaProvider>
		</BrowserRouter>
	</React.StrictMode>,
	app,
);
