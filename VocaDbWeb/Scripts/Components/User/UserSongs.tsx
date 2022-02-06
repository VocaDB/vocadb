import Button from '@Bootstrap/Button';
import UserDetailsContract from '@DataContracts/User/UserDetailsContract';
import RatedSongsSearchStore from '@Stores/User/RatedSongsSearchStore';
import UserDetailsStore from '@Stores/User/UserDetailsStore';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { DebounceInput } from 'react-debounce-input';
import { useTranslation } from 'react-i18next';

import TagAutoComplete from '../KnockoutExtensions/TagAutoComplete';
import SongSearchList from '../Search/Partials/SongSearchList';
import ArtistFilters from '../Shared/Partials/Knockout/ArtistFilters';
import { RatedSongsSearchDropdown } from '../Shared/Partials/Knockout/SearchDropdown';
import { SongAdvancedFilters } from '../Shared/Partials/Search/AdvancedFilters';
import TagFilters from '../Shared/Partials/TagFilters';
import SongVoteRatingsRadioKnockout from '../Shared/Partials/User/SongVoteRatingsRadioKnockout';
import useStoreWithPaging from '../useStoreWithPaging';
import { UserDetailsNav } from './UserDetailsRoutes';

interface RatedSongsProps {
	ratedSongsStore: RatedSongsSearchStore;
}

const RatedSongs = observer(
	({ ratedSongsStore }: RatedSongsProps): React.ReactElement => {
		const { t } = useTranslation([
			'ViewRes',
			'ViewRes.Search',
			'ViewRes.User',
			'VocaDb.Web.Resources.Domain',
		]);

		useStoreWithPaging(ratedSongsStore);

		return (
			<>
				<div className="form-horizontal well well-transparent">
					<div className="pull-right">
						<RatedSongsSearchDropdown ratedSongsStore={ratedSongsStore} />{' '}
						<Button
							onClick={(): void =>
								runInAction(() => {
									ratedSongsStore.groupByRating = !ratedSongsStore.groupByRating;
								})
							}
							className={classNames(ratedSongsStore.groupByRating && 'active')}
						>
							{t('ViewRes.User:RatedSongs.GroupByRating')}
						</Button>{' '}
						<div className="inline-block">
							<div className="btn-group">
								<Button
									onClick={(): void =>
										runInAction(() => {
											ratedSongsStore.viewMode = 'Details';
										})
									}
									className={classNames(
										ratedSongsStore.viewMode === 'Details' && 'active',
										'btn-nomargin',
									)}
									href="#"
									title={t('ViewRes.Search:Index.AlbumDetails')}
								>
									<i className="icon-th-list" />
								</Button>
								<Button
									onClick={(): void =>
										runInAction(() => {
											ratedSongsStore.viewMode = 'PlayList';
										})
									}
									className={classNames(
										ratedSongsStore.viewMode === 'PlayList' && 'active',
										'btn-nomargin',
									)}
									href="#"
									title={t('ViewRes.Search:Index.Playlist')}
								>
									<i className="icon-list" />
								</Button>
							</div>
						</div>{' '}
						<Button
							onClick={(): void =>
								runInAction(() => {
									ratedSongsStore.showTags = !ratedSongsStore.showTags;
								})
							}
							className={classNames(
								ratedSongsStore.showTags && 'active',
								'btn-nomargin',
							)}
							href="#"
							title={t('ViewRes.User:RatedSongs.ShowTags')}
						>
							<i className="icon-tags" />
						</Button>
					</div>

					<div className="control-label">
						<i className="icon-search" />
					</div>
					<div className="control-group">
						<div className="controls">
							<div className="input-append">
								<DebounceInput
									type="text"
									value={ratedSongsStore.searchTerm}
									onChange={(e): void =>
										runInAction(() => {
											ratedSongsStore.searchTerm = e.target.value;
										})
									}
									className="input-xlarge"
									placeholder={t('ViewRes.Search:Index.TypeSomething')}
									debounceTimeout={300}
								/>
								{ratedSongsStore.searchTerm && (
									<Button
										variant="danger"
										onClick={(): void =>
											runInAction(() => {
												ratedSongsStore.searchTerm = '';
											})
										}
									>
										{t('ViewRes:Shared.Clear')}
									</Button>
								)}
							</div>
						</div>
					</div>

					<div className="control-group">
						<div className="control-label">
							{t('ViewRes.Search:Index.Rating')}
						</div>
						<div className="controls">
							<SongVoteRatingsRadioKnockout
								rating={ratedSongsStore.rating}
								onRatingChange={(rating): void =>
									runInAction(() => {
										ratedSongsStore.rating = rating;
									})
								}
							/>
						</div>
					</div>

					<div className="control-group">
						<div className="control-label">{t('ViewRes:Shared.Tag')}</div>
						<div className="controls">
							<TagFilters tagFilters={ratedSongsStore.tagFilters} />
							<div>
								<TagAutoComplete
									type="text"
									className="input-large"
									onAcceptSelection={ratedSongsStore.tagFilters.addTag}
									placeholder={t('ViewRes:Shared.Search')}
								/>
							</div>
						</div>
					</div>

					<div className="control-group">
						<div className="control-label">
							{t('VocaDb.Web.Resources.Domain:EntryTypeNames.Artist')}
						</div>
						<div className="controls">
							<ArtistFilters
								artistFilters={ratedSongsStore.artistFilters}
								artistParticipationStatus={false}
							/>
						</div>
					</div>

					<div className="control-group">
						<div className="control-label">
							{t('ViewRes.User:RatedSongs.SongList')}
						</div>
						<div className="controls">
							<div className="input-append">
								<select
									value={ratedSongsStore.songListId}
									onChange={(e): void =>
										runInAction(() => {
											ratedSongsStore.songListId = Number(e.target.value);
										})
									}
								>
									<option>
										{t('ViewRes.User:RatedSongs.NoSongListSelection')}
									</option>
									{ratedSongsStore.songLists.map((songList) => (
										<option value={songList.id} key={songList.id}>
											{songList.name}
										</option>
									))}
								</select>
							</div>
						</div>
					</div>

					<div className="control-group">
						<div className="control-label"></div>
						<div className="controls">
							<SongAdvancedFilters
								advancedFilters={ratedSongsStore.advancedFilters}
							/>
						</div>
					</div>
				</div>

				<div>
					<SongSearchList songSearchStore={ratedSongsStore} />
				</div>
			</>
		);
	},
);

interface UserSongsProps {
	user: UserDetailsContract;
	userDetailsStore: UserDetailsStore;
}

const UserSongs = ({
	user,
	userDetailsStore,
}: UserSongsProps): React.ReactElement => {
	React.useEffect(() => {
		userDetailsStore.ratedSongsStore.init();
	}, [userDetailsStore]);

	return (
		<>
			<UserDetailsNav user={user} tab="songs" />

			<RatedSongs ratedSongsStore={userDetailsStore.ratedSongsStore} />
		</>
	);
};

export default UserSongs;
