import { SongGrid } from '@/Components/Shared/Partials/Song/SongGrid';
import { RelatedSongs } from '@/DataContracts/Song/RelatedSongs';
import { SongDetailsForApi } from '@/DataContracts/Song/SongDetailsForApi';
import { SongDetailsTabs } from '@/Pages/Song/SongDetailsRoutes';
import { httpClient } from '@/Shared/HttpClient';
import { urlMapper } from '@/Shared/UrlMapper';
import { SongDetailsStore } from '@/Stores/Song/SongDetailsStore';
import { useVdb } from '@/VdbContext';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface SongRelatedProps {
	model: SongDetailsForApi;
	songDetailsStore: SongDetailsStore;
}

const SongRelated = ({
	model,
	songDetailsStore,
}: SongRelatedProps): React.ReactElement => {
	const { t } = useTranslation(['ViewRes.Song']);

	const [relatedSongs, setRelatedSongs] = React.useState<
		RelatedSongs | undefined
	>(undefined);

	React.useEffect(() => {
		songDetailsStore.getRelated().then(setRelatedSongs);
	}, [model, songDetailsStore]);

	return (
		<SongDetailsTabs
			model={model}
			songDetailsStore={songDetailsStore}
			tab="related"
		>
			{relatedSongs !== undefined && (
				<>
					{relatedSongs.artistMatches.length > 0 && (
						<>
							<h3>{t('ViewRes.Song:Details.MatchingArtists')}</h3>
							<SongGrid songs={relatedSongs.artistMatches} columns={2} />
						</>
					)}
					{relatedSongs.likeMatches.length > 0 && (
						<>
							<h3>{t('ViewRes.Song:Details.MatchingLikes')}</h3>
							<SongGrid songs={relatedSongs.likeMatches} columns={2} />
						</>
					)}
					{relatedSongs.tagMatches.length > 0 && (
						<>
							<h3>{t('ViewRes.Song:Details.MatchingTags')}</h3>
							<SongGrid songs={relatedSongs.tagMatches} columns={2} />
						</>
					)}
				</>
			)}
		</SongDetailsTabs>
	);
};

export default SongRelated;
