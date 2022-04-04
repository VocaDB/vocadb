import Button from '@Bootstrap/Button';
import SafeAnchor from '@Bootstrap/SafeAnchor';
import { MediaType } from '@DataContracts/User/AlbumForUserForApiContract';
import UserDetailsContract from '@DataContracts/User/UserDetailsContract';
import EntryStatus from '@Models/EntryStatus';
import EntryType from '@Models/EntryType';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import { AlbumSortRule } from '@Stores/Search/AlbumSearchStore';
import AlbumCollectionStore from '@Stores/User/AlbumCollectionStore';
import UserDetailsStore from '@Stores/User/UserDetailsStore';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { DebounceInput } from 'react-debounce-input';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

import { AlbumSearchListTiles } from '../Search/Partials/AlbumSearchList';
import DiscTypesDropdownKnockout from '../Shared/Partials/Album/DiscTypesDropdownKnockout';
import EntryCountBox from '../Shared/Partials/EntryCountBox';
import ArtistLockingAutoComplete from '../Shared/Partials/Knockout/ArtistLockingAutoComplete';
import ReleaseEventLockingAutoComplete from '../Shared/Partials/Knockout/ReleaseEventLockingAutoComplete';
import { AlbumSearchDropdown } from '../Shared/Partials/Knockout/SearchDropdown';
import ServerSidePaging from '../Shared/Partials/Knockout/ServerSidePaging';
import TagLockingAutoComplete from '../Shared/Partials/Knockout/TagLockingAutoComplete';
import { AlbumAdvancedFilters } from '../Shared/Partials/Search/AdvancedFilters';
import DraftIcon from '../Shared/Partials/Shared/DraftIcon';
import useStoreWithPaging from '../useStoreWithPaging';
import { UserDetailsNav } from './UserDetailsRoutes';

interface AlbumCollectionProps {
	albumCollectionStore: AlbumCollectionStore;
}

