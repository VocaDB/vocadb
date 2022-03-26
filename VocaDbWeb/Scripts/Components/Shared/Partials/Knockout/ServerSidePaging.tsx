import Pagination from '@Bootstrap/Pagination';
import ServerSidePagingStore from '@Stores/ServerSidePagingStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface ServerSidePagingProps {
	pagingStore: ServerSidePagingStore;
}

const ServerSidePaging = observer(
	({ pagingStore }: ServerSidePagingProps): React.ReactElement => {
		const { t } = useTranslation(['VocaDb.Web.Resources.Other']);

		return (
			<Pagination>
				<Pagination.First
					disabled={pagingStore.isFirstPage}
					onClick={pagingStore.goToFirstPage}
				>
					&laquo;&laquo; {t('VocaDb.Web.Resources.Other:PagedList.First')}
				</Pagination.First>
				<Pagination.Prev
					disabled={pagingStore.isFirstPage}
					onClick={pagingStore.previousPage}
				>
					&laquo; {t('VocaDb.Web.Resources.Other:PagedList.Previous')}
				</Pagination.Prev>

				{pagingStore.showMoreBegin && <Pagination.Ellipsis disabled />}

				{pagingStore.pages.map((page) => (
					<Pagination.Item
						active={page === pagingStore.page}
						onClick={(): void =>
							runInAction(() => {
								pagingStore.page = page;
							})
						}
						key={page}
					>
						{page}
					</Pagination.Item>
				))}

				{pagingStore.showMoreEnd && <Pagination.Ellipsis disabled />}

				<Pagination.Next
					disabled={pagingStore.isLastPage}
					onClick={pagingStore.nextPage}
				>
					{t('VocaDb.Web.Resources.Other:PagedList.Next')} &raquo;
				</Pagination.Next>
				<Pagination.Last
					disabled={pagingStore.isLastPage}
					onClick={pagingStore.goToLastPage}
				>
					{t('VocaDb.Web.Resources.Other:PagedList.Last')} &raquo;&raquo;
				</Pagination.Last>
			</Pagination>
		);
	},
);

export default ServerSidePaging;
