import TagCategoryLockingAutoComplete from '@Components/Shared/Partials/Knockout/TagCategoryLockingAutoComplete';
import TagSearchStore from '@Stores/Search/TagSearchStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface TagSearchOptionsProps {
	tagSearchStore: TagSearchStore;
}

const TagSearchOptions = observer(
	({ tagSearchStore }: TagSearchOptionsProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.Search']);

		return (
			<div className="control-group">
				<div className="control-label">
					{t('ViewRes.Search:Index.TagCategory')}
				</div>
				<div className="controls">
					<div className="input-append">
						<TagCategoryLockingAutoComplete
							value={tagSearchStore.categoryName}
							onChange={(value): void =>
								runInAction(() => {
									tagSearchStore.categoryName = value;
								})
							}
							clearValue={true}
						/>
					</div>
				</div>
			</div>
		);
	},
);

export default TagSearchOptions;