const AlbumCollection = observer(
	({ albumCollectionStore }: AlbumCollectionProps): React.ReactElement => {
		const { t } = useTranslation([
			'Resources',
			'ViewRes.Search',
			'ViewRes.User',
			'VocaDb.Model.Resources.Albums',
			'VocaDb.Web.Resources.Domain',
		]);

		useStoreWithPaging(albumCollectionStore);

		return (
			<>
				<div className="form-horizontal well well-transparent">
					<div className="pull-right">
						<AlbumSearchDropdown albumSearchStore={albumCollectionStore} />{' '}
						<div className="btn-group">
							<Button
								onClick={(): void =>
									runInAction(() => {
										albumCollectionStore.viewMode = 'Details';
									})
								}
								className={classNames(
									'btn-nomargin',
									albumCollectionStore.viewMode === 'Details' && 'active',
								)}
								href="#"
							>
								<i className="icon-list" />
							</Button>
							<Button
								onClick={(): void =>
									runInAction(() => {
										albumCollectionStore.viewMode = 'Tiles';
									})
								}
								className={classNames(
									'btn-nomargin',
									albumCollectionStore.viewMode === 'Tiles' && 'active',
								)}
								href="#"
							>
								<i className="icon-th" />
							</Button>
						</div>
					</div>

					<div className="control-label">
						<i className="icon-search" />
					</div>
					<div className="control-group">
						<div className="controls">
							<div className="input-append">
								<DebounceInput
									type="text"
									value={albumCollectionStore.searchTerm}
									onChange={(e): void =>
										runInAction(() => {
											albumCollectionStore.searchTerm = e.target.value;
										})
									}
									className="input-xlarge"
									placeholder={t('ViewRes.Search:Index.TypeSomething')}
									debounceTimeout={300}
								/>
								{albumCollectionStore.searchTerm && (
									<Button
										variant="danger"
										onClick={(): void =>
											runInAction(() => {
												albumCollectionStore.searchTerm = '';
											})
										}
									>
										{t('ViewRes:Shared.Clear')}
									</Button>
								)}
							</div>
						</div>
					</div>

					{albumCollectionStore.publicCollection && (
						<div className="control-group">
							<div className="control-label">
								{t('ViewRes.User:AlbumCollection.Status')}
							</div>
							<div className="controls">
								<Button
									onClick={(): void =>
										runInAction(() => {
											albumCollectionStore.collectionStatus = '';
										})
									}
									className={classNames(
										albumCollectionStore.collectionStatus === '' && 'active',
									)}
								>
									{t('ViewRes.User:AlbumCollection.CollectionStatusAnything')}
								</Button>{' '}
								<Button
									onClick={(): void =>
										runInAction(() => {
											albumCollectionStore.collectionStatus = 'Owned';
										})
									}
									className={classNames(
										albumCollectionStore.collectionStatus === 'Owned' &&
											'active',
									)}
								>
									{t('Resources:AlbumCollectionStatusNames.Owned')}
								</Button>{' '}
								<Button
									onClick={(): void =>
										runInAction(() => {
											albumCollectionStore.collectionStatus = 'Ordered';
										})
									}
									className={classNames(
										albumCollectionStore.collectionStatus === 'Ordered' &&
											'active',
									)}
								>
									{t('Resources:AlbumCollectionStatusNames.Ordered')}
								</Button>{' '}
								<Button
									onClick={(): void =>
										runInAction(() => {
											albumCollectionStore.collectionStatus = 'Wishlisted';
										})
									}
									className={classNames(
										albumCollectionStore.collectionStatus === 'Wishlisted' &&
											'active',
									)}
								>
									{t('Resources:AlbumCollectionStatusNames.Wishlisted')}
								</Button>
							</div>
						</div>
					)}

					<div className="control-group">
						<div className="control-label">{t('ViewRes:Shared.Tag')}</div>
						<div className="controls">
							<div className="input-append input-prepend">
								{!albumCollectionStore.tag.isEmpty && (
									<Button
										as={Link}
										className="btn-nomargin"
										to={albumCollectionStore.tagUrl!}
										title="Tag information" /* TODO: localize */
									>
										<i className="icon icon-info-sign" />
									</Button>
								)}
								<TagLockingAutoComplete
									basicEntryLinkStore={albumCollectionStore.tag}
								/>
							</div>
						</div>
					</div>

					<div className="control-group">
						<div className="control-label">
							{t('ViewRes.Search:Index.AlbumType')}
						</div>
						<div className="controls">
							<DiscTypesDropdownKnockout
								activeKey={albumCollectionStore.albumType}
								onSelect={(eventKey): void =>
									runInAction(() => {
										albumCollectionStore.albumType = eventKey;
									})
								}
							/>
						</div>
					</div>

					<div className="control-group">
						<div className="control-label">
							{t('VocaDb.Web.Resources.Domain:EntryTypeNames.Artist')}
						</div>
						<div className="controls">
							<div className="input-append">
								<ArtistLockingAutoComplete
									basicEntryLinkStore={albumCollectionStore.artist}
								/>
							</div>
						</div>
					</div>

					<div className="control-group">
						<div className="control-label">
							{t('ViewRes.User:AlbumCollection.ReleaseEvent')}
						</div>
						<div className="controls">
							<div className="input-append input-prepend">
								{!albumCollectionStore.releaseEvent.isEmpty && (
									<Button
										as={Link}
										className="btn-nomargin"
										to={albumCollectionStore.releaseEventUrl}
										title="Release event information" /* TODO: localize */
									>
										<i className="icon icon-info-sign" />
									</Button>
								)}
								<ReleaseEventLockingAutoComplete
									basicEntryLinkStore={albumCollectionStore.releaseEvent}
								/>
							</div>
						</div>
					</div>

					<div className="control-group">
						<div className="control-label">
							{t('ViewRes.User:AlbumCollection.MediaType')}
						</div>
						<div className="controls">
							<Button
								onClick={(): void =>
									runInAction(() => {
										albumCollectionStore.mediaType = undefined;
									})
								}
								className={classNames(
									albumCollectionStore.mediaType === undefined && 'active',
								)}
							>
								{t('ViewRes.User:AlbumCollection.CollectionStatusAnything')}
							</Button>{' '}
							<Button
								onClick={(): void =>
									runInAction(() => {
										albumCollectionStore.mediaType = MediaType.PhysicalDisc;
									})
								}
								className={classNames(
									albumCollectionStore.mediaType === MediaType.PhysicalDisc &&
										'active',
								)}
							>
								{t('Resources:AlbumMediaTypeNames.PhysicalDisc')}
							</Button>{' '}
							<Button
								onClick={(): void =>
									runInAction(() => {
										albumCollectionStore.mediaType = MediaType.DigitalDownload;
									})
								}
								className={classNames(
									albumCollectionStore.mediaType ===
										MediaType.DigitalDownload && 'active',
								)}
							>
								{t('Resources:AlbumMediaTypeNames.DigitalDownload')}
							</Button>{' '}
							<Button
								onClick={(): void =>
									runInAction(() => {
										albumCollectionStore.mediaType = MediaType.Other;
									})
								}
								className={classNames(
									albumCollectionStore.mediaType === MediaType.Other &&
										'active',
								)}
							>
								{t('Resources:AlbumMediaTypeNames.Other')}
							</Button>
						</div>
					</div>

					<div className="control-group">
						<div className="control-label"></div>
						<div className="controls">
							<AlbumAdvancedFilters
								advancedFilters={albumCollectionStore.advancedFilters}
							/>
						</div>
					</div>
				</div>

				<div className={classNames(albumCollectionStore.loading && 'loading')}>
					<EntryCountBox pagingStore={albumCollectionStore.paging} />

					<ServerSidePaging pagingStore={albumCollectionStore.paging} />

					{albumCollectionStore.viewMode === 'Details' && (
						<table className="table table-striped">
							<thead>
								<tr>
									<th colSpan={2}>
										<SafeAnchor
											onClick={(): void =>
												runInAction(() => {
													albumCollectionStore.sort = AlbumSortRule.Name;
												})
											}
											href="#"
										>
											{t('ViewRes.User:AlbumCollection.Album')}
											{albumCollectionStore.sort === AlbumSortRule.Name && (
												<>
													{' '}
													<span className="sortDirection_down" />
												</>
											)}
										</SafeAnchor>
									</th>
									<th>
										<SafeAnchor
											onClick={(): void =>
												runInAction(() => {
													albumCollectionStore.sort = AlbumSortRule.ReleaseDate;
												})
											}
											href="#"
										>
											{t('ViewRes.User:AlbumCollection.ReleaseDate')}
											{albumCollectionStore.sort ===
												AlbumSortRule.ReleaseDate && (
												<>
													{' '}
													<span className="sortDirection_down" />
												</>
											)}
										</SafeAnchor>
									</th>
									{albumCollectionStore.publicCollection && (
										<>
											<th>{t('ViewRes.User:AlbumCollection.Status')}</th>
											<th>{t('ViewRes.User:AlbumCollection.MediaType')}</th>
										</>
									)}
									<th>{t('ViewRes.User:AlbumCollection.Rating')}</th>
								</tr>
							</thead>
							<tbody>
								{albumCollectionStore.page.map((albumForUser) => (
									<tr key={albumForUser.album.id}>
										<td style={{ width: '80px' }}>
											{albumForUser.album.mainPicture &&
												albumForUser.album.mainPicture.urlTinyThumb && (
													<Link
														to={EntryUrlMapper.details(
															EntryType.Album,
															albumForUser.album.id,
														)}
														title={albumForUser.album.additionalNames}
														className="coverPicThumb"
													>
														{/* eslint-disable-next-line jsx-a11y/alt-text */}
														<img
															src={albumForUser.album.mainPicture.urlTinyThumb}
															title="Cover picture" /* TODO: localize */
															className="coverPicThumb img-rounded"
														/>
													</Link>
												)}
										</td>
										<td>
											<Link
												to={EntryUrlMapper.details(
													EntryType.Album,
													albumForUser.album.id,
												)}
												title={albumForUser.album.additionalNames}
											>
												{albumForUser.album.name}
											</Link>
											<br />
											<DraftIcon
												status={
													EntryStatus[
														albumForUser.album
															.status as keyof typeof EntryStatus
													]
												}
											/>{' '}
											<small className="extraInfo">
												{albumForUser.album.artistString}
											</small>
											<br />
											<small className="extraInfo">
												{t(
													`VocaDb.Model.Resources.Albums:DiscTypeNames.${albumForUser.album.discType}`,
												)}
											</small>
										</td>
										<td className="search-album-release-date-column">
											<span>{albumForUser.album.releaseDate?.formatted}</span>
											{albumForUser.album.releaseEvent && (
												<span>
													<br />
													<SafeAnchor
														onClick={(): void =>
															runInAction(() => {
																albumCollectionStore.releaseEvent.id =
																	albumForUser.album.releaseEvent?.id;
															})
														}
														href="#"
													>
														{albumForUser.album.releaseEvent.name}
													</SafeAnchor>
												</span>
											)}
										</td>
										{albumCollectionStore.publicCollection && (
											<>
												<td>
													<span>
														{t(
															`Resources:AlbumCollectionStatusNames.${albumForUser.purchaseStatus}`,
														)}
													</span>
												</td>
												<td>
													<span>
														{t(
															`Resources:AlbumMediaTypeNames.${albumForUser.mediaType}`,
														)}
													</span>
												</td>
											</>
										)}
										<td>
											<span title={`${albumForUser.rating}`}>
												{albumCollectionStore
													.ratingStars(albumForUser.rating)
													.map((ratingStar, index) => (
														<React.Fragment key={index}>
															{index > 0 && ' '}
															{/* eslint-disable-next-line jsx-a11y/alt-text */}
															<img
																src={
																	ratingStar.enabled
																		? '/Content/star.png'
																		: '/Content/star_disabled.png'
																}
															/>
														</React.Fragment>
													))}
											</span>
										</td>
									</tr>
								))}
							</tbody>
						</table>
					)}
					{albumCollectionStore.viewMode === 'Tiles' && (
						<AlbumSearchListTiles
							albums={albumCollectionStore.page.map(
								(albumForUser) => albumForUser.album,
							)}
						/>
					)}

					<ServerSidePaging pagingStore={albumCollectionStore.paging} />
				</div>
			</>
		);
	},
);

interface UserAlbumsProps {
	user: UserDetailsContract;
	userDetailsStore: UserDetailsStore;
}

const UserAlbums = ({
	user,
	userDetailsStore,
}: UserAlbumsProps): React.ReactElement => {
	return (
		<>
			<UserDetailsNav user={user} tab="albums" />

			<AlbumCollection
				albumCollectionStore={userDetailsStore.albumCollectionStore}
			/>
		</>
	);
};

export default UserAlbums;
