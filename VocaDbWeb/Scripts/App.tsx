import '@/ArrayExtensions';
// TODO: Remove
import { VdbPlayerProvider } from '@/Components/VdbPlayer/VdbPlayerContext';
import { Compose } from '@/Compose';
import { LoginManagerProvider } from '@/LoginManagerContext';
import { MutedUsersProvider } from '@/MutedUsersContext';
import { VdbProvider } from '@/VdbContext';
import '@/i18n';
import { NostalgicDivaProvider } from '@vocadb/nostalgic-diva';
import React, { Suspense } from 'react';
import { BrowserRouter } from 'react-router-dom';

import { CultureCodesProvider } from './CultureCodesContext';

const NewApp = React.lazy(() => import('./NewApp'));
const OldApp = React.lazy(() => import('./OldApp'));

const InnerAppChooser = (): React.ReactElement => {
	const beta_enabled = localStorage.getItem('new_beta');

	if (beta_enabled === 'true') {
		return <NewApp />;
	}

	return <OldApp />;
};

const App = (): React.ReactElement => {
	return (
		<Compose
			components={[
				VdbProvider,
				LoginManagerProvider,
				BrowserRouter,
				NostalgicDivaProvider,
				VdbPlayerProvider,
				MutedUsersProvider,
				CultureCodesProvider,
			]}
		>
			<Suspense fallback={null}>
				<InnerAppChooser />
			</Suspense>
		</Compose>
	);
};

export default App;
