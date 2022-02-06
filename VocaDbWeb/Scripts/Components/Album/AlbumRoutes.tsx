import EntryType from '@Models/EntryType';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import { SearchType } from '@Stores/Search/SearchStore';
import qs from 'qs';
import React from 'react';
import { Navigate, Route, Routes, useParams } from 'react-router-dom';

import ErrorNotFound from '../Error/ErrorNotFound';

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
			<Route path="Details/:id/*" element={<AlbumDetailsNavigate />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default AlbumRoutes;
