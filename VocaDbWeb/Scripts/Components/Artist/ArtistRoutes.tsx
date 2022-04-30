import ErrorNotFound from '@Components/Error/ErrorNotFound';
import EntryType from '@Models/EntryType';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import { SearchType } from '@Stores/Search/SearchStore';
import qs from 'qs';
import React from 'react';
import { Navigate, Route, Routes, useParams } from 'react-router-dom';

const ArtistEdit = React.lazy(() => import('./ArtistEdit'));
const ArtistVersions = React.lazy(() => import('./ArtistVersions'));

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
			<Route path="Details/:id/*" element={<ArtistDetailsNavigate />} />
			<Route path="Edit/:id" element={<ArtistEdit />} />
			<Route path="Versions/:id" element={<ArtistVersions />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default ArtistRoutes;
