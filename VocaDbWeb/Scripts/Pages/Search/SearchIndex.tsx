import Button from '@/Bootstrap/Button';
import ButtonGroup from '@/Bootstrap/ButtonGroup';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { Layout } from '@/Components/Shared/Layout';
import {
	AlbumSearchDropdown,
	ArtistSearchDropdown,
	EventSearchDropdown,
	SongSearchDropdown,
} from '@/Components/Shared/Partials/Knockout/SearchDropdown';
import { TagFilters } from '@/Components/Shared/Partials/Knockout/TagFilters';
import { useVdbPlayer } from '@/Components/VdbPlayer/VdbPlayerContext';
import AlbumSearchList from '@/Pages/Search/Partials/AlbumSearchList';
import AlbumSearchOptions from '@/Pages/Search/Partials/AlbumSearchOptions';
import AnythingSearchList from '@/Pages/Search/Partials/AnythingSearchList';
import ArtistSearchList from '@/Pages/Search/Partials/ArtistSearchList';
import ArtistSearchOptions from '@/Pages/Search/Partials/ArtistSearchOptions';
import EventSearchList from '@/Pages/Search/Partials/EventSearchList';
import EventSearchOptions from '@/Pages/Search/Partials/EventSearchOptions';
import SongSearchList from '@/Pages/Search/Partials/SongSearchList';
import SongSearchOptions from '@/Pages/Search/Partials/SongSearchOptions';
import TagSearchList from '@/Pages/Search/Partials/TagSearchList';
import TagSearchOptions from '@/Pages/Search/Partials/TagSearchOptions';
import { albumRepo } from '@/Repositories/AlbumRepository';
import { artistRepo } from '@/Repositories/ArtistRepository';
import { entryRepo } from '@/Repositories/EntryRepository';
import { eventRepo } from '@/Repositories/ReleaseEventRepository';
import { songRepo } from '@/Repositories/SongRepository';
import { tagRepo } from '@/Repositories/TagRepository';
import { userRepo } from '@/Repositories/UserRepository';
import { urlMapper } from '@/Shared/UrlMapper';
import { SearchStore, SearchType } from '@/Stores/Search/SearchStore';
import { PlayQueueRepositoryType } from '@/Stores/VdbPlayer/PlayQueueRepository';
import { AutoplayContext } from '@/Stores/VdbPlayer/PlayQueueStore';
import { useLocationStateStore } from '@vocadb/route-sphere';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { DebounceInput } from 'react-debounce-input';
import { useTranslation } from 'react-i18next';

import '../../../wwwroot/Content/Styles/songlist.less';

const searchStore = new SearchStore(
	vdb.values,
	urlMapper,
	entryRepo,
	artistRepo,
	albumRepo,
	songRepo,
	eventRepo,
	tagRepo,
	userRepo,
);

interface SearchCategoryProps {
	entryType: SearchType;
	title: string;
}

const SearchCategory = observer(
	({ entryType, title }: SearchCategoryProps): React.ReactElement => {
		return (
			<li
				className={classNames(searchStore.searchType === entryType && 'active')}
			>
				<SafeAnchor
					onClick={(): void =>
						runInAction(() => {
							searchStore.searchType = entryType;
						})
					}
				>
					{title}
				</SafeAnchor>
			</li>
		);
	},
);

