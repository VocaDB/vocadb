import Container from '@Bootstrap/Container';
import Navbar from '@Bootstrap/Navbar';
import GlobalSearchBox from '@Components/Shared/GlobalSearchBox';
import Footer from '@Components/Shared/Partials/Footer';
import LeftMenu from '@Components/Shared/Partials/LeftMenu';
import React from 'react';
import { BrowserRouter, Route, Routes } from 'react-router-dom';

import './i18n';

const ErrorNotFound = React.lazy(
	() => import('@Components/Error/ErrorNotFound'),
);

const App = (): React.ReactElement => {
	return (
		<BrowserRouter>
			<Navbar className="navbar-inverse" fixed="top">
				<Container id="topBar">
					<GlobalSearchBox /* TODO */ />
				</Container>
			</Navbar>

			<Container fluid={true}>
				<div className="row-fluid">
					<LeftMenu />

					<div className="span10 rightFrame well">
						<React.Suspense fallback={null /* TODO */}>
							<Routes>
								<Route path="/*" element={<ErrorNotFound />} />
							</Routes>
						</React.Suspense>
					</div>
				</div>
			</Container>

			<Footer />
		</BrowserRouter>
	);
};

export default App;
