import AppRoutes from '@/AppRoutes';
import Container from '@/Bootstrap/Container';
import { AboutDisclaimer } from '@/Components/Shared/Partials/AboutDisclaimer';
import { Header } from '@/Components/Shared/Partials/Header';
import { LeftMenu } from '@/Components/Shared/Partials/LeftMenu';
import { miniPlayerHeight, VdbPlayer } from '@/Components/VdbPlayer/VdbPlayer';
import { useVdb } from '@/VdbContext';
import '@/i18n';
import '@/styles/Icons.css';
import '@/styles/css.less';
import '@/styles/jquery.qtip.css';
import '@/styles/themes/redmond/jquery-ui-1.10.1.custom.min.css';
import { ScrollToTop } from '@vocadb/route-sphere';
import React from 'react';
import { Toaster } from 'react-hot-toast';
import { BrowserRouter } from 'react-router-dom';

const UtaiteDB = React.lazy(() => import('./styles/utaiteDb'));
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
				{vdb.values.siteName.toLowerCase().includes('utaite') && <UtaiteDB />}
			</React.Suspense>
		</Container>
	);
};

const OldApp = (): React.ReactElement => {
	return (
		<BrowserRouter>
			<ScrollToTop />
			<Header />
			<div css={{ display: 'flex' }}>
				<LeftMenu />
				<AppContainer />
			</div>
			<Toaster containerStyle={{ top: '10vh' }} gutter={0} />
			<VdbPlayer />{' '}
		</BrowserRouter>
	);
};

export default OldApp;
