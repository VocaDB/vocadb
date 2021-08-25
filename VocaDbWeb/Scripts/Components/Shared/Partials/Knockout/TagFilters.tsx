import ButtonGroup from '@Bootstrap/ButtonGroup';
import Dropdown from '@Bootstrap/Dropdown';
import TagAutoComplete from '@Components/KnockoutExtensions/TagAutoComplete';
import TagFiltersCore from '@Components/Shared/Partials/TagFilters';
import useRedial from '@Components/useRedial';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import SearchStore from '@Stores/Search/SearchStore';
import TagFiltersStore from '@Stores/Search/TagFilters';
import _ from 'lodash';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface TagFiltersProps {
	searchStore: SearchStore;
	tagFilters: TagFiltersStore;
	topGenres: boolean;
}

const TagFilters = observer(
	({
		searchStore,
		tagFilters,
		topGenres,
	}: TagFiltersProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes', 'ViewRes.Search']);
		const redial = useRedial(searchStore.currentCategoryStore.routeParams);

		return (
			<>
				<TagFiltersCore
					tagFilters={tagFilters}
					onClear={(tag): void =>
						redial({
							tagId: _.difference(tagFilters.tags, [tag]).map((t) => t.id),
							page: 1,
						})
					}
				/>

				{tagFilters.tags.length > 0 && (
					<div>
						<label className="checkbox">
							<input
								type="checkbox"
								checked={tagFilters.childTags}
								onChange={(e): void =>
									redial({ childTags: e.target.checked, page: 1 })
								}
							/>
							{t('ViewRes.Search:Index.ChildTags')}
						</label>
					</div>
				)}

				<div>
					<TagAutoComplete
						type="text"
						className="input-large"
						onAcceptSelection={(entry): void =>
							redial({ tagId: tagFilters.tagIds.concat(entry.id), page: 1 })
						}
						placeholder={t('ViewRes:Shared.Search')}
					/>

					{topGenres && (
						<>
							{' '}
							<Dropdown as={ButtonGroup}>
								<Dropdown.Toggle>
									{t('ViewRes.Search:Index.TopGenres')}{' '}
									<span className="caret" />
								</Dropdown.Toggle>
								<Dropdown.Menu>
									{searchStore.genreTags.map((tag) => (
										<Dropdown.Item
											href={EntryUrlMapper.details_tag_contract(tag)}
											onClick={(e): void => {
												e.preventDefault();
												redial({
													tagId: tagFilters.tagIds.concat(tag.id),
													page: 1,
												});
											}}
											key={tag.id}
										>
											{tag.name}
										</Dropdown.Item>
									))}
								</Dropdown.Menu>
							</Dropdown>
						</>
					)}
				</div>
			</>
		);
	},
);

export default TagFilters;
