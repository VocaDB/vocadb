import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { ArtistTypeLabel } from '@/Components/Shared/Partials/Artist/ArtistTypeLabel';
import { EntryCountBox } from '@/Components/Shared/Partials/EntryCountBox';
import { ServerSidePaging } from '@/Components/Shared/Partials/Knockout/ServerSidePaging';
import { DraftIcon } from '@/Components/Shared/Partials/Shared/DraftIcon';
import { EntryType } from '@/Models/EntryType';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import {
	ArtistSearchStore,
	ArtistSortRule,
} from '@/Stores/Search/ArtistSearchStore';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface ArtistSearchListProps {
	artistSearchStore: ArtistSearchStore;
}

const ArtistSearchList = observer(
	({ artistSearchStore }: ArtistSearchListProps): React.ReactElement => {
		const { t } = useTranslation([
			'ViewRes',
			'ViewRes.Search',
			'VocaDb.Model.Resources',
		]);

		return (
			<div>
				<EntryCountBox pagingStore={artistSearchStore.paging} />

				<ServerSidePaging pagingStore={artistSearchStore.paging} />

				<table
					className={classNames(
						'table',
						'table-striped',
						artistSearchStore.loading && 'loading',
					)}
				>
					<thead>
						<tr>
							<th colSpan={2}>
								<SafeAnchor
									onClick={(): void =>
										runInAction(() => {
											artistSearchStore.sort = ArtistSortRule.Name;
										})
									}
								>
									{t('ViewRes:Shared.ArtistName')}
									{artistSearchStore.sort === ArtistSortRule.Name && (
										<>
											{' '}
											<span className="sortDirection_down" />
										</>
									)}
								</SafeAnchor>
							</th>
							<th>{t('ViewRes.Search:Index.ArtistType')}</th>
							<th></th>
							{artistSearchStore.showTags && (
								<th>{t('ViewRes:Shared.Tags')}</th>
							)}
						</tr>
					</thead>
					<tbody>
						{artistSearchStore.page.map((artist) => (
							<tr key={artist.id}>
								<td style={{ width: '80px' }}>
									{artist.mainPicture && artist.mainPicture.urlTinyThumb && (
										<Link
											to={EntryUrlMapper.details(EntryType.Artist, artist.id)}
											title={artist.additionalNames}
											className="coverPicThumb"
										>
											{/* eslint-disable-next-line jsx-a11y/alt-text */}
											<img
												src={artist.mainPicture.urlTinyThumb}
												title="Cover picture" /* LOC */
												className="coverPicThumb img-rounded"
												referrerPolicy="same-origin"
											/>
										</Link>
									)}
								</td>
								<td>
									<Link
										to={EntryUrlMapper.details(EntryType.Artist, artist.id)}
									>
										{artist.name}
									</Link>{' '}
									<ArtistTypeLabel artistType={artist.artistType} />{' '}
									<DraftIcon status={artist.status} />
									<br />
									<small className="extraInfo">{artist.additionalNames}</small>
								</td>
								<td>
									<span>
										{t(
											`VocaDb.Model.Resources:ArtistTypeNames.${artist.artistType}`,
										)}
									</span>
								</td>
								<td>
									{artist.releaseDate && (
										<>
											<span>
												{t('ViewRes.Search:Index.VoicebankReleaseDate')}
											</span>{' '}
											<span>
												{artistSearchStore.formatDate(artist.releaseDate)}
											</span>
										</>
									)}
								</td>
								{artistSearchStore.showTags && (
									<td className="search-tags-column">
										{artist.tags && artist.tags.length > 0 && (
											<div>
												<i className="icon icon-tags fix-icon-margin" />{' '}
												{artist.tags.map((tag, index) => (
													<React.Fragment key={tag.tag.id}>
														{index > 0 && ', '}
														<SafeAnchor
															onClick={(): void =>
																artistSearchStore.selectTag(tag.tag)
															}
														>
															{tag.tag.name}
														</SafeAnchor>
													</React.Fragment>
												))}
											</div>
										)}
									</td>
								)}
							</tr>
						))}
					</tbody>
				</table>

				<ServerSidePaging pagingStore={artistSearchStore.paging} />
			</div>
		);
	},
);

export default ArtistSearchList;
