import ButtonGroup from '@Bootstrap/ButtonGroup';
import Dropdown from '@Bootstrap/Dropdown';
import TagAutoComplete from '@Components/KnockoutExtensions/TagAutoComplete';
import TagFiltersCore from '@Components/Shared/Partials/TagFilters';
import TagBaseContract from '@DataContracts/Tag/TagBaseContract';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import TagFiltersStore from '@Stores/Search/TagFilters';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface TagFiltersProps {
	tagFilters: TagFiltersStore;
	genreTags?: TagBaseContract[];
}

const TagFilters = observer(
	({ tagFilters, genreTags }: TagFiltersProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes', 'ViewRes.Search']);

		return (
			<>
				<TagFiltersCore tagFilters={tagFilters} />

				{tagFilters.tags.length > 0 && (
					<div>
						<label className="checkbox">
							<input
								type="checkbox"
								checked={tagFilters.childTags}
								onChange={(e): void =>
									runInAction(() => {
										tagFilters.childTags = e.target.checked;
									})
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
						onAcceptSelection={tagFilters.addTag}
						placeholder={t('ViewRes:Shared.Search')}
					/>

					{genreTags && (
						<>
							{' '}
							<Dropdown as={ButtonGroup}>
								<Dropdown.Toggle>
									{t('ViewRes.Search:Index.TopGenres')}{' '}
									<span className="caret" />
								</Dropdown.Toggle>
								<Dropdown.Menu>
									{genreTags.map((tag) => (
										<Dropdown.Item
											href={EntryUrlMapper.details_tag_contract(tag)}
											onClick={(e): void => {
												e.preventDefault();
												tagFilters.addTag(tag);
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
