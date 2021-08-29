import Button from '@Bootstrap/Button';
import ButtonGroup from '@Bootstrap/ButtonGroup';
import SafeAnchor from '@Bootstrap/SafeAnchor';
import Layout from '@Components/Shared/Layout';
import {
	AlbumSearchDropdown,
	ArtistSearchDropdown,
	EventSearchDropdown,
	SongSearchDropdown,
} from '@Components/Shared/Partials/Knockout/SearchDropdown';
import TagFilters from '@Components/Shared/Partials/Knockout/TagFilters';
import useScript from '@Components/useScript';
import useStoreWithRouteParams from '@Components/useStoreWithRouteParams';
import AlbumRepository from '@Repositories/AlbumRepository';
import ArtistRepository from '@Repositories/ArtistRepository';
import EntryRepository from '@Repositories/EntryRepository';
import ReleaseEventRepository from '@Repositories/ReleaseEventRepository';
import SongRepository from '@Repositories/SongRepository';
import TagRepository from '@Repositories/TagRepository';
import UserRepository from '@Repositories/UserRepository';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import PVPlayersFactory from '@Stores/PVs/PVPlayersFactory';
import SearchStore, {
	SearchRouteParams,
	SearchType,
} from '@Stores/Search/SearchStore';
import Ajv, { JSONSchemaType } from 'ajv';
import addFormats from 'ajv-formats';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { DebounceInput } from 'react-debounce-input';
import { useTranslation } from 'react-i18next';

import '../../../wwwroot/Content/Styles/songlist.css';
import AlbumSearchList from './Partials/AlbumSearchList';
import AlbumSearchOptions from './Partials/AlbumSearchOptions';
import AnythingSearchList from './Partials/AnythingSearchList';
import ArtistSearchList from './Partials/ArtistSearchList';
import ArtistSearchOptions from './Partials/ArtistSearchOptions';
import EventSearchList from './Partials/EventSearchList';
import EventSearchOptions from './Partials/EventSearchOptions';
import SongSearchList from './Partials/SongSearchList';
import SongSearchOptions from './Partials/SongSearchOptions';
import TagSearchList from './Partials/TagSearchList';
import TagSearchOptions from './Partials/TagSearchOptions';

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);
const entryRepo = new EntryRepository(httpClient, vdb.values.baseAddress);
const artistRepo = new ArtistRepository(httpClient, vdb.values.baseAddress);
const albumRepo = new AlbumRepository(httpClient, vdb.values.baseAddress);
const songRepo = new SongRepository(httpClient, vdb.values.baseAddress);
const eventRepo = new ReleaseEventRepository(httpClient, urlMapper);
const tagRepo = new TagRepository(httpClient, vdb.values.baseAddress);
const userRepo = new UserRepository(httpClient, urlMapper);

const pvPlayersFactory = new PVPlayersFactory();

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
	pvPlayersFactory,
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

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });
addFormats(ajv);

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const schema: JSONSchemaType<SearchRouteParams> = require('@Stores/Search/SearchRouteParams.schema');
const validate = ajv.compile(schema);

const SearchIndex = observer(
	(): React.ReactElement => {
		const { t } = useTranslation([
			'ViewRes',
			'ViewRes.Search',
			'VocaDb.Web.Resources.Domain',
		]);

		useStoreWithRouteParams(validate, searchStore);

		React.useEffect(() => {
			searchStore.updateResults();
		}, []);

		useScript('/Scripts/soundcloud-api.js');
		useScript('https://www.youtube.com/iframe_api');

		return (
			<Layout>
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
											className={classNames(
												searchStore.songSearchStore.viewMode === 'Details' &&
													'active',
												'btn-nomargin',
											)}
											onClick={(): void =>
												runInAction(() => {
													searchStore.songSearchStore.viewMode = 'Details';
												})
											}
											href="#"
											title={t('ViewRes.Search:Index.AlbumDetails')}
										>
											<i className="icon-th-list" />
										</Button>
										<Button
											className={classNames(
												searchStore.songSearchStore.viewMode === 'PlayList' &&
													'active',
												'btn-nomargin',
											)}
											onClick={(): void =>
												runInAction(() => {
													searchStore.songSearchStore.viewMode = 'PlayList';
												})
											}
											href="#"
											title={t('ViewRes.Search:Index.Playlist')}
										>
											<i className="icon-list" />
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
											searchStore={searchStore}
											tagFilters={searchStore.tagFilters}
											topGenres={true}
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
