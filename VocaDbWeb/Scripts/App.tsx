import AboutDisclaimer from '@Components/Shared/Partials/AboutDisclaimer';
import Header from '@Components/Shared/Partials/Header';
import LeftMenu from '@Components/Shared/Partials/LeftMenu';
import { ScrollToTop } from '@vocadb/route-sphere';
import React from 'react';
import { Toaster } from 'react-hot-toast';

import AppRoutes from './AppRoutes';

const App = (): React.ReactElement => {
	return (
		<>
			<ScrollToTop />

			<Header />

			<div>
				<LeftMenu />

				<div className="rightFrame well">
					<React.Suspense fallback={null /* TODO */}>
						<AppRoutes />
					</React.Suspense>
				</div>
			</div>

			<AboutDisclaimer />

			<Toaster containerStyle={{ top: '10vh' }} gutter={0} />
		</>
	);
};

export default App;
