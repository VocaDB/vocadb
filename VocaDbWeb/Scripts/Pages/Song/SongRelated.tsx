import SongDetailsForApi from '@/DataContracts/Song/SongDetailsForApi';
import HttpClient from '@/Shared/HttpClient';
import UrlMapper from '@/Shared/UrlMapper';
import SongDetailsStore from '@/Stores/Song/SongDetailsStore';
import React from 'react';

import { SongDetailsTabs } from './SongDetailsRoutes';

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

interface SongRelatedProps {
	model: SongDetailsForApi;
	songDetailsStore: SongDetailsStore;
}

const SongRelated = ({
	model,
	songDetailsStore,
}: SongRelatedProps): React.ReactElement => {
	const [contentHtml, setContentHtml] = React.useState<string | undefined>();

	React.useEffect(() => {
		httpClient
			.get<string>(urlMapper.mapRelative(`/Song/Related/${model.id}`))
			.then((contentHtml) => setContentHtml(contentHtml));
	}, [model]);

	return (
		<SongDetailsTabs
			model={model}
			songDetailsStore={songDetailsStore}
			tab="related"
		>
			{contentHtml && (
				// TODO: Replace this with React
				<div
					dangerouslySetInnerHTML={{
						__html: contentHtml,
					}}
				/>
			)}
		</SongDetailsTabs>
	);
};

export default SongRelated;
