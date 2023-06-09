import '@/ArrayExtensions';
// TODO: Remove
import { VdbPlayerProvider } from '@/Components/VdbPlayer/VdbPlayerContext';
import { Compose } from '@/Compose';
import { LoginManagerProvider } from '@/LoginManagerContext';
import { MutedUsersProvider } from '@/MutedUsersContext';
import { VdbProvider } from '@/VdbContext';
import '@/i18n';
import { NostalgicDivaProvider } from '@/nostalgic-diva';
import React, { Suspense } from 'react';

import { CultureCodesProvider } from './CultureCodesContext';
import OldApp from './OldApp';

const App = (): React.ReactElement => {
	return (
		<Compose
			components={[
				VdbProvider,
				LoginManagerProvider,
				NostalgicDivaProvider,
				VdbPlayerProvider,
				MutedUsersProvider,
				CultureCodesProvider,
			]}
		>
			<Suspense fallback={null}>
				<OldApp />
			</Suspense>
		</Compose>
	);
};

export default App;
