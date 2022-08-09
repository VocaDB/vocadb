import Button from '@/Bootstrap/Button';
import ButtonGroup from '@/Bootstrap/ButtonGroup';
import { SongSearchDropdown } from '@/Components/Shared/Partials/Knockout/SearchDropdown';
import { ArtistDetailsContract } from '@/DataContracts/Artist/ArtistDetailsContract';
import { ArtistDetailsTabs } from '@/Pages/Artist/ArtistDetailsRoutes';
import SongSearchList from '@/Pages/Search/Partials/SongSearchList';
import { ArtistDetailsStore } from '@/Stores/Artist/ArtistDetailsStore';
import { useStoreWithPagination } from '@vocadb/route-sphere';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useParams } from 'react-router';

interface ArtistSongsProps {
	artist: ArtistDetailsContract;
	artistDetailsStore: ArtistDetailsStore;
}

const ArtistSongs = observer(
	({ artist, artistDetailsStore }: ArtistSongsProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.Artist']);

		const { id } = useParams();

		React.useEffect(() => {
			artistDetailsStore.songsStore.artistFilters.artistIds = [Number(id)];
		}, [id, artistDetailsStore]);

		useStoreWithPagination(artistDetailsStore.songsStore);

		return (
			<ArtistDetailsTabs
				artist={artist}
				artistDetailsStore={artistDetailsStore}
				tab="songs"
			>
				<div className="clearfix">
					<div className="pull-right">
						<SongSearchDropdown
							songSearchStore={artistDetailsStore.songsStore}
						/>{' '}
						<ButtonGroup>
							<Button
								onClick={(): void =>
									runInAction(() => {
										artistDetailsStore.songsStore.viewMode = 'Details';
									})
								}
								className={classNames(
									'btn-nomargin',
									artistDetailsStore.songsStore.viewMode === 'Details' &&
										'active',
								)}
								href="#"
								title={t('ViewRes.Artist:Details.ViewModeDetails')}
							>
								<i className="icon-th-list noMargin" />{' '}
								{t('ViewRes.Artist:Details.ViewModeDetails')}
							</Button>
							<Button
								onClick={(): void =>
									runInAction(() => {
										artistDetailsStore.songsStore.viewMode = 'PlayList';
									})
								}
								className={classNames(
									'btn-nomargin',
									artistDetailsStore.songsStore.viewMode === 'PlayList' &&
										'active',
								)}
								href="#"
								title={t('ViewRes.Artist:Details.ViewModePlayList')}
							>
								<i className="icon-list noMargin" />{' '}
								{t('ViewRes.Artist:Details.ViewModePlayList')}
							</Button>
						</ButtonGroup>
					</div>
				</div>
				<div>
					<SongSearchList songSearchStore={artistDetailsStore.songsStore} />
				</div>
			</ArtistDetailsTabs>
		);
	},
);

export default ArtistSongs;
