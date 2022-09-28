import { TagAutoComplete } from '@/Components/KnockoutExtensions/TagAutoComplete';
import { ArtistTypeLabel } from '@/Components/Shared/Partials/Artist/ArtistTypeLabel';
import { ArtistTypesDropdownKnockout } from '@/Components/Shared/Partials/Artist/ArtistTypesDropdownKnockout';
import { EntryCountBox } from '@/Components/Shared/Partials/EntryCountBox';
import { ServerSidePaging } from '@/Components/Shared/Partials/Knockout/ServerSidePaging';
import { DraftIcon } from '@/Components/Shared/Partials/Shared/DraftIcon';
import { TagFiltersBase } from '@/Components/Shared/Partials/TagFiltersBase';
import { UserDetailsContract } from '@/DataContracts/User/UserDetailsContract';
import { ArtistType } from '@/Models/Artists/ArtistType';
import { EntryType } from '@/Models/EntryType';
import { UserDetailsNav } from '@/Pages/User/UserDetailsRoutes';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { FollowedArtistsStore } from '@/Stores/User/FollowedArtistsStore';
import { UserDetailsStore } from '@/Stores/User/UserDetailsStore';
import { useLocationStateStore } from '@vocadb/route-sphere';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface FollowedArtistsProps {
	followedArtistsStore: FollowedArtistsStore;
}

const FollowedArtists = observer(
	({ followedArtistsStore }: FollowedArtistsProps): React.ReactElement => {
		const { t } = useTranslation([
			'ViewRes',
			'ViewRes.Search',
			'VocaDb.Model.Resources',
		]);

		useLocationStateStore(followedArtistsStore);

		return (
			<>
				<div className="form-horizontal well well-transparent">
					<div className="control-group">
						<div className="control-label">
							{t('ViewRes.Search:Index.ArtistType')}
						</div>
						<div className="controls">
							<ArtistTypesDropdownKnockout
								value={followedArtistsStore.artistType}
								onChange={(e): void =>
									runInAction(() => {
										followedArtistsStore.artistType = e.target
											.value as ArtistType;
									})
								}
							/>
						</div>
					</div>

					<div className="control-group">
						<div className="control-label">{t('ViewRes:Shared.Tag')}</div>
						<div className="controls">
							<TagFiltersBase tagFilters={followedArtistsStore.tagFilters} />
							<div>
								<TagAutoComplete
									type="text"
									className="input-large"
									onAcceptSelection={followedArtistsStore.tagFilters.addTag}
									placeholder={t('ViewRes:Shared.Search')}
								/>
							</div>
						</div>
					</div>
				</div>

				<div className={classNames(followedArtistsStore.loading && 'loading')}>
					<EntryCountBox pagingStore={followedArtistsStore.paging} />

					<ServerSidePaging pagingStore={followedArtistsStore.paging} />

					<table
						className={classNames(
							'table',
							'table-striped',
							followedArtistsStore.loading && 'loading',
						)}
					>
						<thead>
							<tr>
								<th colSpan={2}>{t('ViewRes:Shared.ArtistName')}</th>
							</tr>
						</thead>
						<tbody>
							{followedArtistsStore.page.map((artistForUser) => (
								<tr key={artistForUser.artist.id}>
									<td style={{ width: '80px' }}>
										{artistForUser.artist.mainPicture &&
											artistForUser.artist.mainPicture.urlTinyThumb && (
												<Link
													to={EntryUrlMapper.details(
														EntryType.Artist,
														artistForUser.artist.id,
													)}
													title={artistForUser.artist.additionalNames}
													className="coverPicThumb"
												>
													{/* eslint-disable-next-line jsx-a11y/alt-text */}
													<img
														src={artistForUser.artist.mainPicture.urlTinyThumb}
														title="Cover picture" /* TODO: localize */
														className="coverPicThumb img-rounded"
													/>
												</Link>
											)}
									</td>
									<td>
										<Link
											to={EntryUrlMapper.details(
												EntryType.Artist,
												artistForUser.artist.id,
											)}
										>
											{artistForUser.artist.name}
										</Link>{' '}
										<ArtistTypeLabel
											artistType={artistForUser.artist.artistType}
										/>{' '}
										<DraftIcon status={artistForUser.artist.status} />
										<br />
										{artistForUser.artist.additionalNames && (
											<span>
												<small className="extraInfo">
													{artistForUser.artist.additionalNames}
												</small>
												<br />
											</span>
										)}
										<small className="extraInfo">
											{t(
												`VocaDb.Model.Resources:ArtistTypeNames:${artistForUser.artist.artistType}`,
											)}
										</small>
									</td>
								</tr>
							))}
						</tbody>
					</table>

					<ServerSidePaging pagingStore={followedArtistsStore.paging} />
				</div>
			</>
		);
	},
);

interface UserArtistsProps {
	user: UserDetailsContract;
	userDetailsStore: UserDetailsStore;
}

const UserArtists = ({
	user,
	userDetailsStore,
}: UserArtistsProps): React.ReactElement => {
	return (
		<>
			<UserDetailsNav user={user} tab="artists" />

			<FollowedArtists
				followedArtistsStore={userDetailsStore.followedArtistsStore}
			/>
		</>
	);
};

export default UserArtists;
