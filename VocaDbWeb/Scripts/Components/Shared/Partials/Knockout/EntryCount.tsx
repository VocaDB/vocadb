import Dropdown from '@Bootstrap/Dropdown';
import SafeAnchor from '@Bootstrap/SafeAnchor';
import ServerSidePagingStore from '@Stores/ServerSidePagingStore';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface EntryCountProps {
	pagingStore: ServerSidePagingStore;
	selections?: number[];
}

const EntryCount = observer(
	({
		pagingStore,
		selections = [10, 20, 40, 100],
	}: EntryCountProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.Search']);

		return (
			<Dropdown>
				<Dropdown.Toggle as={SafeAnchor}>
					{t('ViewRes.Search:Index.ShowingItemsOf', {
						0: pagingStore.pageSize,
						1: pagingStore.totalItems,
					})}
				</Dropdown.Toggle>
				<Dropdown.Menu>
					{selections.map((selection) => (
						<Dropdown.Item
							onClick={(): void => pagingStore.setPageSize(selection)}
							key={selection}
						>
							{t('ViewRes.Search:Index.ItemsPerPage', { 0: selection })}
						</Dropdown.Item>
					))}
				</Dropdown.Menu>
			</Dropdown>
		);
	},
);

export default EntryCount;
