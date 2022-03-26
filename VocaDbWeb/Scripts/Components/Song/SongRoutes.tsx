import ErrorNotFound from '@Components/Error/ErrorNotFound';
import EntryType from '@Models/EntryType';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import React from 'react';
import { Navigate, Route, Routes, useParams } from 'react-router-dom';

const SongRankings = React.lazy(() => import('./SongRankings'));

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
			<Route path="Details/:id" element={<SongDetailsNavigate />} />
			<Route path="Rankings" element={<SongRankings />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default SongRoutes;
