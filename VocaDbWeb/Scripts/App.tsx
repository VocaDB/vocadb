import Container from '@Bootstrap/Container';
import Footer from '@Components/Shared/Partials/Footer';
import Header from '@Components/Shared/Partials/Header';
import LeftMenu from '@Components/Shared/Partials/LeftMenu';
import React from 'react';
import ReactGA from 'react-ga';
import { Toaster } from 'react-hot-toast';
import { Route, Routes } from 'react-router-dom';

import ScrollToTop from './ScrollToTop';

const ActivityEntryRoutes = React.lazy(
	() => import('@Components/ActivityEntry/ActivityEntryRoutes'),
);
const AdminRoutes = React.lazy(() => import('@Components/Admin/AdminRoutes'));
const AlbumRoutes = React.lazy(() => import('@Components/Album/AlbumRoutes'));
const ArtistRoutes = React.lazy(
	() => import('@Components/Artist/ArtistRoutes'),
);
const DiscussionRoutes = React.lazy(
	() => import('@Components/Discussion/DiscussionRoutes'),
);
const EventRoutes = React.lazy(() => import('@Components/Event/EventRoutes'));
const HomeRoutes = React.lazy(() => import('@Components/Home/HomeRoutes'));
const SearchRoutes = React.lazy(
	() => import('@Components/Search/SearchRoutes'),
);
const SongListRoutes = React.lazy(
	() => import('@Components/SongList/SongListRoutes'),
);
const SongRoutes = React.lazy(() => import('@Components/Song/SongRoutes'));
const StatsRoutes = React.lazy(() => import('@Components/Stats/StatsRoutes'));
const TagRoutes = React.lazy(() => import('@Components/Tag/TagRoutes'));
const UserRoutes = React.lazy(() => import('@Components/User/UserRoutes'));
const VenueRoutes = React.lazy(() => import('@Components/Venue/VenueRoutes'));

const AlbumDetails = React.lazy(() => import('@Components/Album/AlbumDetails'));
const ArtistDetails = React.lazy(
	() => import('@Components/Artist/ArtistDetails'),
);
const EventDetails = React.lazy(() => import('@Components/Event/EventDetails'));
const EventSeriesDetails = React.lazy(
	() => import('@Components/Event/EventSeriesDetails'),
);
const SongDetails = React.lazy(() => import('@Components/Song/SongDetails'));
const SongListDetails = React.lazy(
	() => import('@Components/SongList/SongListDetails'),
);
const TagDetails = React.lazy(() => import('@Components/Tag/TagDetails'));
const UserDetails = React.lazy(() => import('@Components/User/UserDetails'));

const App = (): React.ReactElement => {
	React.useEffect(() => {
		ReactGA.initialize(vdb.values.gaAccountId);
	}, []);

	return (
		<>
			<ScrollToTop />

			<Header />

			<Container fluid={true}>
				<div className="row-fluid">
					<LeftMenu />

					<div className="span10 rightFrame well">
						<React.Suspense fallback={null /* TODO */}>
							<Routes>
								<Route
									path="ActivityEntry/*"
									element={<ActivityEntryRoutes />}
								/>
								<Route path="Admin/*" element={<AdminRoutes />} />
								<Route path="Album/*" element={<AlbumRoutes />} />
								<Route path="Artist/*" element={<ArtistRoutes />} />
								<Route path="discussion/*" element={<DiscussionRoutes />} />
								<Route path="Event/*" element={<EventRoutes />} />
								<Route path="Search/*" element={<SearchRoutes />} />
								<Route path="SongList/*" element={<SongListRoutes />} />
								<Route path="Song/*" element={<SongRoutes />} />
								<Route path="Stats/*" element={<StatsRoutes />} />
								<Route path="Tag/*" element={<TagRoutes />} />
								<Route path="User/*" element={<UserRoutes />} />
								<Route path="Venue/*" element={<VenueRoutes />} />

								<Route path="Al/:id/*" element={<AlbumDetails />} />
								<Route path="Ar/:id/*" element={<ArtistDetails />} />
								<Route path="E/:id/*" element={<EventDetails />} />
								<Route path="Es/:id/*" element={<EventSeriesDetails />} />
								<Route path="L/:id/*" element={<SongListDetails />} />
								<Route path="S/:id/*" element={<SongDetails />} />
								<Route path="T/:id/*" element={<TagDetails />} />
								<Route path="Profile/:name/*" element={<UserDetails />} />

								<Route path="*" element={<HomeRoutes />} />
							</Routes>
						</React.Suspense>
					</div>
				</div>
			</Container>

			<Footer />

			<Toaster containerStyle={{ top: '10vh' }} gutter={0} />
		</>
	);
};

export default App;
