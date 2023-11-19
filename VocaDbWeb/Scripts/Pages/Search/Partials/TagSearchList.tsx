import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { EntryCountBox } from '@/Components/Shared/Partials/EntryCountBox';
import { ServerSidePaging } from '@/Components/Shared/Partials/Knockout/ServerSidePaging';
import { DraftIcon } from '@/Components/Shared/Partials/Shared/DraftIcon';
import { TagApiContract } from '@/DataContracts/Tag/TagApiContract';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { TagSearchStore, TagSortRule } from '@/Stores/Search/TagSearchStore';
import { ServerSidePagingStore } from '@/Stores/ServerSidePagingStore';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface ITagSearchStore {
	loading: boolean;
	page: TagApiContract[];
	paging: ServerSidePagingStore;
	sort: TagSortRule;
}

interface TagSearchListProps {
	tagSearchStore: ITagSearchStore;
}

const TagSearchList = observer(
	({ tagSearchStore }: TagSearchListProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes', 'ViewRes.Search']);

		return (
			<>
				<EntryCountBox pagingStore={tagSearchStore.paging} />

				<ServerSidePaging pagingStore={tagSearchStore.paging} />

				<table
					className={classNames(
						'table',
						'table-striped',
						tagSearchStore.loading && 'loading',
					)}
				>
					<thead>
						<tr>
							<th colSpan={2}>
								<SafeAnchor
									onClick={(): void =>
										runInAction(() => {
											tagSearchStore.sort = TagSortRule.Name;
										})
									}
								>
									{t('ViewRes:Shared.Name')}
									{tagSearchStore.sort === TagSortRule.Name && (
										<>
											{' '}
											<span className="sortDirection_down" />
										</>
									)}
								</SafeAnchor>
							</th>
							<th>{t('ViewRes.Search:Index.Category')}</th>
							<th>
								<SafeAnchor
									onClick={(): void =>
										runInAction(() => {
											tagSearchStore.sort = TagSortRule.UsageCount;
										})
									}
								>
									{t('ViewRes.Search:Index.TagUsages')}
									{tagSearchStore.sort === TagSortRule.UsageCount && (
										<>
											{' '}
											<span className="sortDirection_down" />
										</>
									)}
								</SafeAnchor>
							</th>
						</tr>
					</thead>
					<tbody>
						{tagSearchStore.page.map((tag) => (
							<tr key={tag.id}>
								<td style={{ width: '80px' }}>
									{tag.mainPicture && tag.mainPicture.urlSmallThumb && (
										<Link
											to={EntryUrlMapper.details_tag(tag.id, tag.urlSlug)}
											title={tag.additionalNames}
										>
											{/* eslint-disable-next-line jsx-a11y/alt-text */}
											<img
												src={tag.mainPicture.urlSmallThumb}
												title="Cover picture" /* LOC */
												className="coverPicThumb img-rounded"
											/>
										</Link>
									)}
								</td>
								<td>
									<Link to={EntryUrlMapper.details_tag(tag.id, tag.urlSlug)}>
										{tag.name}
									</Link>{' '}
									<DraftIcon status={tag.status} />
									<br />
									<small className="extraInfo">{tag.additionalNames}</small>
								</td>
								<td>
									<p>{tag.categoryName}</p>
								</td>
								<td>
									<p>{tag.usageCount}</p>
								</td>
							</tr>
						))}
					</tbody>
				</table>

				<ServerSidePaging pagingStore={tagSearchStore.paging} />
			</>
		);
	},
);

export default TagSearchList;
