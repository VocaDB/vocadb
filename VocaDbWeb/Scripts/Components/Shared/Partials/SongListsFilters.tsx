import Button from '@/Bootstrap/Button';
import ButtonGroup from '@/Bootstrap/ButtonGroup';
import Dropdown from '@/Bootstrap/Dropdown';
import TagAutoComplete from '@/Components/KnockoutExtensions/TagAutoComplete';
import SongListsBaseStore, {
	SongListSortRule,
} from '@/Stores/SongList/SongListsBaseStore';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { DebounceInput } from 'react-debounce-input';
import { useTranslation } from 'react-i18next';

import TagFiltersBase from './TagFiltersBase';

interface SongListsFiltersProps {
	songListsBaseStore: SongListsBaseStore;
}

const SongListsFilters = observer(
	({ songListsBaseStore }: SongListsFiltersProps): React.ReactElement => {
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
									`VocaDb.Web.Resources.Views.SongList:SongListSortRuleNames.${songListsBaseStore.sort}`,
								)}{' '}
								<span className="caret" />
							</Dropdown.Toggle>
							<Dropdown.Menu>
								{songListsBaseStore.showEventDateSort && (
									<Dropdown.Item
										onClick={(): void =>
											runInAction(() => {
												songListsBaseStore.sort = SongListSortRule.Date;
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
											songListsBaseStore.sort = SongListSortRule.CreateDate;
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
											songListsBaseStore.sort = SongListSortRule.Name;
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
							songListsBaseStore.showTags && 'active',
							'btn-nomargin',
						)}
						onClick={(): void =>
							runInAction(() => {
								songListsBaseStore.showTags = !songListsBaseStore.showTags;
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
							<DebounceInput
								type="text"
								value={songListsBaseStore.query}
								onChange={(e): void =>
									runInAction(() => {
										songListsBaseStore.query = e.target.value;
									})
								}
								className="input-xlarge"
								placeholder={t('ViewRes.Search:Index.TypeSomething')}
								debounceTimeout={300}
							/>
							{songListsBaseStore.query && (
								<Button
									variant="danger"
									onClick={(): void =>
										runInAction(() => {
											songListsBaseStore.query = '';
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
						<TagFiltersBase tagFilters={songListsBaseStore.tagFilters} />
						<div>
							<TagAutoComplete
								type="text"
								className="input-large"
								onAcceptSelection={songListsBaseStore.tagFilters.addTag}
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
