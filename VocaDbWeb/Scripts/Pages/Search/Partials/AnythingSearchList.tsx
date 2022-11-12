import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { ArtistTypeLabel } from '@/Components/Shared/Partials/Artist/ArtistTypeLabel';
import { EntryCountBox } from '@/Components/Shared/Partials/EntryCountBox';
import { ServerSidePaging } from '@/Components/Shared/Partials/Knockout/ServerSidePaging';
import { DraftIcon } from '@/Components/Shared/Partials/Shared/DraftIcon';
import { SongTypeLabel } from '@/Components/Shared/Partials/Song/SongTypeLabel';
import { EntryContract } from '@/DataContracts/EntryContract';
import { EntryType } from '@/Models/EntryType';
import { AnythingSearchStore } from '@/Stores/Search/AnythingSearchStore';
import { SearchStore, SearchType } from '@/Stores/Search/SearchStore';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const useEntryCategoryName = (): ((
	entry: EntryContract,
) => string | undefined) => {
	const { t } = useTranslation([
		'VocaDb.Model.Resources',
		'VocaDb.Model.Resources.Albums',
		'VocaDb.Model.Resources.Songs',
		'VocaDb.Web.Resources.Domain.ReleaseEvents',
	]);

	return React.useCallback(
		(entry: EntryContract): string | undefined => {
			switch (EntryType[entry.entryType as keyof typeof EntryType]) {
				case EntryType.Artist:
					return t(
						`VocaDb.Model.Resources:ArtistTypeNames.${entry.artistType}`,
					);
				case EntryType.Album:
					return t(
						`VocaDb.Model.Resources.Albums:DiscTypeNames.${entry.discType}`,
					);
				case EntryType.ReleaseEvent:
					return t(
						`VocaDb.Web.Resources.Domain.ReleaseEvents:EventCategoryNames.${entry.eventCategory}`,
					);
				case EntryType.Song:
					return t(
						`VocaDb.Model.Resources.Songs:SongTypeNames.${entry.songType}`,
					);
				default:
					return undefined;
			}
		},
		[t],
	);
};

interface AnythingSearchListProps {
	searchStore: SearchStore;
	anythingSearchStore: AnythingSearchStore;
}

const AnythingSearchList = observer(
	({
		searchStore,
		anythingSearchStore,
	}: AnythingSearchListProps): React.ReactElement => {
		const { t } = useTranslation([
			'ViewRes',
			'ViewRes.Search',
			'VocaDb.Web.Resources.Domain',
		]);

		const entryCategoryName = useEntryCategoryName();

		return (
			<div>
				<EntryCountBox pagingStore={anythingSearchStore.paging} />

				<ServerSidePaging pagingStore={anythingSearchStore.paging} />

				<table
					className={classNames(
						'table',
						'table-striped',
						anythingSearchStore.loading && 'loading',
					)}
				>
					<thead>
						<tr>
							<th colSpan={2}>{t('ViewRes:Shared.Name')}</th>
							<th>{t('ViewRes.Search:Index.EntryType')}</th>
							{anythingSearchStore.showTags && (
								<th>{t('ViewRes:Shared.Tags')}</th>
							)}
						</tr>
					</thead>
					<tbody>
						{anythingSearchStore.page.map((entry) => (
							<tr key={`${entry.entryType}.${entry.id}`}>
								<td style={{ width: '80px' }}>
									{entry.mainPicture && entry.mainPicture.urlTinyThumb && (
										<Link
											to={anythingSearchStore.entryUrl(entry)}
											title={entry.additionalNames}
											className="coverPicThumb"
										>
											{/* eslint-disable-next-line jsx-a11y/alt-text */}
											<img
												src={entry.mainPicture.urlTinyThumb}
												title="Cover picture" /* LOC */
												className="coverPicThumb img-rounded"
												referrerPolicy="same-origin"
											/>
										</Link>
									)}
								</td>
								<td>
									<Link
										to={anythingSearchStore.entryUrl(entry)}
										title={entry.additionalNames}
									>
										{entry.name}
									</Link>
									{entry.artistType && (
										<>
											{' '}
											<ArtistTypeLabel artistType={entry.artistType} />
										</>
									)}
									{entry.songType && (
										<>
											{' '}
											<SongTypeLabel songType={entry.songType} />
										</>
									)}{' '}
									<DraftIcon status={entry.status} />
									{entry.artistString && (
										<>
											<br />
											<small className="extraInfo">{entry.artistString}</small>
										</>
									)}
								</td>
								<td>
									<SafeAnchor
										onClick={(): void =>
											runInAction(() => {
												searchStore.searchType =
													SearchType[
														entry.entryType as keyof typeof SearchType
													];
											})
										}
									>
										{t(
											`VocaDb.Web.Resources.Domain:EntryTypeNames.${entry.entryType}`,
										)}
									</SafeAnchor>
									{(entry.artistType ||
										entry.discType ||
										entry.songType ||
										entry.eventCategory) && (
										<>
											{' '}
											<small className="extraInfo">
												({entryCategoryName(entry)})
											</small>
										</>
									)}
								</td>
								{anythingSearchStore.showTags && (
									<td className="search-tags-column">
										{entry.tags && entry.tags.length > 0 && (
											<div>
												<i className="icon icon-tags fix-icon-margin" />{' '}
												{entry.tags.map((tag, index) => (
													<React.Fragment key={tag.tag.id}>
														{index > 0 && ', '}
														<SafeAnchor
															onClick={(): void =>
																anythingSearchStore.selectTag(tag.tag)
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

				<ServerSidePaging pagingStore={anythingSearchStore.paging} />
			</div>
		);
	},
);

export default AnythingSearchList;