const SearchIndex = observer(
	(): React.ReactElement => {
		const { t } = useTranslation([
			'ViewRes',
			'ViewRes.Search',
			'VocaDb.Web.Resources.Domain',
		]);

		useLocationStateStore(searchStore);

		const { playQueue } = useVdbPlayer();

		return (
			<Layout pageTitle={undefined} ready={true}>
				<ul className="nav nav-pills">
					<SearchCategory
						entryType={SearchType.Anything}
						title={t('VocaDb.Web.Resources.Domain:EntryTypeNames.Undefined')}
					/>
					<SearchCategory
						entryType={SearchType.Artist}
						title={t('ViewRes:Shared.Artists')}
					/>
					<SearchCategory
						entryType={SearchType.Album}
						title={t('ViewRes:Shared.Albums')}
					/>
					<SearchCategory
						entryType={SearchType.Song}
						title={t('ViewRes:Shared.Songs')}
					/>
					<SearchCategory
						entryType={SearchType.ReleaseEvent}
						title={t('ViewRes:Shared.ReleaseEvents')}
					/>
					<SearchCategory
						entryType={SearchType.Tag}
						title={t('ViewRes:Shared.Tags')}
					/>
				</ul>

				<div
					id="anythingSearchTab"
					className="form-horizontal well well-transparent"
				>
					<div className="pull-right">
						{searchStore.showArtistSearch && (
							<ArtistSearchDropdown
								artistSearchStore={searchStore.artistSearchStore}
							/>
						)}
						{searchStore.showAlbumSearch && (
							<AlbumSearchDropdown
								albumSearchStore={searchStore.albumSearchStore}
							/>
						)}
						{searchStore.showSongSearch && (
							<SongSearchDropdown
								songSearchStore={searchStore.songSearchStore}
							/>
						)}
						{searchStore.showEventSearch && (
							<EventSearchDropdown
								eventSearchStore={searchStore.eventSearchStore}
							/>
						)}

						{searchStore.showAlbumSearch && (
							<>
								{' '}
								<div className="inline-block">
									<ButtonGroup>
										<Button
											className={classNames(
												searchStore.albumSearchStore.viewMode === 'Details' &&
													'active',
												'btn-nomargin',
											)}
											onClick={(): void =>
												runInAction(() => {
													searchStore.albumSearchStore.viewMode = 'Details';
												})
											}
											href="#"
											title={t('ViewRes.Search:Index.AlbumDetails')}
										>
											<i className="icon-list" />
										</Button>
										<Button
											className={classNames(
												searchStore.albumSearchStore.viewMode === 'Tiles' &&
													'active',
												'btn-nomargin',
											)}
											onClick={(): void =>
												runInAction(() => {
													searchStore.albumSearchStore.viewMode = 'Tiles';
												})
											}
											href="#"
											title={t('ViewRes.Search:Index.AlbumTiles')}
										>
											<i className="icon-th" />
										</Button>
									</ButtonGroup>
								</div>
							</>
						)}

						{searchStore.showSongSearch && (
							<>
								{' '}
								<div className="inline-block">
									<ButtonGroup>
										<Button
											onClick={async (): Promise<void> => {
												await playQueue.startAutoplay(
													new AutoplayContext(
														PlayQueueRepositoryType.Songs,
														searchStore.songSearchStore.queryParams,
													),
												);
											}}
											title="Play" /* LOC */
											className="btn-nomargin"
										>
											<i className="icon-play noMargin" /> Play
											{/* LOC */}
										</Button>
									</ButtonGroup>
								</div>
							</>
						)}

						{searchStore.showTagFilter && (
							<>
								{' '}
								<div className="inline-block">
									<Button
										className={classNames(
											searchStore.showTags && 'active',
											'btn-nomargin',
										)}
										onClick={(): void =>
											runInAction(() => {
												searchStore.showTags = !searchStore.showTags;
											})
										}
										title={t('ViewRes.Search:Index.ShowTags')}
									>
										<i className="icon-tags" />
									</Button>
								</div>
							</>
						)}
					</div>

					<div className="control-label">
						<i className="icon-search" />
					</div>
					<div className="control-group">
						<div className="controls">
							<div className="input-append">
								<DebounceInput
									type="text"
									value={searchStore.searchTerm}
									onChange={(e): void =>
										runInAction(() => {
											searchStore.searchTerm = e.target.value;
										})
									}
									className="input-xlarge"
									placeholder={t('ViewRes.Search:Index.TypeSomething')}
									debounceTimeout={300}
								/>
								{searchStore.searchTerm && (
									<Button
										variant="danger"
										onClick={(e): void =>
											runInAction(() => {
												searchStore.searchTerm = '';
											})
										}
									>
										{t('ViewRes:Shared.Clear')}
									</Button>
								)}
								&nbsp;
							</div>{' '}
							<Button
								onClick={(): void =>
									runInAction(() => {
										searchStore.showAdvancedFilters = !searchStore.showAdvancedFilters;
									})
								}
								className={classNames(
									searchStore.showAdvancedFilters && 'active',
								)}
							>
								{t('ViewRes.Search:Index.MoreFilters')}{' '}
								<span className="caret" />
							</Button>
						</div>
					</div>

					{searchStore.showAdvancedFilters && (
						<div>
							{/* Tag filtering with top genres */}
							{searchStore.showTagFilter && (
								<div className="control-group">
									<div className="control-label">{t('ViewRes:Shared.Tag')}</div>
									<div className="controls">
										<TagFilters
											tagFilters={searchStore.tagFilters}
											genreTags={searchStore.genreTags}
										/>
									</div>
								</div>
							)}

							{searchStore.showArtistSearch && (
								<ArtistSearchOptions
									artistSearchStore={searchStore.artistSearchStore}
								/>
							)}
							{searchStore.showAlbumSearch && (
								<AlbumSearchOptions
									albumSearchStore={searchStore.albumSearchStore}
								/>
							)}
							{searchStore.showSongSearch && (
								<SongSearchOptions
									songSearchStore={searchStore.songSearchStore}
								/>
							)}
							{searchStore.showEventSearch && (
								<EventSearchOptions
									eventSearchStore={searchStore.eventSearchStore}
								/>
							)}
							{searchStore.showTagSearch && (
								<TagSearchOptions tagSearchStore={searchStore.tagSearchStore} />
							)}

							{/* Checkboxes */}
							<div className="control-group">
								<div className="controls">
									{searchStore.showArtistSearch && (
										<div>
											{vdb.values.isLoggedIn && (
												<label className="checkbox">
													<input
														type="checkbox"
														checked={
															searchStore.artistSearchStore.onlyFollowedByMe
														}
														onChange={(e): void =>
															runInAction(() => {
																searchStore.artistSearchStore.onlyFollowedByMe =
																	e.target.checked;
															})
														}
													/>
													{t('ViewRes.Search:Index.OnlyMyFollowedArtists')}
												</label>
											)}
										</div>
									)}

									{searchStore.showSongSearch && (
										<div>
											<label className="checkbox">
												<input
													type="checkbox"
													checked={searchStore.songSearchStore.pvsOnly}
													onChange={(e): void =>
														runInAction(() => {
															searchStore.songSearchStore.pvsOnly =
																e.target.checked;
														})
													}
												/>
												{t('ViewRes.Search:Index.OnlyWithPVs')}
											</label>

											{vdb.values.isLoggedIn && (
												<label className="checkbox">
													<input
														type="checkbox"
														checked={searchStore.songSearchStore.onlyRatedSongs}
														onChange={(e): void =>
															runInAction(() => {
																searchStore.songSearchStore.onlyRatedSongs =
																	e.target.checked;
															})
														}
													/>
													{t('ViewRes.Search:Index.InMyCollection')}
												</label>
											)}
										</div>
									)}

									{searchStore.showEventSearch && (
										<div>
											{vdb.values.isLoggedIn && (
												<label className="checkbox">
													<input
														type="checkbox"
														checked={searchStore.eventSearchStore.onlyMyEvents}
														onChange={(e): void =>
															runInAction(() => {
																searchStore.eventSearchStore.onlyMyEvents =
																	e.target.checked;
															})
														}
													/>
													{t('ViewRes.Search:Index.OnlyMyEvents')}
												</label>
											)}
										</div>
									)}

									{searchStore.showDraftsFilter && (
										<label className="checkbox">
											<input
												type="checkbox"
												checked={searchStore.draftsOnly}
												onChange={(e): void =>
													runInAction(() => {
														searchStore.draftsOnly = e.target.checked;
													})
												}
											/>
											{t('ViewRes:EntryIndex.OnlyDrafts')}
										</label>
									)}
								</div>
							</div>
						</div>
					)}
				</div>

				{searchStore.showAnythingSearch && (
					<AnythingSearchList
						searchStore={searchStore}
						anythingSearchStore={searchStore.anythingSearchStore}
					/>
				)}
				{searchStore.showArtistSearch && (
					<ArtistSearchList artistSearchStore={searchStore.artistSearchStore} />
				)}
				{searchStore.showAlbumSearch && (
					<AlbumSearchList albumSearchStore={searchStore.albumSearchStore} />
				)}
				{searchStore.showSongSearch && (
					<SongSearchList songSearchStore={searchStore.songSearchStore} />
				)}
				{searchStore.showEventSearch && (
					<EventSearchList eventSearchStore={searchStore.eventSearchStore} />
				)}
				{searchStore.showTagSearch && (
					<TagSearchList tagSearchStore={searchStore.tagSearchStore} />
				)}
			</Layout>
		);
	},
);

export default SearchIndex;
