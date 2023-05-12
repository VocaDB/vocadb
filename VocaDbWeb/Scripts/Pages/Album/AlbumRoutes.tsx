import { EntryType } from '@/Models/EntryType';
import ErrorNotFound from '@/Pages/Error/ErrorNotFound';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { SearchType } from '@/Stores/Search/SearchStore';
import qs from 'qs';
import React from 'react';
import { Navigate, Route, Routes, useParams } from 'react-router-dom';

const AlbumCreate = React.lazy(() => import('./AlbumCreate'));
const AlbumDeleted = React.lazy(() => import('./AlbumDeleted'));
const AlbumEdit = React.lazy(() => import('./AlbumEdit'));
const AlbumManageTagUsages = React.lazy(() => import('./AlbumManageTagUsages'));
const AlbumMerge = React.lazy(() => import('./AlbumMerge'));
const AlbumVersions = React.lazy(() => import('./AlbumVersions'));
const AlbumViewVersion = React.lazy(() => import('./AlbumViewVersion'));

const AlbumDetailsNavigate = (): React.ReactElement => {
	const { id } = useParams();

	return (
		<Navigate
			to={EntryUrlMapper.details(EntryType.Album, Number(id))}
			replace={true}
		/>
	);
};

const AlbumRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route
				path=""
				element={
					<Navigate
						to={`/Search?${qs.stringify({ searchType: SearchType.Album })}`}
						replace={true}
					/>
				}
			/>
			<Route path="Create" element={<AlbumCreate />} />
			<Route path="Deleted" element={<AlbumDeleted />} />
			<Route path="Details/:id/*" element={<AlbumDetailsNavigate />} />
			<Route path="Edit/:id" element={<AlbumEdit />} />
			<Route path="ManageTagUsages/:id" element={<AlbumManageTagUsages />} />
			<Route path="Merge/:id" element={<AlbumMerge />} />
			<Route path="Versions/:id" element={<AlbumVersions />} />
			<Route path="ViewVersion/:id" element={<AlbumViewVersion />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default AlbumRoutes;
