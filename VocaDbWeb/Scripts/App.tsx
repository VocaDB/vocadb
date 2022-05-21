import Container from '@Bootstrap/Container';
import AboutDisclaimer from '@Components/Shared/Partials/AboutDisclaimer';
import Header from '@Components/Shared/Partials/Header';
import LeftMenu from '@Components/Shared/Partials/LeftMenu';
import { ScrollToTop } from '@vocadb/route-sphere';
import React from 'react';
import { Toaster } from 'react-hot-toast';

import AppRoutes from './AppRoutes';
import VdbPlayer from './Components/VdbPlayer/VdbPlayer';

const App = (): React.ReactElement => {
	return (
		<>
			<ScrollToTop />

			<Header />

			<Container fluid={true}>
				<div className="row-fluid">
					<LeftMenu />

					<div className="span10 rightFrame well">
						<React.Suspense fallback={null /* TODO */}>
							<AppRoutes />
						</React.Suspense>
					</div>

					<AboutDisclaimer />
				</div>
			</Container>

			<Toaster containerStyle={{ top: '10vh' }} gutter={0} />

			<VdbPlayer />
		</>
	);
};

export default App;
