import Button from '@Bootstrap/Button';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import TagFilterStore from '@Stores/Search/TagFilter';
import TagFiltersStore from '@Stores/Search/TagFilters';
import { observer } from 'mobx-react-lite';
import React from 'react';

interface TagFiltersProps {
	tagFilters: TagFiltersStore;
	onClear: (tag: TagFilterStore) => void;
}

const TagFilters = observer(
	({ tagFilters, onClear }: TagFiltersProps): React.ReactElement => {
		return (
			<>
				{tagFilters.tags.map((tag, index) => (
					<div className="control-group" key={index}>
						<div
							style={{ display: 'inline-block' }}
							className="input-append input-prepend"
						>
							<Button
								className="btn-nomargin"
								href={EntryUrlMapper.details_tag(tag.id, tag.urlSlug)}
							>
								<i className="icon icon-info-sign" />
							</Button>
							<div className="input-append">
								<input
									type="text"
									className="input-large"
									readOnly
									value={tag.name ?? ''}
								/>
								<Button variant="danger" onClick={(): void => onClear(tag)}>
									Clear{/* TODO: localize */}
								</Button>
							</div>
						</div>
					</div>
				))}
			</>
		);
	},
);

export default TagFilters;
