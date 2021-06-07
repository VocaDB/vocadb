import Container from '@Bootstrap/Container';
import Navbar from '@Bootstrap/Navbar';
import InversifyContext from '@Components/InversifyContext';
import GlobalSearchBox from '@Components/Shared/GlobalSearchBox';
import Footer from '@Components/Shared/Partials/Footer';
import LeftMenu from '@Components/Shared/Partials/LeftMenu';
import VocaDbPageContext, { VocaDbPage } from '@Components/VocaDbPageContext';
import { container } from '@Shared/inversify.config';
import React from 'react';
import { BrowserRouter, Route, Routes } from 'react-router-dom';

import './i18n';

const ErrorNotFound = React.lazy(
	() => import('@Components/Error/ErrorNotFound'),
);
const SearchIndex = React.lazy(() => import('@Components/Search/SearchIndex'));

interface AppProps {
	initialPage: VocaDbPage;
}

const App = ({ initialPage }: AppProps): React.ReactElement => {
	return (
		<VocaDbPageContext.Provider value={initialPage}>
			<InversifyContext.Provider value={{ container: container }}>
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
										<Route path="/Search" element={<SearchIndex />} />
										<Route path="/*" element={<ErrorNotFound />} />
									</Routes>
								</React.Suspense>
							</div>
						</div>
					</Container>

					<Footer />
				</BrowserRouter>
			</InversifyContext.Provider>
		</VocaDbPageContext.Provider>
	);
};

export default App;
