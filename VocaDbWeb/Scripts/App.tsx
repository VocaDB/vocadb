import Container from '@Bootstrap/Container';
import Navbar from '@Bootstrap/Navbar';
import ErrorNotFound from '@Components/Error/ErrorNotFound';
import GlobalSearchBox from '@Components/Shared/GlobalSearchBox';
import Footer from '@Components/Shared/Partials/Footer';
import LeftMenu from '@Components/Shared/Partials/LeftMenu';
import LoginManager from '@Models/LoginManager';
import EntryReportRepository from '@Repositories/EntryReportRepository';
import UserRepository from '@Repositories/UserRepository';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import TopBarStore from '@Stores/TopBarStore';
import React from 'react';
import ReactGA from 'react-ga';
import { BrowserRouter, Route, Routes } from 'react-router-dom';

import './i18n';

const AdminRoutes = React.lazy(() => import('@Components/Admin/AdminRoutes'));
const DiscussionRoutes = React.lazy(
	() => import('@Components/Discussion/DiscussionRoutes'),
);
const SearchRoutes = React.lazy(
	() => import('@Components/Search/SearchRoutes'),
);
const SongListRoutes = React.lazy(
	() => import('@Components/SongList/SongListRoutes'),
);
const StatsRoutes = React.lazy(() => import('@Components/Stats/StatsRoutes'));
const UserRoutes = React.lazy(() => import('@Components/User/UserRoutes'));

const loginManager = new LoginManager(vdb.values);

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);
const entryReportRepo = new EntryReportRepository(httpClient, urlMapper);
const userRepo = new UserRepository(httpClient, urlMapper);

const topBarStore = new TopBarStore(loginManager, entryReportRepo, userRepo);

const App = (): React.ReactElement => {
	React.useEffect(() => {
		ReactGA.initialize(vdb.values.gaAccountId);
	}, []);

	return (
		<BrowserRouter>
			<Navbar className="navbar-inverse" fixed="top">
				<Container id="topBar">
					<GlobalSearchBox topBarStore={topBarStore} />
				</Container>
			</Navbar>

			<Container fluid={true}>
				<div className="row-fluid">
					<LeftMenu />

					<div className="span10 rightFrame well">
						<React.Suspense fallback={null /* TODO */}>
							<Routes>
								<Route path="Admin/*" element={<AdminRoutes />} />
								<Route path="discussion/*" element={<DiscussionRoutes />} />
								<Route path="Search/*" element={<SearchRoutes />} />
								<Route path="SongList/*" element={<SongListRoutes />} />
								<Route path="Stats/*" element={<StatsRoutes />} />
								<Route path="User/*" element={<UserRoutes />} />
								<Route path="*" element={<ErrorNotFound />} />
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
