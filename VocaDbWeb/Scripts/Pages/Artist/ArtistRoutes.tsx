import { EntryType } from '@/Models/EntryType';
import ErrorNotFound from '@/Pages/Error/ErrorNotFound';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { SearchType } from '@/Stores/Search/SearchStore';
import qs from 'qs';
import React from 'react';
import { Navigate, Route, Routes, useParams } from 'react-router-dom';

const ArtistCreate = React.lazy(() => import('./ArtistCreate'));
const ArtistEdit = React.lazy(() => import('./ArtistEdit'));
const ArtistManageTagUsages = React.lazy(
	() => import('./ArtistManageTagUsages'),
);
const ArtistMerge = React.lazy(() => import('./ArtistMerge'));
const ArtistVersions = React.lazy(() => import('./ArtistVersions'));
const ArtistViewVersion = React.lazy(() => import('./ArtistViewVersion'));

const ArtistDetailsNavigate = (): React.ReactElement => {
	const { id } = useParams();

	return (
		<Navigate
			to={EntryUrlMapper.details(EntryType.Artist, Number(id))}
			replace={true}
		/>
	);
};

const ArtistRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route
				path=""
				element={
					<Navigate
						to={`/Search?${qs.stringify({ searchType: SearchType.Artist })}`}
						replace={true}
					/>
				}
			/>
			<Route path="Create" element={<ArtistCreate />} />
			<Route path="Details/:id/*" element={<ArtistDetailsNavigate />} />
			<Route path="Edit/:id" element={<ArtistEdit />} />
			<Route path="ManageTagUsages/:id" element={<ArtistManageTagUsages />} />
			<Route path="Merge/:id" element={<ArtistMerge />} />
			<Route path="Versions/:id" element={<ArtistVersions />} />
			<Route path="ViewVersion/:id" element={<ArtistViewVersion />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default ArtistRoutes;
