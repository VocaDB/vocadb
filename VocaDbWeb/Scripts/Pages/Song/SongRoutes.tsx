import { EntryType } from '@/Models/EntryType';
import ErrorNotFound from '@/Pages/Error/ErrorNotFound';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { SearchType } from '@/Stores/Search/SearchStore';
import qs from 'qs';
import React from 'react';
import { Navigate, Route, Routes, useParams } from 'react-router-dom';

const SongCreate = React.lazy(() => import('./SongCreate'));
const SongEdit = React.lazy(() => import('./SongEdit'));
const SongManageTagUsages = React.lazy(() => import('./SongManageTagUsages'));
const SongMerge = React.lazy(() => import('./SongMerge'));
const SongRankings = React.lazy(() => import('./SongRankings'));
const SongVersions = React.lazy(() => import('./SongVersions'));
const SongViewVersion = React.lazy(() => import('./SongViewVersion'));

const SongDetailsNavigate = (): React.ReactElement => {
	const { id } = useParams();

	return (
		<Navigate
			to={EntryUrlMapper.details(EntryType.Song, Number(id))}
			replace={true}
		/>
	);
};

const SongRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route
				path=""
				element={
					<Navigate
						to={`/Search?${qs.stringify({ searchType: SearchType.Song })}`}
						replace={true}
					/>
				}
			/>
			<Route path="Create" element={<SongCreate />} />
			<Route path="Details/:id" element={<SongDetailsNavigate />} />
			<Route path="Edit/:id" element={<SongEdit />} />
			<Route path="ManageTagUsages/:id" element={<SongManageTagUsages />} />
			<Route path="Merge/:id" element={<SongMerge />} />
			<Route path="Rankings" element={<SongRankings />} />
			<Route path="Versions/:id" element={<SongVersions />} />
			<Route path="ViewVersion/:id" element={<SongViewVersion />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default SongRoutes;
