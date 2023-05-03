import React from 'react';
import { Route, Routes } from 'react-router-dom';

const ActivityEntryRoutes = React.lazy(
	() => import('@/Pages/ActivityEntry/ActivityEntryRoutes'),
);
const AdminRoutes = React.lazy(() => import('@/Pages/Admin/AdminRoutes'));
const AlbumRoutes = React.lazy(() => import('@/Pages/Album/AlbumRoutes'));
const ArtistRoutes = React.lazy(() => import('@/Pages/Artist/ArtistRoutes'));
const CommentRoutes = React.lazy(() => import('@/Pages/Comment/CommentRoutes'));
const DiscussionRoutes = React.lazy(
	() => import('@/Pages/Discussion/DiscussionRoutes'),
);
const ErrorRoutes = React.lazy(() => import('@/Pages/Error/ErrorRoutes'));
const EventRoutes = React.lazy(() => import('@/Pages/Event/EventRoutes'));
const HelpRoutes = React.lazy(() => import('@/Pages/Help/HelpRoutes'));
const HomeRoutes = React.lazy(() => import('@/Pages/Home/HomeRoutes'));
const PlaylistRoutes = React.lazy(
	() => import('@/Pages/Playlist/PlaylistRoutes'),
);
const SearchRoutes = React.lazy(() => import('@/Pages/Search/SearchRoutes'));
const SongListRoutes = React.lazy(
	() => import('@/Pages/SongList/SongListRoutes'),
);
const SongRoutes = React.lazy(() => import('@/Pages/Song/SongRoutes'));
const StatsRoutes = React.lazy(() => import('@/Pages/Stats/StatsRoutes'));
const TagRoutes = React.lazy(() => import('@/Pages/Tag/TagRoutes'));
const UserRoutes = React.lazy(() => import('@/Pages/User/UserRoutes'));
const VenueRoutes = React.lazy(() => import('@/Pages/Venue/VenueRoutes'));

const AlbumDetails = React.lazy(() => import('@/Pages/Album/AlbumDetails'));
const ArtistDetails = React.lazy(() => import('@/Pages/Artist/ArtistDetails'));
const EventDetails = React.lazy(() => import('@/Pages/Event/EventDetails'));
const EventSeriesDetails = React.lazy(
	() => import('@/Pages/Event/EventSeriesDetails'),
);
const SongDetails = React.lazy(() => import('@/Pages/Song/SongDetails'));
const SongListDetails = React.lazy(
	() => import('@/Pages/SongList/SongListDetails'),
);
const TagDetails = React.lazy(() => import('@/Pages/Tag/TagDetails'));
const UserDetails = React.lazy(() => import('@/Pages/User/UserDetails'));

const AppRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="ActivityEntry/*" element={<ActivityEntryRoutes />} />
			<Route path="Admin/*" element={<AdminRoutes />} />
			<Route path="Album/*" element={<AlbumRoutes />} />
			<Route path="Artist/*" element={<ArtistRoutes />} />
			<Route path="Comment/*" element={<CommentRoutes />} />
			<Route path="discussion/*" element={<DiscussionRoutes />} />
			<Route path="Error" element={<ErrorRoutes />} />
			<Route path="Event/*" element={<EventRoutes />} />
			<Route path="Help/*" element={<HelpRoutes />} />
			<Route path="playlist/*" element={<PlaylistRoutes />} />
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
	);
};

export default AppRoutes;
