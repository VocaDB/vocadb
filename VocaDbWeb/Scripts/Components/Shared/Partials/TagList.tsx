import SafeAnchor from '@Bootstrap/SafeAnchor';
import { TagToolTip } from '@Components/KnockoutExtensions/EntryToolTip';
import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import TagListStore from '@Stores/Tag/TagListStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface LinkListProps {
	tagUsages: TagUsageForApiContract[];
}

const LinkList = React.memo(
	({ tagUsages }: LinkListProps): React.ReactElement => {
		return (
			<ul className="link-list">
				{tagUsages.map((tagUsage, index) => (
					<React.Fragment key={tagUsage.tag.id}>
						{index > 0 && ' '}
						<li className="link-item">
							<span>
								<TagToolTip
									as="a"
									href={EntryUrlMapper.details_tag(
										tagUsage.tag.id,
										tagUsage.tag.urlSlug,
									)}
									title={tagUsage.tag.additionalNames}
									id={tagUsage.tag.id}
								>
									{tagUsage.tag.name}
								</TagToolTip>{' '}
								<small className="muted">({tagUsage.count})</small>
							</span>
						</li>
					</React.Fragment>
				))}
			</ul>
		);
	},
);

interface TagListProps {
	tagListStore: TagListStore;
}

const TagList = observer(
	({ tagListStore }: TagListProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		return (
			<>
				{tagListStore.expanded ? (
					<dl style={{ margin: 0 }} className="dl-horizontal">
						{tagListStore.tagUsagesByCategories.map((category) => (
							<React.Fragment key={category.categoryName}>
								<dt
									style={{ fontWeight: 'normal', width: 'auto', float: 'left' }}
								>
									{category.categoryName}
								</dt>
								<dd style={{ marginLeft: '80px' }}>
									<LinkList tagUsages={category.tagUsages} />
								</dd>
							</React.Fragment>
						))}
					</dl>
				) : (
					<>
						<LinkList tagUsages={tagListStore.displayedTagUsages} />{' '}
						<span>
							-{' '}
							<SafeAnchor
								onClick={(): void =>
									runInAction(() => {
										tagListStore.expanded = true;
									})
								}
								href="#"
							>
								{t('ViewRes:EntryDetails.ShowAllTags')} (
								<span>{tagListStore.tagUsages.length}</span>)
							</SafeAnchor>
						</span>
					</>
				)}
			</>
		);
	},
);

export default TagList;
