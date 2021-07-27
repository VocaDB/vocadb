import Button from '@Bootstrap/Button';
import ButtonGroup from '@Bootstrap/ButtonGroup';
import Dropdown from '@Bootstrap/Dropdown';
import TagAutoComplete from '@Components/KnockoutExtensions/TagAutoComplete';
import { FeaturedSongListCategoryStore } from '@Stores/SongList/FeaturedSongListsStore';
import { SongListSortRule } from '@Stores/SongList/SongListsBaseStore';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

import TagFilters from './TagFilters';

interface SongListsFiltersProps {
	featuredSongListCategoryStore: FeaturedSongListCategoryStore;
}

const SongListsFilters = observer(
	({
		featuredSongListCategoryStore,
	}: SongListsFiltersProps): React.ReactElement => {
		const { t } = useTranslation([
			'ViewRes',
			'ViewRes.Search',
			'VocaDb.Web.Resources.Views.SongList',
		]);

		return (
			<div className="form-horizontal well well-transparent">
				<div className="pull-right">
					<div className="inline-block">
						{t('ViewRes:EntryIndex.SortBy')}{' '}
						{/* TODO: use KnockoutHelpers.Dropdown */}
						<Dropdown as={ButtonGroup}>
							<Dropdown.Toggle>
								{t(
									`VocaDb.Web.Resources.Views.SongList:SongListSortRuleNames.${featuredSongListCategoryStore.sort}`,
								)}{' '}
								<span className="caret" />
							</Dropdown.Toggle>
							<Dropdown.Menu>
								{featuredSongListCategoryStore.showEventDateSort && (
									<Dropdown.Item
										onClick={(): void =>
											runInAction(() => {
												featuredSongListCategoryStore.sort =
													SongListSortRule.Date;
											})
										}
									>
										{t(
											`VocaDb.Web.Resources.Views.SongList:SongListSortRuleNames.${SongListSortRule.Date}`,
										)}
									</Dropdown.Item>
								)}
								<Dropdown.Item
									onClick={(): void =>
										runInAction(() => {
											featuredSongListCategoryStore.sort =
												SongListSortRule.CreateDate;
										})
									}
								>
									{t(
										`VocaDb.Web.Resources.Views.SongList:SongListSortRuleNames.${SongListSortRule.CreateDate}`,
									)}
								</Dropdown.Item>
								<Dropdown.Item
									onClick={(): void =>
										runInAction(() => {
											featuredSongListCategoryStore.sort =
												SongListSortRule.Name;
										})
									}
								>
									{t(
										`VocaDb.Web.Resources.Views.SongList:SongListSortRuleNames.${SongListSortRule.Name}`,
									)}
								</Dropdown.Item>
							</Dropdown.Menu>
						</Dropdown>
					</div>{' '}
					<Button
						className={classNames(
							featuredSongListCategoryStore.showTags && 'active',
							'btn-nomargin',
						)}
						onClick={(): void =>
							runInAction(() => {
								featuredSongListCategoryStore.showTags = !featuredSongListCategoryStore.showTags;
							})
						}
						href="#"
						title={t('ViewRes.Search:Index.ShowTags')}
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
							<input
								type="text"
								value={featuredSongListCategoryStore.query}
								onChange={(e): void =>
									runInAction(() => {
										featuredSongListCategoryStore.query = e.target.value;
									})
								}
								className="input-xlarge"
								placeholder={t('ViewRes.Search:Index.TypeSomething')}
							/>
							{featuredSongListCategoryStore.query && (
								<Button
									variant="danger"
									onClick={(): void =>
										runInAction(() => {
											featuredSongListCategoryStore.query = '';
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
					<div className="control-label">{t('ViewRes:Shared.Tag')}</div>
					<div className="controls">
						<TagFilters tagFilters={featuredSongListCategoryStore.tagFilters} />
						<div>
							<TagAutoComplete
								type="text"
								className="input-large"
								onAcceptSelection={
									featuredSongListCategoryStore.tagFilters.addTag
								}
								placeholder={t('ViewRes:Shared.Search')}
							/>
						</div>
					</div>
				</div>
			</div>
		);
	},
);

export default SongListsFilters;
