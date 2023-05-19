import AppRoutes from '@/AppRoutes';
import '@/ArrayExtensions';
import Container from '@/Bootstrap/Container';
import { AboutDisclaimer } from '@/Components/Shared/Partials/AboutDisclaimer';
import { Header } from '@/Components/Shared/Partials/Header';
import { LeftMenu } from '@/Components/Shared/Partials/LeftMenu';
import { miniPlayerHeight, VdbPlayer } from '@/Components/VdbPlayer/VdbPlayer';
import { VdbPlayerProvider } from '@/Components/VdbPlayer/VdbPlayerContext';
import { Compose } from '@/Compose';
import { LoginManagerProvider } from '@/LoginManagerContext';
import { MutedUsersProvider } from '@/MutedUsersContext';
import { VdbProvider, useVdb } from '@/VdbContext';
import '@/i18n';
import '@/styles/css.less';
import { NostalgicDivaProvider } from '@vocadb/nostalgic-diva';
import { ScrollToTop } from '@vocadb/route-sphere';
import React from 'react';
import { Toaster } from 'react-hot-toast';
import { BrowserRouter } from 'react-router-dom';

const TetoDB = React.lazy(() => import('./styles/tetoDb'));
const DarkAngel = React.lazy(() => import('./styles/darkAngel'));

const AppContainer = (): React.ReactElement => {
	const vdb = useVdb();

	return (
		<Container
			fluid
			css={{
				flex: '1 1 100%',
				paddingBottom: miniPlayerHeight,
				minWidth: 0,
				overflow: 'hidden',
			}}
		>
			<div className="row-fluid">
				<div className="span12 rightFrame well">
					<React.Suspense fallback={null /* TODO */}>
						<AppRoutes />
					</React.Suspense>
				</div>
				<AboutDisclaimer />
			</div>
			<React.Suspense fallback={null}>
				{vdb.values.loggedUser?.stylesheet && (
					<>
						{vdb.values.loggedUser?.stylesheet
							.toLowerCase()
							.startsWith('darkangel') && <DarkAngel />}

						{vdb.values.loggedUser?.stylesheet
							.toLowerCase()
							.startsWith('tetodb') && <TetoDB />}
					</>
				)}
			</React.Suspense>
		</Container>
	);
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
			]}
		>
			<ScrollToTop />
			<Header />
			<div css={{ display: 'flex' }}>
				<LeftMenu />
				<AppContainer />
			</div>
			<Toaster containerStyle={{ top: '10vh' }} gutter={0} />
			<VdbPlayer />
		</Compose>
	);
};

export default App;
