import { AlbumGrid } from '@/Components/Shared/Partials/Album/AlbumGrid';
import { AlbumDetailsForApi } from '@/DataContracts/Album/AlbumDetailsForApi';
import { RelatedAlbums } from '@/DataContracts/Album/RelatedAlbums';
import { AlbumDetailsTabs } from '@/Pages/Album/AlbumDetailsRoutes';
import { httpClient } from '@/Shared/HttpClient';
import { urlMapper } from '@/Shared/UrlMapper';
import { AlbumDetailsStore } from '@/Stores/Album/AlbumDetailsStore';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface AlbumRelatedProps {
	model: AlbumDetailsForApi;
	albumDetailsStore: AlbumDetailsStore;
}

const AlbumRelated = ({
	model,
	albumDetailsStore,
}: AlbumRelatedProps): React.ReactElement => {
	const { t } = useTranslation(['ViewRes.Album']);

	const [relatedAlbums, setRelatedAlbums] = React.useState<
		RelatedAlbums | undefined
	>(undefined);

	React.useEffect(() => {
		albumDetailsStore.getRelated().then(setRelatedAlbums);
	}, [model, albumDetailsStore]);

	return (
		<AlbumDetailsTabs
			model={model}
			albumDetailsStore={albumDetailsStore}
			tab="related"
		>
			{relatedAlbums !== undefined && (
				<>
					{relatedAlbums.artistMatches.length > 0 && (
						<>
							<h3>{t('ViewRes.Album:Details.MatchingArtists')}</h3>
							<AlbumGrid
								displayRating={false}
								displayReleaseDate={false}
								albums={relatedAlbums.artistMatches}
								columns={2}
							/>
						</>
					)}
					{relatedAlbums.likeMatches.length > 0 && (
						<>
							<h3>{t('ViewRes.Album:Details.MatchingLikes')}</h3>
							<AlbumGrid
								displayRating={false}
								displayReleaseDate={false}
								albums={relatedAlbums.likeMatches}
								columns={2}
							/>
						</>
					)}
					{relatedAlbums.tagMatches.length > 0 && (
						<>
							<h3>{t('ViewRes.Album:Details.MatchingTags')}</h3>
							<AlbumGrid
								displayRating={false}
								displayReleaseDate={false}
								albums={relatedAlbums.tagMatches}
								columns={2}
							/>
						</>
					)}
				</>
			)}
		</AlbumDetailsTabs>
	);
};

export default AlbumRelated;
