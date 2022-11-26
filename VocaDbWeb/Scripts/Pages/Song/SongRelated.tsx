import { SongDetailsForApi } from '@/DataContracts/Song/SongDetailsForApi';
import { SongDetailsTabs } from '@/Pages/Song/SongDetailsRoutes';
import { httpClient } from '@/Shared/HttpClient';
import { urlMapper } from '@/Shared/UrlMapper';
import { SongDetailsStore } from '@/Stores/Song/SongDetailsStore';
import React from 'react';

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
