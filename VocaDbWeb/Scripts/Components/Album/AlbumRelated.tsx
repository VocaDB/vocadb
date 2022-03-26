import AlbumDetailsForApi from '@DataContracts/Album/AlbumDetailsForApi';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import AlbumDetailsStore from '@Stores/Album/AlbumDetailsStore';
import React from 'react';

import { AlbumDetailsTabs } from './AlbumDetailsRoutes';

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

interface AlbumRelatedProps {
	model: AlbumDetailsForApi;
	albumDetailsStore: AlbumDetailsStore;
}

const AlbumRelated = ({
	model,
	albumDetailsStore,
}: AlbumRelatedProps): React.ReactElement => {
	const [contentHtml, setContentHtml] = React.useState<string | undefined>();

	React.useEffect(() => {
		httpClient
			.get<string>(urlMapper.mapRelative(`/Album/Related/${model.id}`))
			.then((contentHtml) => setContentHtml(contentHtml));
	}, [model]);

	return (
		<AlbumDetailsTabs
			model={model}
			albumDetailsStore={albumDetailsStore}
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
		</AlbumDetailsTabs>
	);
};

export default AlbumRelated;
